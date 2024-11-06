namespace Common.Structured.Messaging;

[AttributeUsage(validOn: AttributeTargets.Class, Inherited = false)]
public class MessageTagsAttribute(params string[] _tags) : Attribute
{
    public IEnumerable<string> Tags => _tags;
}