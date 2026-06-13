using GremlinT.Core.Abstractions;
using JetBrains.Annotations;

namespace GremlinT.Core.Tests;

public class GTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid ElementId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly FullyQualifiedId FqId = new(TenantId, ElementId);

    [UsedImplicitly]
    private class Person : IVertex { public Guid Id { get; set; } }

    [UsedImplicitly]
    private class Knows : IEdge { public Guid Id { get; set; } }

    [Fact]
    public void V_WithTenantId_StartsWithGvAndHasTenantId()
    {
        G.V(TenantId).ToString().Is($"g.V().has('tenantId','{TenantId}')");
    }

    [Fact]
    public void V_WithGenericType_IncludesHasLabel()
    {
        G.V<Person>(TenantId).ToString().Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person')");
    }

    [Fact]
    public void V_WithFullyQualifiedId_IncludesElementIdAndTenantId()
    {
        G.V(FqId).ToString().Is($"g.V('{ElementId}').has('tenantId','{TenantId}')");
    }

    [Fact]
    public void V_WithGenericTypeAndFullyQualifiedId_IncludesBothLabelAndIds()
    {
        G.V<Person>(FqId).ToString().Is($"g.V('{ElementId}').has('tenantId','{TenantId}').hasLabel('Person')");
    }

    [Fact]
    public void E_WithTenantId_StartsWithGeAndHasTenantId()
    {
        G.E(TenantId).ToString().Is($"g.E().has('tenantId','{TenantId}')");
    }

    [Fact]
    public void E_WithGenericType_IncludesHasLabel()
    {
        G.E<Knows>(TenantId).ToString().Is($"g.E().has('tenantId','{TenantId}').hasLabel('Knows')");
    }

    [Fact]
    public void E_WithFullyQualifiedId_IncludesElementIdAndTenantId()
    {
        G.E(FqId).ToString().Is($"g.E('{ElementId}').has('tenantId','{TenantId}')");
    }

    [Fact]
    public void E_WithGenericTypeAndFullyQualifiedId_IncludesBothLabelAndIds()
    {
        G.E<Knows>(FqId).ToString().Is($"g.E('{ElementId}').has('tenantId','{TenantId}').hasLabel('Knows')");
    }

    [Fact]
    public void AnonV_StartsWithDoubleUnderscore()
    {
        G.AnonV.ToString().Is("__");
    }

    [Fact]
    public void AnonE_StartsWithDoubleUnderscore()
    {
        G.AnonE.ToString().Is("__");
    }

    [Fact]
    public void AddV_SetsTenantId()
    {
        G.AddV(TenantId, "Person").ToString().Is($"g.addV('Person').property('tenantId','{TenantId}')");
    }
}
