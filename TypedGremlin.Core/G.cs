using System.Text;

namespace TypedGremlin.Core;

public static class G
{
    public static VertexQuery AnonV
        => new(new StringBuilder("__"));

    public static EdgeQuery AnonE
        => new(new StringBuilder("__"));

    public static VertexQuery V(Guid tenantId)
    {
        return new VertexQuery(new StringBuilder(
            $"g.V().has('tenantId','{tenantId}')"
        ));
    }

    public static VertexQuery<T> V<T>(Guid tenantId)
    {
        return new VertexQuery<T>(new StringBuilder(
            $"g.V().hasLabel('{typeof(T).Name}').has('tenantId','{tenantId}')"
        ));
    }

    public static VertexQuery V(FullyQualifiedId fullyQualifiedId)
    {
        return new VertexQuery(new StringBuilder(
            $"g.V('{fullyQualifiedId.ElementId}').has('tenantId','{fullyQualifiedId.TenantId}')"
        ));
    }

    public static VertexQuery<T> V<T>(FullyQualifiedId fullyQualifiedId)
    {
        return new VertexQuery<T>(new StringBuilder(
            $"g.V('{fullyQualifiedId.ElementId}').hasLabel('{typeof(T).Name}').has('tenantId','{fullyQualifiedId.TenantId}')"
        ));
    }

    public static EdgeQuery E(Guid tenantId)
    {
        return new EdgeQuery(new StringBuilder(
            $"g.E().has('tenantId','{tenantId}')"
        ));
    }

    public static EdgeQuery E<T>(Guid tenantId)
    {
        return new EdgeQuery(new StringBuilder(
            $"g.E().hasLabel('{typeof(T).Name}').has('tenantId','{tenantId}')"
        ));
    }

    public static EdgeQuery E(FullyQualifiedId fullyQualifiedId)
    {
        return new EdgeQuery(new StringBuilder(
            $"g.E('{fullyQualifiedId.ElementId}').has('tenantId','{fullyQualifiedId.TenantId}')"
        ));
    }

    public static EdgeQuery E<T>(FullyQualifiedId fullyQualifiedId)
    {
        return new EdgeQuery(new StringBuilder(
            $"g.E('{fullyQualifiedId.ElementId}').hasLabel('{typeof(T).Name}').has('tenantId','{fullyQualifiedId.TenantId}')"
        ));
    }
}