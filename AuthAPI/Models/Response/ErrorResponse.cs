namespace AuthAPI.Models.Response;

public class ErrorResponse
{
    public List<ErrorInfo> ErrorInfo { get; set; } = new();
}