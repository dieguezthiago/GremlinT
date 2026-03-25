using System.Text;

namespace TypedGremlin.Core;

public class VertexProjection<T> : GraphTraversal
{
    internal VertexProjection(StringBuilder sb) : base(sb)
    {
    }

    public VertexProjection<T> By(GraphTraversal subTraversal)
    {
        Sb.Append($".by({subTraversal})");
        return this;
    }
}