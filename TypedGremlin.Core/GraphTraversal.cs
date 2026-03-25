using System.Text;

namespace TypedGremlin.Core;

public abstract class GraphTraversal(StringBuilder sb)
{
	internal readonly StringBuilder Sb = sb;

	public override string ToString()
	{
		return Sb.ToString();
	}

	public static implicit operator string(GraphTraversal q)
	{
		return q.ToString();
	}
}