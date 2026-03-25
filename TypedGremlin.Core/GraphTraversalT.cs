using System.Text;

namespace TypedGremlin.Core;

public abstract class GraphTraversal<TSelf>(
    StringBuilder sb
) : GraphTraversal(sb) where TSelf : GraphTraversal<TSelf>
{
    protected TSelf Write(string step)
    {
        Sb.Append(step);
        return (TSelf)this;
    }

    public TSelf HasId(string id)
    {
        return Write($".hasId('{id}')");
    }

    public TSelf HasId(Guid id)
    {
        return HasId(id.ToString());
    }

    public TSelf Has(string key, string value)
    {
        return Write($".has('{key}','{value}')");
    }

    public TSelf Has(string key, bool value)
    {
        return Has(key, value ? 1 : 0);
    }

    public TSelf Has(string key, int value)
    {
        return Has(key, value.ToString());
    }

    public TSelf Has(string key, long value)
    {
        return Has(key, value.ToString());
    }

    public TSelf Has<TEnum>(string key, TEnum value) where TEnum : Enum
    {
        return Has(key, value.ToString());
    }

    public TSelf HasLabel(params string[] labels)
    {
        return Write($".hasLabel({string.Join(",", labels.Select(l => $"'{l}'"))})");
    }

    public TSelf HasLabel<T>()
    {
        return HasLabel(typeof(T).Name);
    }

    public TSelf Where(GraphTraversal subTraversal)
    {
        return Write($".where({subTraversal})");
    }

    public TSelf IsNotEqual(long value)
    {
        return Write($".is(neq({value}))");
    }

    public TSelf Count()
    {
        return Write(".count()");
    }

    public TSelf Fold()
    {
        return Write(".fold()");
    }

    public TSelf Id()
    {
        return Write(".id()");
    }

    public TSelf Limit(long n)
    {
        return Write($".limit({n})");
    }

    public TSelf As(string alias)
    {
        return Write($".as('{alias}')");
    }

    public TSelf Values(params string[] keys)
    {
        return Write($".values({string.Join(",", keys.Select(k => $"'{k}'"))})");
    }

    public TSelf Value()
    {
        return Write(".value()");
    }

    public TSelf Constant(string value)
    {
        return Write($".constant('{value}')");
    }

    public TSelf Union(params GraphTraversal[] traversals)
    {
        return Write($".union({string.Join(",", traversals.Select(t => t.ToString()))})");
    }

    public TSelf Coalesce(IEnumerable<GraphTraversal> traversals)
    {
        return Write($".coalesce({string.Join(",", traversals.Select(t => t.ToString()))})");
    }

    public TSelf OrderByAscending(string key)
    {
        return Write($".order().by('{key}',asc)");
    }

    public TSelf OrderByDescending(string key)
    {
        return Write($".order().by('{key}',desc)");
    }
}