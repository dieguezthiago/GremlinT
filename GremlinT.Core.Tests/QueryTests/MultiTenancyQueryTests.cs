using GremlinT.Core.Abstractions;
using JetBrains.Annotations;

namespace GremlinT.Core.Tests.QueryTests;

public class MultiTenancyQueryTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid ElementId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly FullyQualifiedId FqId = new(TenantId, ElementId);

    [UsedImplicitly]
    private class Person : Vertex
    {
        public string Name { get; set; } = "";
    }

    [UsedImplicitly]
    private class Car : Vertex
    {
        public string Make { get; set; } = "";
    }

    [UsedImplicitly]
    private class Owns : Edge;

    [UsedImplicitly]
    private class Knows : Edge;

    [Fact]
    public void TenantId_IsNotRepeated_AfterMultiHopTraversal()
    {
        // TenantId is applied once at the root; subsequent hops do not re-add it
        G.V<Person>(TenantId).Out<Owns>().V<Car>().Has("Make", "Toyota")
            .ToString()
            .Is(
                $"g.V().has('tenantId','{TenantId}').hasLabel('Person').out('Owns').hasLabel('Car').has('Make','Toyota')"
            );
    }

    [Fact]
    public void FqId_ScopesTraversal_ToSpecificElementAndTenant()
    {
        G.V<Person>(FqId).Out<Owns>().V<Car>()
            .ToString()
            .Is($"g.V('{ElementId}').has('tenantId','{TenantId}').hasLabel('Person').out('Owns').hasLabel('Car')");
    }

    [Fact]
    public void FqId_OnEdge_ScopesTraversal_ToSpecificEdge()
    {
        G.E<Knows>(FqId).Property("weight", "1.0")
            .ToString()
            .Is($"g.E('{ElementId}').has('tenantId','{TenantId}').hasLabel('Knows').property('weight','1.0')");
    }

    [Fact]
    public void TenantId_OnEdge_PersistsThrough_EdgeMutation()
    {
        G.E(TenantId).From("a").To("b").Property("weight", "high")
            .ToString()
            .Is($"g.E().has('tenantId','{TenantId}').from('a').to('b').property('weight','high')");
    }

    [Fact]
    public void TenantId_OnTypedVertex_PersistsThrough_FilterAndProjection()
    {
        G.V<Person>(TenantId)
            .Has("Name", "Alice")
            .ValueMap(x => x.Name)
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').has('Name','Alice').valueMap('Name')");
    }

    [Fact]
    public void FqId_OnTypedVertex_PersistsThrough_MultiHopWithFilter()
    {
        G.V<Person>(FqId)
            .Out<Knows>().V<Person>()
            .Has("Name", "Bob")
            .ToString()
            .Is(
                $"g.V('{ElementId}').has('tenantId','{TenantId}').hasLabel('Person').out('Knows').hasLabel('Person').has('Name','Bob')"
            );
    }
}
