using JetBrains.Annotations;

namespace TypedGremlin.Core.Tests.QueryTests;

public class GraphTraversalQueryTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

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
    private class Company : Vertex;

    [UsedImplicitly]
    private class Owns : Edge;

    [UsedImplicitly]
    private class Knows : Edge;

    [Fact]
    public void Out_ViaEdgeType_ThenTypedVertex_ProducesFullTraversal()
    {
        G.V<Person>(TenantId).Out<Owns>().V<Car>()
            .ToString()
            .Is($"g.V().hasLabel('Person').has('tenantId','{TenantId}').out('Owns').hasLabel('Car')");
    }

    [Fact]
    public void Out_WithLabelAndTargetType_ProducesTypedTraversal()
    {
        G.V<Person>(TenantId).Out<Company>("worksAt")
            .ToString()
            .Is($"g.V().hasLabel('Person').has('tenantId','{TenantId}').out('worksAt')");
    }

    [Fact]
    public void In_ViaEdgeType_ProducesInTraversal()
    {
        G.V<Car>(TenantId).In<Owns>()
            .ToString()
            .Is($"g.V().hasLabel('Car').has('tenantId','{TenantId}').in('Owns')");
    }

    [Fact]
    public void MultiHop_Out_ThenIn_ViaEdgeType_ProducesRoundTripTraversal()
    {
        G.V<Person>(TenantId).Out<Owns>().V<Car>().In<Owns>().V<Person>()
            .ToString()
            .Is(
                $"g.V().hasLabel('Person').has('tenantId','{TenantId}').out('Owns').hasLabel('Car').in('Owns').hasLabel('Person')");
    }

    [Fact]
    public void OutE_ThenInV_ThenHasLabel_ProducesEdgeVertexTraversal()
    {
        G.V<Person>(TenantId).OutE("knows").InV().HasLabel<Person>()
            .ToString()
            .Is($"g.V().hasLabel('Person').has('tenantId','{TenantId}').outE('knows').inV().hasLabel('Person')");
    }

    [Fact]
    public void InE_ThenOutV_ThenHasLabel_ProducesReverseEdgeTraversal()
    {
        G.V<Car>(TenantId).InE("Owns").OutV().HasLabel<Person>()
            .ToString()
            .Is($"g.V().hasLabel('Car').has('tenantId','{TenantId}').inE('Owns').outV().hasLabel('Person')");
    }

    [Fact]
    public void OutE_ThenOtherV_ProducesAdjacentVertexTraversal()
    {
        G.V<Person>(TenantId).OutE("knows").OtherV()
            .ToString()
            .Is($"g.V().hasLabel('Person').has('tenantId','{TenantId}').outE('knows').otherV()");
    }

    [Fact]
    public void MultiHop_WithFiltersAtEachStep_ProducesComplexTraversal()
    {
        G.V<Person>(TenantId)
            .Has("Name", "Alice")
            .Out<Knows>().V<Person>()
            .Has("Name", "Bob")
            .Out<Owns>().V<Car>()
            .ToString()
            .Is(
                $"g.V().hasLabel('Person').has('tenantId','{TenantId}').has('Name','Alice').out('Knows').hasLabel('Person').has('Name','Bob').out('Owns').hasLabel('Car')");
    }
}