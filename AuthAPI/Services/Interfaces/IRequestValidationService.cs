namespace AuthAPI.Services.Interfaces;

public interface IRequestValidationService
{
    Task<bool> ValidateRequestHeaders(string applicationId, string traceId, string securityToken);
}