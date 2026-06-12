namespace GremlinT.Core.Tests;

public class EdgeQueryTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private class Person { }

    [Fact]
    public void OutV_AppendsOutVStep()
    {
        G.AnonE.OutV().ToString().Is("__.outV()");
    }

    [Fact]
    public void OutV_ReturnsVertexQuery()
    {
        G.AnonE.OutV().IsInstanceOf<VertexQuery>();
    }

    [Fact]
    public void InV_AppendsInVStep()
    {
        G.AnonE.InV().ToString().Is("__.inV()");
    }

    [Fact]
    public void InV_ReturnsVertexQuery()
    {
        G.AnonE.InV().IsInstanceOf<VertexQuery>();
    }

    [Fact]
    public void OtherV_AppendsOtherVStep()
    {
        G.AnonE.OtherV().ToString().Is("__.otherV()");
    }

    [Fact]
    public void OtherV_ReturnsVertexQuery()
    {
        G.AnonE.OtherV().IsInstanceOf<VertexQuery>();
    }

    [Fact]
    public void From_WithAlias_QuotesAlias()
    {
        G.AnonE.From("v1").ToString().Is("__.from('v1')");
    }

    [Fact]
    public void To_WithAlias_QuotesAlias()
    {
        G.AnonE.To("v2").ToString().Is("__.to('v2')");
    }

    [Fact]
    public void Property_WithKeyAndValue_QuotesBothKeyAndValue()
    {
        G.AnonE.Property("weight", "heavy").ToString().Is("__.property('weight','heavy')");
    }

    [Fact]
    public void Chain_FromToPropertyInV_ProducesCorrectFullQuery()
    {
        G.E(TenantId)
            .From("a").To("b").Property("weight", "high")
            .InV().HasLabel<Person>()
            .ToString()
            .Is($"g.E().has('tenantId','{TenantId}').from('a').to('b').property('weight','high').inV().hasLabel('Person')");
    }
}

