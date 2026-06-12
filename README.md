# GremlinT

A .NET library that provides a strongly-typed, fluent API for building [Gremlin](https://tinkerpop.apache.org/gremlin.html) graph traversal queries. Instead of writing raw Gremlin strings by hand, you compose queries through C# methods that enforce correct structure and leverage the compiler for type safety.

---

## What does it do?

GremlinT lets you build Gremlin traversal query strings using a fluent C# API. The final output is a valid Gremlin query string that can be sent to any Gremlin-compatible graph database (e.g. Azure Cosmos DB for Gremlin, Amazon Neptune, JanusGraph).

**Without GremlinT:**
```
g.V().hasLabel('Person').has('age','30').order().by('name',asc).limit(10)
```

**With GremlinT:**
```csharp
new PersonTraversal()
    .HasLabel<Person>()
    .Has("age", 30)
    .OrderByAscending("name")
    .Limit(10)
    .ToString()
```

---

## How is it achieved?

The library uses a combination of **generic inheritance** and the **Curiously Recurring Template Pattern (CRTP)** to allow method chaining to return the most-derived type (`TSelf`) instead of the base type. This means every step in the traversal chain returns the concrete subclass, preserving the full API of the derived type throughout the chain.

Query building is done by appending Gremlin step strings to an internal `StringBuilder`. When the traversal is complete, calling `ToString()` (or implicitly converting to `string`) returns the full query string.

---

## Components

### `GraphTraversal` (base class)

`GremlinT.Core/GraphTraversal.cs`

The root abstract class. Holds the internal `StringBuilder` that accumulates the Gremlin steps. Provides:

- `ToString()` - returns the assembled query string.
- An implicit conversion operator to `string`, so traversals can be passed directly wherever a string is expected (e.g. as a sub-traversal argument).

### `GraphTraversal<TSelf>` (generic base class)

`GremlinT.Core/GraphTraversalT.cs`

The core fluent API layer. Inherits from `GraphTraversal` and is parameterized by `TSelf`, which must itself be a `GraphTraversal<TSelf>`. Every method appends a Gremlin step to the `StringBuilder` and returns `(TSelf)this`, enabling unbroken method chaining on the concrete type.

**Available steps:**

| Method | Gremlin Output |
|---|---|
| `HasId(string)` / `HasId(Guid)` | `.hasId('...')` |
| `Has(string, string/int/long/bool/Enum)` | `.has('key','value')` |
| `HasLabel(params string[])` | `.hasLabel('...')` |
| `HasLabel<T>()` | `.hasLabel('TypeName')` |
| `Where(GraphTraversal)` | `.where(...)` |
| `IsNotEqual(long)` | `.is(neq(...))` |
| `Count()` | `.count()` |
| `Fold()` | `.fold()` |
| `Id()` | `.id()` |
| `Limit(long)` | `.limit(n)` |
| `As(string)` | `.as('alias')` |
| `Values(params string[])` | `.values('key',...)` |
| `Value()` | `.value()` |
| `Constant(string)` | `.constant('value')` |
| `Union(params GraphTraversal[])` | `.union(...)` |
| `Coalesce(IEnumerable<GraphTraversal>)` | `.coalesce(...)` |
| `OrderByAscending(string)` | `.order().by('key',asc)` |
| `OrderByDescending(string)` | `.order().by('key',desc)` |

### `Vertex`

`GremlinT.Core/Vertex.cs`

Abstract base class for graph vertex models. Provides a `Guid Id` property. Domain vertex types inherit from this to represent graph nodes (e.g. `Person`, `Product`).

### `Edge`

`GremlinT.Core/Edge.cs`

Abstract base class for graph edge models. Provides a `Guid Id` property. Domain edge types inherit from this to represent relationships between vertices (e.g. `Knows`, `Purchased`).

### Component Relationships

```
Vertex / Edge
    └── Domain models (e.g. Person, Order)

GraphTraversal
    └── GraphTraversal<TSelf>
            └── Concrete traversal classes (e.g. PersonTraversal)
```

Concrete traversals are thin subclasses that close the generic parameter:

```csharp
public class PersonTraversal : GraphTraversal<PersonTraversal>
{
    public PersonTraversal() : base(new StringBuilder("g.V()")) { }
}
```

---

## Patterns Applied

### Curiously Recurring Template Pattern (CRTP)

`GraphTraversal<TSelf> where TSelf : GraphTraversal<TSelf>` ensures every fluent method returns the concrete derived type rather than the base, so the derived type's own methods remain available throughout the chain without casting.

### Fluent Builder / Method Chaining

Each step method calls `Write(step)` which appends to the `StringBuilder` and returns `this` as `TSelf`. The traversal is assembled incrementally and materialized only when `ToString()` is called.

### Implicit Conversion Operator

`GraphTraversal` defines `public static implicit operator string(GraphTraversal q)`, so a traversal can be passed as a sub-traversal argument to `Where`, `Union`, or `Coalesce` without an explicit `.ToString()` call.

### Type-safe Label Resolution

`HasLabel<T>()` uses `typeof(T).Name` to derive the Gremlin label from the C# class name, eliminating magic strings for labels when domain model class names match graph labels.

---

## Query Examples

### Find a vertex by ID

```csharp
var query = new PersonTraversal()
    .HasId(new Guid("a1b2c3d4-..."))
    .ToString();
// g.V().hasId('a1b2c3d4-...')
```

### Filter by property

```csharp
var query = new PersonTraversal()
    .HasLabel<Person>()
    .Has("status", true)
    .Has("age", 30)
    .ToString();
// g.V().hasLabel('Person').has('status','1').has('age','30')
```

### Order and page results

```csharp
var query = new PersonTraversal()
    .HasLabel<Person>()
    .OrderByAscending("lastName")
    .Limit(20)
    .ToString();
// g.V().hasLabel('Person').order().by('lastName',asc).limit(20)
```

### Enum-based filter

```csharp
var query = new PersonTraversal()
    .HasLabel<Person>()
    .Has("role", UserRole.Admin)
    .ToString();
// g.V().hasLabel('Person').has('role','Admin')
```

---

## Complex Query Examples

### Conditional sub-traversal with `Where`

Find all persons whose age is not equal to 0:

```csharp
var ageCheck = new PersonTraversal()
    .Values("age")
    .IsNotEqual(0);

var query = new PersonTraversal()
    .HasLabel<Person>()
    .Where(ageCheck)
    .ToString();
// g.V().hasLabel('Person').where(__.values('age').is(neq(0)))
```

### `Coalesce` for default value fallback

Return the nickname if it exists, otherwise fall back to the full name:

```csharp
var nickname = new PersonTraversal().Values("nickname");
var fullName = new PersonTraversal().Values("fullName");

var query = new PersonTraversal()
    .HasId(personId)
    .Coalesce([nickname, fullName])
    .ToString();
// g.V().hasId('...').coalesce(__.values('nickname'),__.values('fullName'))
```

### `Union` to merge multiple traversal results

Retrieve both email and phone values in one step:

```csharp
var email = new PersonTraversal().Values("email");
var phone = new PersonTraversal().Values("phone");

var query = new PersonTraversal()
    .HasId(personId)
    .Union(email, phone)
    .ToString();
// g.V().hasId('...').union(__.values('email'),__.values('phone'))
```

### Aliasing with `As` and counting

Count how many persons share the same city, excluding those with count 0:

```csharp
var countCheck = new PersonTraversal()
    .Count()
    .IsNotEqual(0);

var query = new PersonTraversal()
    .HasLabel<Person>()
    .Has("city", "Amsterdam")
    .As("p")
    .Where(countCheck)
    .OrderByDescending("joinedAt")
    .Limit(5)
    .Fold()
    .ToString();
// g.V().hasLabel('Person').has('city','Amsterdam').as('p')
//   .where(__.count().is(neq(0))).order().by('joinedAt',desc).limit(5).fold()
```
