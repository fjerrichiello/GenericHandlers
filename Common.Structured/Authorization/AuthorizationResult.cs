namespace Common.Structured.Authorization;

public class AuthorizationResult
{
    public AuthorizationResult(string? error = null)
    {
        if (error is not null)
        {
            AddError(error);
        }
    }

    public bool IsAuthorized { get; private set; } = true;
    private readonly List<string> _errorMessages = [];
    public string ErrorMessage => string.Join(Environment.NewLine, _errorMessages);

    public void AddError(string errorMessage)
    {
        IsAuthorized = false;
        _errorMessages.Add(errorMessage);
    }
}