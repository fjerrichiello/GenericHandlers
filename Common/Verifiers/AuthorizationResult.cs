namespace Common.Verifiers;

public class AuthorizationResult
{
    public bool IsAuthorized { get; private set; } = true;
    private readonly List<string> _errorMessages = [];

    public string ErrorMessages => string.Join(' ', _errorMessages);

    public void AddError(string errorMessage)
    {
        IsAuthorized = false;
        _errorMessages.Add(errorMessage);
    }
}