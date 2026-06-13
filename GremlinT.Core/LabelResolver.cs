using System.Reflection;
using GremlinT.Core.Attributes;

namespace GremlinT.Core;

internal static class LabelResolver
{
    internal static string For<T>() => For(typeof(T));

    internal static string For(Type type)
    {
        return type.GetCustomAttribute<LabelAttribute>()?.Value ?? type.Name;
    }
}
