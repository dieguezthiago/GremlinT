using System.Text;

namespace TypedGremlin.Core;

public class VertexQuery : VertexQueryBase<VertexQuery>
{
	internal VertexQuery(StringBuilder sb) : base(sb)
	{
	}
}
