using GremlinT.Core.Abstractions;
using JetBrains.Annotations;

namespace GremlinT.Core.Tests.QueryTests;

public class GraphTraversalQueryTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [UsedImplicitly]
    private class Person : IVertex { public Guid Id { get; set; } }

    [UsedImplicitly]
    private class Car : IVertex { public Guid Id { get; set; } }

    [UsedImplicitly]
    private class Company : IVertex { public Guid Id { get; set; } }

    [UsedImplicitly]
    private class Owns : IEdge { public Guid Id { get; set; } }

    [UsedImplicitly]
    private class Knows : IEdge { public Guid Id { get; set; } }

    [Fact]
    public void Out_ViaEdgeType_ThenTypedVertex_ProducesFullTraversal()
    {
        G.V<Person>(TenantId).Out<Owns>().V<Car>()
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').out('Owns').hasLabel('Car')");
    }

    [Fact]
    public void Out_WithLabelAndTargetType_ProducesTypedTraversal()
    {
        G.V<Person>(TenantId).Out<Knows>()
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').out('Knows')");
    }

    [Fact]
    public void In_ViaEdgeType_ProducesInTraversal()
    {
        G.V<Car>(TenantId).In<Owns>()
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Car').in('Owns')");
    }

    [Fact]
    public void MultiHop_Out_ThenIn_ViaEdgeType_ProducesRoundTripTraversal()
    {
        G.V<Person>(TenantId).Out<Owns>().V<Car>().In<Owns>().V<Person>()
            .ToString()
            .Is(
                $"g.V().has('tenantId','{TenantId}').hasLabel('Person').out('Owns').hasLabel('Car').in('Owns').hasLabel('Person')");
    }

    [Fact]
    public void OutE_ThenInV_ThenHasLabel_ProducesEdgeVertexTraversal()
    {
        G.V<Person>(TenantId).OutE("knows").InV().HasLabel<Person>()
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').outE('knows').inV().hasLabel('Person')");
    }

    [Fact]
    public void InE_ThenOutV_ThenHasLabel_ProducesReverseEdgeTraversal()
    {
        G.V<Car>(TenantId).InE("Owns").OutV().HasLabel<Person>()
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Car').inE('Owns').outV().hasLabel('Person')");
    }

    [Fact]
    public void OutE_ThenOtherV_ProducesAdjacentVertexTraversal()
    {
        G.V<Person>(TenantId).OutE("knows").OtherV()
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').outE('knows').otherV()");
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
                $"g.V().has('tenantId','{TenantId}').hasLabel('Person').has('Name','Alice').out('Knows').hasLabel('Person').has('Name','Bob').out('Owns').hasLabel('Car')");
    }
}
