using System.Text;

namespace GremlinT.Core;

public static class G
{
    public static VertexQuery AnonV
        => new(new StringBuilder("__"));

    public static EdgeQuery AnonE
        => new(new StringBuilder("__"));

    public static VertexQuery V(Guid tenantId)
    {
        var vertexQuery = new VertexQuery(new StringBuilder(
            $"g.V()"
        )).HasTenantId(tenantId);

        return vertexQuery;
    }

    public static VertexQuery<T> V<T>(Guid tenantId)
        where T : Vertex
    {
        return V(tenantId).V<T>();
    }

    public static VertexQuery V(FullyQualifiedId fullyQualifiedId)
    {
        return new VertexQuery(new StringBuilder($"g.V('{fullyQualifiedId.ElementId}')"))
            .HasTenantId(fullyQualifiedId.TenantId);
    }

    public static VertexQuery<T> V<T>(FullyQualifiedId fullyQualifiedId)
        where T : Vertex
    {
        return V(fullyQualifiedId).V<T>();
    }

    public static EdgeQuery E(Guid tenantId)
    {
        return new EdgeQuery(new StringBuilder("g.E()")).HasTenantId(tenantId);
    }

    public static EdgeQuery E<T>(Guid tenantId)
        where T : Edge
    {
        return E(tenantId).HasLabel(typeof(T).Name);
    }

    public static EdgeQuery E(FullyQualifiedId fullyQualifiedId)
    {
        return new EdgeQuery(new StringBuilder($"g.E('{fullyQualifiedId.ElementId}')"))
            .HasTenantId(fullyQualifiedId.TenantId);
    }

    public static EdgeQuery E<T>(FullyQualifiedId fullyQualifiedId)
        where T : Edge
    {
        return E(fullyQualifiedId).HasLabel(typeof(T).Name);
    }

    public static VertexQuery AddV(Guid tenantId, string label)
    {
        return new VertexQuery(new StringBuilder(
            $"g.addV('{label}')"
        )).WithTenantId(tenantId);
    }

    public static VertexQuery<T> AddV<T>(Guid tenantId) where T : Vertex
    {
        return new VertexQuery<T>(new StringBuilder(
            $"g.addV('{typeof(T).Name}')"
        )).WithTenantId(tenantId);
    }

    public static VertexQuery AddV(FullyQualifiedId fullyQualifiedId, string label)
    {
        return new VertexQuery(new StringBuilder(
            $"g.addV(T.label,'{label}',T.id,'{fullyQualifiedId.ElementId}')"
        )).WithTenantId(fullyQualifiedId.TenantId);
    }

    public static VertexQuery AddV<T>(FullyQualifiedId fullyQualifiedId)
    {
        return AddV(fullyQualifiedId, typeof(T).Name);
    }
}
