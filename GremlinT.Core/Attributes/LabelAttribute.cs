namespace GremlinT.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class LabelAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}
