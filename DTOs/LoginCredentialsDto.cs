namespace Rocky.DTOs;

public class LoginCredentialsDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool RememberMe { get; set; } = false;
}