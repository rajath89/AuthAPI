namespace AuthAPI.Models.Response;

public class AuthResponse
{
    public Completion Completion { get; set; } = new();
    public string Token { get; set; } = string.Empty;
    public string TokenType { get; set; }
}