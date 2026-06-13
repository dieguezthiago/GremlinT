using System.Text;
using GremlinT.Core.Abstractions;

namespace GremlinT.Core;

public class VertexQuery : VertexQueryBase<VertexQuery>
{
	internal VertexQuery(StringBuilder sb) : base(sb)
	{
	}

	public VertexQuery<TVertex> V<TVertex>()
		where TVertex : IVertex
	{
		HasLabel(LabelResolver.For<TVertex>());
		return new VertexQuery<TVertex>(Sb);
	}
}

