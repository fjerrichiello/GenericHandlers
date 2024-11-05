namespace Common.Messaging;

public abstract record MessageMetadata(IEnumerable<string> Tags, string AuthenticatedUser)
{
    protected MessageMetadata() : this([], string.Empty)
    {
    }
};