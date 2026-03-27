using System.Linq.Expressions;
using System.Text;

namespace TypedGremlin.Core;

public class VertexQuery<T> : VertexQueryBase<VertexQuery<T>>
    where T : Vertex
{
    internal VertexQuery(StringBuilder sb) : base(sb)
    {
    }

    public VertexQuery<TTarget> In<TTarget>(string label)
        where TTarget : Vertex
    {
        Sb.Append($".in('{label}')");
        return new VertexQuery<TTarget>(Sb);
    }

    public VertexQuery<TTarget> Out<TTarget>(string label)
        where TTarget : Vertex
    {
        Sb.Append($".out('{label}')");
        return new VertexQuery<TTarget>(Sb);
    }

    public VertexQuery<T> Properties(params Expression<Func<T, object?>>[] selectors)
    {
        base.Properties(Array.ConvertAll(selectors, ExpressionHelper.MemberName));
        return this;
    }

    public VertexQuery<T> ValueMap(params Expression<Func<T, object?>>[] selectors)
    {
        base.ValueMap(false, Array.ConvertAll(selectors, ExpressionHelper.MemberName));
        return this;
    }

    public VertexQuery<T> ValueMap(bool includeSystemFields, params Expression<Func<T, object?>>[] selectors)
    {
        base.ValueMap(includeSystemFields, Array.ConvertAll(selectors, ExpressionHelper.MemberName));
        return this;
    }

    public VertexQuery<T> Where<TTraversal>(Func<VertexQuery<T>, TTraversal> builder)
        where TTraversal : GraphTraversal
    {
        return Where(builder(new VertexQuery<T>(new StringBuilder("__"))));
    }

    public new VertexProjection<T, TResult> Project<TResult>(params Expression<Func<TResult, object>>[] selectors)
    {
        var keys = Array.ConvertAll(selectors, ExpressionHelper.MemberName);
        Sb.Append($".project({string.Join(",", Array.ConvertAll(keys, k => $"'{k}'"))})");
        return new VertexProjection<T, TResult>(Sb);
    }

    public VertexQuery<T> OrderByAscending(Expression<Func<T, object>> expression)
    {
        var key = ExpressionHelper.MemberName(expression);
        OrderByAscending(key);
        return this;
    }

    public VertexQuery<T> OrderByDescending(Expression<Func<T, object>> expression)
    {
        var key = ExpressionHelper.MemberName(expression);
        OrderByDescending(key);
        return this;
    }
}