using JetBrains.Annotations;

namespace TypedGremlin.Core.Tests.QueryTests;

public class AggregationQueryTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [UsedImplicitly]
    private class Person : Vertex
    {
        public string Name { get; set; } = "";
    }

    [UsedImplicitly]
    private class Knows : Edge;

    [UsedImplicitly]
    private class Car : Vertex;

    [UsedImplicitly]
    private class Has : Edge;

    [Fact]
    public void Count_OnTypedVertex_ProducesCountStep()
    {
        G.V<Person>(TenantId).Count()
            .ToString()
            .Is($"g.V().hasLabel('Person').has('tenantId','{TenantId}').count()");
    }

    [Fact]
    public void Fold_OnTypedVertex_ProducesFoldStep()
    {
        G.V<Person>(TenantId).Fold()
            .ToString()
            .Is($"g.V().hasLabel('Person').has('tenantId','{TenantId}').fold()");
    }

    [Fact]
    public void Limit_OnTypedVertex_ProducesLimitStep()
    {
        G.V<Person>(TenantId).Limit(10)
            .ToString()
            .Is($"g.V().hasLabel('Person').has('tenantId','{TenantId}').limit(10)");
    }

    [Fact]
    public void OrderByAscending_OnTypedVertexWithString_ProducesAscOrder()
    {
        G.V<Person>(TenantId).OrderByAscending("Name")
            .ToString()
            .Is($"g.V().hasLabel('Person').has('tenantId','{TenantId}').order().by('Name',asc)");
    }

    [Fact]
    public void OrderByAscending_OnTypedVertexWithExpression_ProducesAscOrder()
    {
        G.V<Person>(TenantId).OrderByAscending(p => p.Name)
            .ToString()
            .Is($"g.V().hasLabel('Person').has('tenantId','{TenantId}').order().by('Name',asc)");
    }

    [Fact]
    public void OrderByDescending_OnTypedVertexWithString_ProducesDescOrder()
    {
        G.V<Person>(TenantId).OrderByDescending("Name")
            .ToString()
            .Is($"g.V().hasLabel('Person').has('tenantId','{TenantId}').order().by('Name',desc)");
    }

    [Fact]
    public void OrderByDescending_OnTypedVertexWithExpression_ProducesDescOrder()
    {
        G.V<Person>(TenantId).OrderByDescending(x => x.Name)
            .ToString()
            .Is($"g.V().hasLabel('Person').has('tenantId','{TenantId}').order().by('Name',desc)");
    }

    [Fact]
    public void Union_OfTwoLabelTraversals_ProducesUnionStep()
    {
        G.V(TenantId)
            .Union(G.AnonV.HasLabel("Person"), G.AnonV.HasLabel("Car"))
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').union(__.hasLabel('Person'),__.hasLabel('Car'))");
    }

    [Fact]
    public void Union_OfTypedVertexOfTwoTraversals_ProducesUnionStep()
    {
        G.V<Person>(TenantId)
            // .Union(
            //     b => b.Out<Knows>().V<Person>(),
            //     b => b.Out<Has>().V<Car>()
            // )
            .ToString()
            .Is($"""
                 g.V().hasLabel('Person')
                 .has('tenantId','{TenantId}')
                 .union(
                 __.out('Knows').hasLabel('Person'),
                 __.out('Has').hasLabel('Car')
                 )
                 """.Replace("\n", ""));
    }

    [Fact]
    public void Coalesce_OfFilterAndConstant_ProducesCoalesceStep()
    {
        GraphTraversal[] traversals = [G.AnonV.Has("Name", "Alice"), G.AnonV.Constant("unknown")];
        G.V<Person>(TenantId)
            .Coalesce(traversals)
            .ToString()
            .Is(
                $"g.V().hasLabel('Person').has('tenantId','{TenantId}').coalesce(__.has('Name','Alice'),__.constant('unknown'))");
    }

    [Fact]
    public void Count_AfterMultiHopTraversal_CountsResultVertices()
    {
        G.V<Person>(TenantId).Out<Has>().V<Car>().Count()
            .ToString()
            .Is($"g.V().hasLabel('Person').has('tenantId','{TenantId}').out('Has').hasLabel('Car').count()");
    }

    [Fact]
    public void IsNotEqual_AfterCount_ProducesCountWithNeqFilter()
    {
        G.V<Person>(TenantId).Count().IsNotEqual(0)
            .ToString()
            .Is($"g.V().hasLabel('Person').has('tenantId','{TenantId}').count().is(neq(0))");
    }
}