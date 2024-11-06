namespace Common.Structured.Authorization;

public interface IAuthorizer<in TParameters>
{
    AuthorizationResult Authorize(
        TParameters parameters);
}