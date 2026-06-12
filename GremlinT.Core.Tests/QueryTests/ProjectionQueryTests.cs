using JetBrains.Annotations;

namespace GremlinT.Core.Tests.QueryTests;

public class ProjectionQueryTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [UsedImplicitly]
    private class Person : Vertex
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    private class PersonView
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    [Fact]
    public void Project_SingleKey_WithBySubTraversal_ProducesProjection()
    {
        G.V<Person>(TenantId)
            .Project<PersonView>(x => x.Name)
            .By(G.AnonV.Values("Name"))
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').project('Name').by(__.values('Name'))");
    }

    [Fact]
    public void Project_MultipleKeys_WithMultipleBySteps_ProducesFullProjection()
    {
        G.V<Person>(TenantId)
            .Project<PersonView>(x => x.Name, x => x.Age)
            .By(G.AnonV.Values("Name"))
            .By(G.AnonV.Values("Age"))
            .ToString()
            .Is(
                $"g.V().has('tenantId','{TenantId}').hasLabel('Person').project('Name','Age').by(__.values('Name')).by(__.values('Age'))");
    }

    [Fact]
    public void Project_WithBuilderBy_ProducesProjectionUsingAnonTraversal()
    {
        G.V<Person>(TenantId)
            .Project<PersonView>(x => x.Name)
            .By(q => q.Values("Name"))
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').project('Name').by(__.values('Name'))");
    }

    [Fact]
    public void Project_WithBuilderBy_IncludingFilter_ProducesConditionalProjection()
    {
        G.V<Person>(TenantId)
            .Project<PersonView>(x => x.Age)
            .By(q => q.Has("Name", "Alice").Values("Age"))
            .ToString()
            .Is(
                $"g.V().has('tenantId','{TenantId}').hasLabel('Person').project('Age').by(__.has('Name','Alice').values('Age'))");
    }

    [Fact]
    public void ValueMap_WithLambdaSelectors_ProducesTypedValueMap()
    {
        G.V<Person>(TenantId).ValueMap(x => x.Name, x => x.Age)
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').valueMap('Name','Age')");
    }

    [Fact]
    public void ValueMap_WithIncludeSystemFields_PrependsTrueFlag()
    {
        G.V<Person>(TenantId).ValueMap(true, x => x.Name)
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').valueMap(true,'Name')");
    }

    [Fact]
    public void Values_AfterFilterTraversal_SelectsSpecificProperty()
    {
        G.V<Person>(TenantId).Has("Name", "Alice").Values("Name")
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').has('Name','Alice').values('Name')");
    }

    [Fact]
    public void Project_OnUntypedVertex_WithBySubTraversal_ProducesUntypedProjection()
    {
        G.V(TenantId)
            .Project<PersonView>(x => x.Name)
            .By(G.AnonV.Values("Name"))
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').project('Name').by(__.values('Name'))");
    }
}
