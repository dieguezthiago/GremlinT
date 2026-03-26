using System.Linq.Expressions;
using System.Text;

namespace TypedGremlin.Core;

public abstract class VertexQueryBase<TSelf>(StringBuilder sb) : GraphTraversal<TSelf>(sb)
    where TSelf : VertexQueryBase<TSelf>
{
    public TSelf Out(string label)
    {
        return Write($".out('{label}')");
    }

    public VertexQuery Out<TEdge>()
        where TEdge : Edge
    {
        Out(typeof(TEdge).Name);
        return new VertexQuery(Sb);
    }

    public TSelf In(string label)
    {
        return Write($".in('{label}')");
    }

    public VertexQuery In<TEdge>()
        where TEdge : Edge
    {
        In(typeof(TEdge).Name);
        return new VertexQuery(Sb);
    }

    public EdgeQuery OutE(string label)
    {
        Write($".outE('{label}')");
        return new EdgeQuery(Sb);
    }

    public EdgeQuery InE(string label)
    {
        Write($".inE('{label}')");
        return new EdgeQuery(Sb);
    }

    public TSelf AddV<TLabel>()
        where TLabel : Vertex
    {
        return Write($".addV('{typeof(TLabel).Name}')");
    }

    public EdgeQuery AddE(string label)
    {
        Write($".addE('{label}')");
        return new EdgeQuery(Sb);
    }

    public TSelf Properties(params string[] keys)
    {
        return Write($".properties({string.Join(",", keys.Select(k => $"'{k}'"))})");
    }

    public TSelf ValueMap(params string[] keys)
    {
        return ValueMap(false, keys);
    }

    public TSelf ValueMap(bool includeSystemFields = false, params string[] keys)
    {
        var parts = includeSystemFields
            ? ["true", ..keys.Select(k => $"'{k}'")]
            : keys.Select(k => $"'{k}'");
        return Write($".valueMap({string.Join(",", parts)})");
    }

    public VertexProjection<TResult> Project<TResult>(params Expression<Func<TResult, object>>[] selectors)
    {
        var keys = Array.ConvertAll(selectors, ExpressionHelper.MemberName);
        Write($".project({string.Join(",", Array.ConvertAll(keys, k => $"'{k}'"))})");
        return new VertexProjection<TResult>(Sb);
    }
}