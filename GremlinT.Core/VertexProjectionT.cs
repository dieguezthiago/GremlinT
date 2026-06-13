using System.Text;
using GremlinT.Core.Abstractions;

namespace GremlinT.Core;

public class VertexProjection<TVertex, TResult> : VertexProjection<TResult>
    where TVertex : IVertex
{
    internal VertexProjection(StringBuilder sb) : base(sb)
    {
    }

    public new VertexProjection<TVertex, TResult> By(GraphTraversal subTraversal)
    {
        base.By(subTraversal);
        return this;
    }

    public VertexProjection<TVertex, TResult> By<TTraversal>(
        Func<VertexQuery<TVertex>, TTraversal> builder
    ) where TTraversal : GraphTraversal
    {
        return By(builder(new VertexQuery<TVertex>(new StringBuilder("__"))));
    }
}
