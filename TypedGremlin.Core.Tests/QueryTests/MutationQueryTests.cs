using JetBrains.Annotations;

namespace TypedGremlin.Core.Tests.QueryTests;

public class MutationQueryTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [UsedImplicitly]
    private class Person : Vertex;

    [UsedImplicitly]
    private class Car : Vertex;

    [UsedImplicitly]
    private class Knows : Edge;

    [Fact]
    public void AddV_WithTypedVertex_ProducesAddVStep()
    {
        G.AnonV.AddV<Person>()
            .ToString()
            .Is("__.addV('Person')");
    }

    [Fact]
    public void AddE_WithFromAndTo_ProducesEdgeMutation()
    {
        G.V(TenantId).AddE("knows").From("v1").To("v2")
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').addE('knows').from('v1').to('v2')");
    }

    [Fact]
    public void AddE_WithFromToAndProperty_ProducesFullEdgeMutation()
    {
        G.V(TenantId).AddE("knows").From("v1").To("v2").Property("since", "2023")
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').addE('knows').from('v1').to('v2').property('since','2023')");
    }

    [Fact]
    public void AddE_ThenInV_ThenFilter_ProducesFullMutationWithTraversal()
    {
        G.V(TenantId).AddE("knows").From("v1").To("v2").InV().HasLabel<Person>()
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').addE('knows').from('v1').to('v2').inV().hasLabel('Person')");
    }

    [Fact]
    public void EdgeProperty_OnExistingTypedEdge_ProducesPropertyStep()
    {
        G.E<Knows>(TenantId).Property("weight", "1.0")
            .ToString()
            .Is($"g.E().has('tenantId','{TenantId}').hasLabel('Knows').property('weight','1.0')");
    }

    [Fact]
    public void AddV_ThenAddE_ProducesVertexAndEdgeMutation()
    {
        G.AnonV.AddV<Person>().AddE("knows")
            .ToString()
            .Is("__.addV('Person').addE('knows')");
    }
}