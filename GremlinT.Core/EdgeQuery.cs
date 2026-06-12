using System.Text;

namespace GremlinT.Core;

public class EdgeQuery : GraphTraversal<EdgeQuery>
{
    internal EdgeQuery(StringBuilder sb) : base(sb)
    {
    }

    public VertexQuery OutV()
    {
        Write(".outV()");
        return new VertexQuery(Sb);
    }

    public VertexQuery InV()
    {
        Write(".inV()");
        return new VertexQuery(Sb);
    }

    public VertexQuery OtherV()
    {
        Write(".otherV()");
        return new VertexQuery(Sb);
    }

    public EdgeQuery From(string alias)
    {
        return Write($".from('{alias}')");
    }

    public EdgeQuery To(string alias)
    {
        return Write($".to('{alias}')");
    }

    public EdgeQuery Property(string key, string value)
    {
        return Write($".property('{key}','{value}')");
    }
}
