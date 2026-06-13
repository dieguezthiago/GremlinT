using GremlinT.Core.Abstractions;
using JetBrains.Annotations;

namespace GremlinT.Core.Tests;

public class GraphTraversalTests
{
    private readonly Guid _tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [UsedImplicitly]
    private class Person : IVertex { public Guid Id { get; set; } }

    private enum Status
    {
        Active,
        Inactive
    }

    [Fact]
    public void HasId_WithStringId_AppendsHasIdStep()
    {
        G.AnonV.HasId("abc-123").ToString().Is("__.hasId('abc-123')");
    }

    [Fact]
    public void HasId_WithGuid_FormatsGuidAsLowercaseString()
    {
        var id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        G.AnonV.HasId(id).ToString().Is("__.hasId('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa')");
    }

    [Fact]
    public void Has_WithStringValue_QuotesBothKeyAndValue()
    {
        G.AnonV.Has("name", "Alice").ToString().Is("__.has('name','Alice')");
    }

    [Fact]
    public void Has_WithTrue_SerializesAsQuotedOne()
    {
        G.AnonV.Has("active", true).ToString().Is("__.has('active','1')");
    }

    [Fact]
    public void Has_WithFalse_SerializesAsQuotedZero()
    {
        G.AnonV.Has("active", false).ToString().Is("__.has('active','0')");
    }

    [Fact]
    public void Has_WithInt_AppendsQuotedNumber()
    {
        G.AnonV.Has("age", 42).ToString().Is("__.has('age','42')");
    }

    [Fact]
    public void Has_WithLong_AppendsQuotedNumber()
    {
        G.AnonV.Has("score", 9_999_999_999L).ToString().Is("__.has('score','9999999999')");
    }

    [Fact]
    public void Has_WithEnum_AppendsEnumName()
    {
        G.AnonV.Has("status", Status.Active).ToString().Is("__.has('status','Active')");
    }

    [Fact]
    public void HasLabel_WithSingleLabel_QuotesLabel()
    {
        G.AnonV.HasLabel("Person").ToString().Is("__.hasLabel('Person')");
    }

    [Fact]
    public void HasLabel_WithMultipleLabels_JoinsQuotedLabels()
    {
        G.AnonV.HasLabel("Person", "Employee").ToString().Is("__.hasLabel('Person','Employee')");
    }

    [Fact]
    public void HasLabel_WithGenericType_UsesTypeName()
    {
        G.AnonV.HasLabel<Person>().ToString().Is("__.hasLabel('Person')");
    }

    [Fact]
    public void Where_WithSubTraversal_WrapsInWhereStep()
    {
        var sub = G.AnonV.HasId("x");
        G.AnonV.Where(sub).ToString().Is("__.where(__.hasId('x'))");
    }

    [Fact]
    public void Count_AppendsCountStep()
    {
        G.AnonV.Count().ToString().Is("__.count()");
    }

    [Fact]
    public void Fold_AppendsFoldStep()
    {
        G.AnonV.Fold().ToString().Is("__.fold()");
    }

    [Fact]
    public void Id_AppendsIdStep()
    {
        G.AnonV.Id().ToString().Is("__.id()");
    }

    [Fact]
    public void Value_AppendsValueStep()
    {
        G.AnonV.Value().ToString().Is("__.value()");
    }

    [Fact]
    public void Limit_AppendsUnquotedNumber()
    {
        G.AnonV.Limit(10).ToString().Is("__.limit(10)");
    }

    [Fact]
    public void As_WithAlias_QuotesAlias()
    {
        G.AnonV.As("v1").ToString().Is("__.as('v1')");
    }

    [Fact]
    public void Values_WithSingleKey_QuotesKey()
    {
        G.AnonV.Values("name").ToString().Is("__.values('name')");
    }

    [Fact]
    public void Values_WithMultipleKeys_JoinsQuotedKeys()
    {
        G.AnonV.Values("name", "age").ToString().Is("__.values('name','age')");
    }

    [Fact]
    public void Constant_WithString_QuotesValue()
    {
        G.AnonV.Constant("unknown").ToString().Is("__.constant('unknown')");
    }

    [Fact]
    public void Union_WithMultipleTraversals_JoinsThemUnquoted()
    {
        var t1 = G.AnonV.HasLabel("A");
        var t2 = G.AnonV.HasLabel("B");
        G.AnonV.Union(t1, t2).ToString().Is("__.union(__.hasLabel('A'),__.hasLabel('B'))");
    }

    [Fact]
    public void Coalesce_WithMultipleTraversals_JoinsThemUnquoted()
    {
        GraphTraversal[] traversals = [G.AnonV.HasLabel("A"), G.AnonV.HasLabel("B")];
        G.AnonV.Coalesce(traversals).ToString().Is("__.coalesce(__.hasLabel('A'),__.hasLabel('B'))");
    }

    [Fact]
    public void OrderByAscending_AppendsAscKeyword()
    {
        G.AnonV.OrderByAscending("name").ToString().Is("__.order().by('name',asc)");
    }

    [Fact]
    public void OrderByDescending_AppendsDescKeyword()
    {
        G.AnonV.OrderByDescending("name").ToString().Is("__.order().by('name',desc)");
    }

    [Fact]
    public void IsNotEqual_AppendsNeqExpression()
    {
        G.AnonV.IsNotEqual(0L).ToString().Is("__.is(neq(0))");
    }

    [Fact]
    public void ToString_ReturnsAccumulatedSteps()
    {
        G.AnonV.HasId("x").Has("name", "Alice").ToString().Is("__.hasId('x').has('name','Alice')");
    }

    [Fact]
    public void ImplicitStringCast_ReturnsFullQuery()
    {
        string result = G.AnonV.HasId("x");
        result.Is("__.hasId('x')");
    }

    [Fact]
    public void Property_()
    {
        G.AddV<Person>(_tenantId).Property("Name", "Thiago")
            .ToString()
            .Is($"g.addV('Person').property('tenantId','{_tenantId}').property('Name','Thiago')");
    }
}
