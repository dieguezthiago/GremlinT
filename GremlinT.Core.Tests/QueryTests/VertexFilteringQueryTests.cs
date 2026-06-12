using GremlinT.Core.Abstractions;
using JetBrains.Annotations;

namespace GremlinT.Core.Tests.QueryTests;

public class VertexFilteringQueryTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [UsedImplicitly]
    private class Person : Vertex
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    [UsedImplicitly]
    private class Car : Vertex;

    private enum CarStatus
    {
        Available,
        Unavailable
    }

    [Fact]
    public void SingleHas_OnTypedVertex_FiltersProperty()
    {
        G.V<Person>(TenantId).Has("Name", "Alice")
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').has('Name','Alice')");
    }

    [Fact]
    public void MultipleHas_OnTypedVertex_ChainsAllFilters()
    {
        G.V<Person>(TenantId).Has("Name", "Alice").Has("Age", 30)
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').has('Name','Alice').has('Age','30')");
    }

    [Fact]
    public void HasId_OnTypedVertex_FiltersById()
    {
        var id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        G.V<Person>(TenantId).HasId(id)
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').hasId('{id}')");
    }

    [Fact]
    public void HasLabel_WithMultipleLabels_OnUntypedVertex_FiltersAnyMatchingLabel()
    {
        G.V(TenantId).HasLabel("Person", "Employee")
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person','Employee')");
    }

    [Fact]
    public void Where_WithSubTraversal_OnTypedVertex_WrapsFilter()
    {
        G.V<Person>(TenantId).Where(q => q.Has("Name", "Alice"))
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').where(__.has('Name','Alice'))");
    }

    [Fact]
    public void Has_AndWhere_OnTypedVertex_CombinesFilters()
    {
        G.V<Person>(TenantId)
            .Has("Name", "Alice")
            .Where(q => q.Has("Age", 30))
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').has('Name','Alice').where(__.has('Age','30'))");
    }

    [Fact]
    public void Has_WithBoolValue_OnTypedVertex_SerializesAsZeroOrOne()
    {
        G.V<Person>(TenantId).Has("IsActive", true)
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').has('IsActive','1')");
    }

    [Fact]
    public void Has_WithEnumValue_OnTypedVertex_SerializesEnumName()
    {
        G.V<Car>(TenantId).Has("Status", CarStatus.Available)
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Car').has('Status','Available')");
    }
}
