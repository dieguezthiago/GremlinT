using System.Linq.Expressions;

namespace GremlinT.Core;

internal static class ExpressionHelper
{
    internal static string MemberName<T, TProp>(Expression<Func<T, TProp>> expr)
    {
        var body = expr.Body;
        if (body is UnaryExpression { NodeType: ExpressionType.Convert } unary)
        {
            body = unary.Operand;
        }

        if (body is MemberExpression member)
        {
            return member.Member.Name;
        }

        throw new ArgumentException(
            $"Expression must be a simple property access (e.g. x => x.Title), got: {expr}"
        );
    }
}
