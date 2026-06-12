using GremlinT.Core.Abstractions;
using JetBrains.Annotations;

namespace GremlinT.Core.Tests;

public class VertexQueryTTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [UsedImplicitly]
    private class Person : Vertex
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public bool HasHouse { get; set; }
    }

    [UsedImplicitly]
    private class Employee : Vertex;

    [UsedImplicitly]
    private class Car : Vertex;

    [UsedImplicitly]
    private class Owns : Edge;

    [UsedImplicitly]
    private class PersonView
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    [Fact]
    public void Out_WithTargetType_AppendsOutStep()
    {
        G.V<Person>(TenantId).Out<Owns>()
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').out('Owns')");
    }

    [Fact]
    public void Out_WithTargetType_ReturnsVertexQueryOfTargetType()
    {
        G.V<Person>(TenantId).Out<Owns>().IsInstanceOf<VertexQuery>();
    }

    [Fact]
    public void In_WithTargetType_AppendsInStep()
    {
        G.V<Person>(TenantId).In<Owns>()
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').in('Owns')");
    }

    [Fact]
    public void In_WithTargetType_ReturnsVertexQueryOfTargetType()
    {
        G.V<Person>(TenantId).In<Owns>().IsInstanceOf<VertexQuery>();
    }

    [Fact]
    public void Has_OfTypedVertexWithStringValue_UsesPropertyName()
    {
        G.V<Person>(TenantId).Has(x => x.Name, "Thiago")
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').has('Name','Thiago')");
    }

    [Fact]
    public void Has_OfTypedVertexWithBooleanValue_UsesPropertyName()
    {
        G.V<Person>(TenantId).Has(x => x.HasHouse, true)
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').has('HasHouse','1')");
    }

    [Fact]
    public void Properties_FromSingleLambda_UsesPropertyName()
    {
        G.V<Person>(TenantId).Properties(x => x.Name)
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').properties('Name')");
    }

    [Fact]
    public void Properties_FromMultipleLambdas_JoinsPropertyNames()
    {
        G.V<Person>(TenantId).Properties(x => x.Name, x => x.Age)
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').properties('Name','Age')");
    }

    [Fact]
    public void ValueMap_FromLambdas_UsesPropertyNames()
    {
        G.V<Person>(TenantId).ValueMap(x => x.Name, x => x.Age)
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').valueMap('Name','Age')");
    }

    [Fact]
    public void ValueMap_WithIncludeSystemFields_FromLambdas_PrependsTrueFlag()
    {
        G.V<Person>(TenantId).ValueMap(true, x => x.Name)
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').valueMap(true,'Name')");
    }

    [Fact]
    public void Where_WithBuilderFunc_WrapsAnonTraversal()
    {
        G.V<Person>(TenantId).Where(q => q.Has("name", "Alice"))
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').where(__.has('name','Alice'))");
    }

    [Fact]
    public void Project_WithTypedSelectors_AppendsAllPropertyNames()
    {
        G.V<Person>(TenantId).Project<PersonView>(x => x.Name, x => x.Age)
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').project('Name','Age')");
    }

    [Fact]
    public void Project_WithTypedSelectors_ReturnsTypedVertexProjection()
    {
        G.V<Person>(TenantId).Project<PersonView>(x => x.Name).IsInstanceOf<VertexProjection<Person, PersonView>>();
    }

    [Fact]
    public void Out_WithEdgeType_DerivesLabelFromTypeName()
    {
        G.V<Person>(TenantId).Out<Owns>()
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').out('Owns')");
    }

    [Fact]
    public void Out_WithEdgeType_ReturnsUntypedVertexQuery()
    {
        G.V<Person>(TenantId).Out<Owns>().IsInstanceOf<VertexQuery>();
    }

    [Fact]
    public void In_WithEdgeType_DerivesLabelFromTypeName()
    {
        G.V<Car>(TenantId).In<Owns>()
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Car').in('Owns')");
    }

    [Fact]
    public void In_WithEdgeType_ReturnsUntypedVertexQuery()
    {
        G.V<Car>(TenantId).In<Owns>().IsInstanceOf<VertexQuery>();
    }
}
