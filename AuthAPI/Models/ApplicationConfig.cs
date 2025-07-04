namespace AuthAPI.Models;

public class ApplicationConfig
{
    public int Id { get; set; }
    public string ApplicationId { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? SecurityToken { get; set; }
}