using System.Text;
using GremlinT.Core.Abstractions;

namespace GremlinT.Core;

public class VertexQuery : VertexQueryBase<VertexQuery>
{
	internal VertexQuery(StringBuilder sb) : base(sb)
	{
	}

	public VertexQuery<TVertex> V<TVertex>()
		where TVertex : Vertex
	{
		HasLabel(typeof(TVertex).Name);
		return new VertexQuery<TVertex>(Sb);
	}
}

