namespace Common.Messaging;

public class MessageMetadataAccessor : IMessageMetadataAccessor
{
    private static readonly IEnumerable<string> EmptyTags = Array.Empty<string>();
    private const string UnknownAuthenticatedUser = "UNKNOWN USER";

    private static readonly EmptyMessageMetadata Empty = new();

    private MessageMetadata? _messageMetadata;

    public MessageMetadata MessageMetadata
    {
        get => _messageMetadata ?? Empty;
        set => _messageMetadata = value;
    }

    private sealed record EmptyMessageMetadata() : MessageMetadata(
        EmptyTags,
        UnknownAuthenticatedUser
    );
}