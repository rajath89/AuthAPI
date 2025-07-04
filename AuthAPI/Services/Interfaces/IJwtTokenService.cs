using AuthAPI.Models;

namespace AuthAPI.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user, string traceId);
}