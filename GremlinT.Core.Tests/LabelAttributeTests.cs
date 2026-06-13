using GremlinT.Core.Abstractions;
using GremlinT.Core.Attributes;
using JetBrains.Annotations;

namespace GremlinT.Core.Tests;

public class LabelAttributeTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid ElementId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly FullyQualifiedId FqId = new(TenantId, ElementId);

    [Label("human")]
    [UsedImplicitly]
    private class Person : Vertex;

    [Label("drives")]
    [UsedImplicitly]
    private class Owns : Edge;

    [UsedImplicitly]
    private class Car : Vertex;

    [UsedImplicitly]
    private class HasCar : Edge;

    [Fact]
    public void AddV_WithLabelAttribute_UsesAttributeName()
    {
        G.AddV<Person>(TenantId).ToString().Is($"g.addV('human').property('tenantId','{TenantId}')");
    }

    [Fact]
    public void V_WithLabelAttribute_UsesAttributeNameInHasLabel()
    {
        G.V<Person>(TenantId).ToString().Is($"g.V().has('tenantId','{TenantId}').hasLabel('human')");
    }

    [Fact]
    public void AddV_WithFqIdAndLabelAttribute_UsesAttributeName()
    {
        G.AddV<Person>(FqId).ToString()
            .Is($"g.addV(T.label,'human',T.id,'{ElementId}').property('tenantId','{TenantId}')");
    }

    [Fact]
    public void Out_WithLabelAttribute_UsesAttributeName()
    {
        G.V<Person>(TenantId).Out<Owns>().ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('human').out('drives')");
    }

    [Fact]
    public void In_WithLabelAttribute_UsesAttributeName()
    {
        G.V<Person>(TenantId).In<Owns>().ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('human').in('drives')");
    }

    [Fact]
    public void OutE_WithLabelAttribute_UsesAttributeName()
    {
        G.V<Person>(TenantId).OutE<Owns>().ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('human').outE('drives')");
    }

    [Fact]
    public void InE_WithLabelAttribute_UsesAttributeName()
    {
        G.V<Person>(TenantId).InE<Owns>().ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('human').inE('drives')");
    }

    [Fact]
    public void E_WithLabelAttribute_UsesAttributeName()
    {
        G.E<Owns>(TenantId).ToString()
            .Is($"g.E().has('tenantId','{TenantId}').hasLabel('drives')");
    }

    [Fact]
    public void AddV_WithoutLabelAttribute_UsesClassName()
    {
        G.AddV<Car>(TenantId).ToString().Is($"g.addV('Car').property('tenantId','{TenantId}')");
    }

    [Fact]
    public void Out_WithoutLabelAttribute_UsesClassName()
    {
        G.V<Car>(TenantId).Out<HasCar>().ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Car').out('HasCar')");
    }
}
