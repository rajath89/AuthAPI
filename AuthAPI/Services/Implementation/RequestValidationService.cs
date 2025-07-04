using AuthAPI.Data;
using AuthAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Services.Implementation;

public class RequestValidationService : IRequestValidationService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
        
    public RequestValidationService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
        
    public async Task<bool> ValidateRequestHeaders(string applicationId, string traceId, string securityToken)
    {
        // Validate Application ID from database
        var appConfig = await _context.ApplicationConfigs
            .FirstOrDefaultAsync(x => x.ApplicationId == applicationId && x.IsActive);
            
        if (appConfig == null)
            return false;
            
        // Validate Security Token from config
        var expectedSecurityToken = _configuration["SecurityToken"] ?? "DEFAULT_SECURITY_TOKEN";
        if (securityToken != expectedSecurityToken)
            return false;
            
        // Validate TraceId format
        if (string.IsNullOrEmpty(traceId) || traceId.Length < 10)
            return false;
            
        return true;
    }
}