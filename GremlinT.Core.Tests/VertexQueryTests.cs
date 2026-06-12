using JetBrains.Annotations;

namespace GremlinT.Core.Tests;

public class VertexQueryTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [UsedImplicitly]
    private class Person : Vertex
    {
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
    }

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
    public void Out_WithLabel_AppendsOutStep()
    {
        G.AnonV.Out("knows").ToString().Is("__.out('knows')");
    }

    [Fact]
    public void Out_WithLabel_ReturnsSameVertexQueryType()
    {
        G.AnonV.Out("knows").IsInstanceOf<VertexQuery>();
    }

    [Fact]
    public void In_WithLabel_AppendsInStep()
    {
        G.AnonV.In("knows").ToString().Is("__.in('knows')");
    }

    [Fact]
    public void In_WithLabel_ReturnsSameVertexQueryType()
    {
        G.AnonV.In("knows").IsInstanceOf<VertexQuery>();
    }

    [Fact]
    public void OutE_WithLabel_AppendsOutEStep()
    {
        G.AnonV.OutE("knows").ToString().Is("__.outE('knows')");
    }

    [Fact]
    public void OutE_WithLabel_ReturnsEdgeQuery()
    {
        G.AnonV.OutE("knows").IsInstanceOf<EdgeQuery>();
    }

    [Fact]
    public void InE_WithLabel_AppendsInEStep()
    {
        G.AnonV.InE("knows").ToString().Is("__.inE('knows')");
    }

    [Fact]
    public void InE_WithLabel_ReturnsEdgeQuery()
    {
        G.AnonV.InE("knows").IsInstanceOf<EdgeQuery>();
    }

    [Fact]
    public void AddV_WithGenericType_UsesTypeName()
    {
        G.AnonV.AddV<Person>().ToString().Is("__.addV('Person')");
    }

    [Fact]
    public void AddE_WithLabel_AppendsAddEStep()
    {
        G.AnonV.AddE("knows").ToString().Is("__.addE('knows')");
    }

    [Fact]
    public void AddE_WithLabel_ReturnsEdgeQuery()
    {
        G.AnonV.AddE("knows").IsInstanceOf<EdgeQuery>();
    }

    [Fact]
    public void Properties_WithNoKeys_EmitsEmptyProperties()
    {
        G.AnonV.Properties().ToString().Is("__.properties()");
    }

    [Fact]
    public void Properties_WithSingleKey_QuotesKey()
    {
        G.AnonV.Properties("name").ToString().Is("__.properties('name')");
    }

    [Fact]
    public void Properties_WithMultipleKeys_JoinsQuotedKeys()
    {
        G.AnonV.Properties("name", "age").ToString().Is("__.properties('name','age')");
    }

    [Fact]
    public void ValueMap_WithNoKeys_EmitsEmptyValueMap()
    {
        G.AnonV.ValueMap().ToString().Is("__.valueMap()");
    }

    [Fact]
    public void ValueMap_WithKeys_QuotesAllKeys()
    {
        G.AnonV.ValueMap("name", "age").ToString().Is("__.valueMap('name','age')");
    }

    [Fact]
    public void ValueMap_WithIncludeSystemFields_PrependsTrueFlag()
    {
        G.AnonV.ValueMap(true, "name").ToString().Is("__.valueMap(true,'name')");
    }

    [Fact]
    public void Project_WithSingleSelector_AppendsProjectStep()
    {
        G.AnonV.Project<PersonView>(x => x.Name).ToString().Is("__.project('Name')");
    }

    [Fact]
    public void Project_WithMultipleSelectors_JoinsAllKeys()
    {
        G.AnonV.Project<PersonView>(x => x.Name, x => x.Age).ToString().Is("__.project('Name','Age')");
    }

    [Fact]
    public void Chain_OutE_InV_HasLabel_ProducesCorrectFullQuery()
    {
        G.V(TenantId).OutE("knows").InV().HasLabel<Person>()
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').outE('knows').inV().hasLabel('Person')");
    }

    [Fact]
    public void Chain_AddE_From_To_Property_ProducesCorrectFullQuery()
    {
        G.V(TenantId).AddE("knows").From("v1").To("v2").Property("since", "2020")
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').addE('knows').from('v1').to('v2').property('since','2020')");
    }

    [Fact]
    public void V_WithVertexType_AppendsHasLabelAndReturnsTyped()
    {
        G.V<Person>(TenantId).Out<Owns>().V<Car>()
            .ToString()
            .Is($"g.V().has('tenantId','{TenantId}').hasLabel('Person').out('Owns').hasLabel('Car')");
    }

    [Fact]
    public void V_WithVertexType_ReturnsTypedVertexQuery()
    {
        G.V<Person>(TenantId).Out<Owns>().V<Car>().IsInstanceOf<VertexQuery<Car>>();
    }

    [Fact]
    public void Property_()
    {
        G.AddV<Person>(TenantId)
            .Property(x => x.Name, "Thiago")
            .Property(x => x.Surname, "Dieguez")
            .ToString()
            .Is(
                $"g.addV('Person').property('tenantId','{TenantId}').property('Name','Thiago').property('Surname','Dieguez')"
            );
    }
}
