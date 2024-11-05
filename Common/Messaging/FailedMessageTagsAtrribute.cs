namespace Common.Messaging;

[AttributeUsage(validOn: AttributeTargets.Class, Inherited = false)]
public class FailedMessageTagsAttribute(params string[] _tags) : Attribute
{
    public IEnumerable<string> Tags => _tags;
}