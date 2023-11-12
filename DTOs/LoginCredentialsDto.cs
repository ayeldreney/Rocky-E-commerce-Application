using System.ComponentModel.DataAnnotations;

namespace Rocky.DTOs;

public class LoginCredentialsDto
{
    [Required]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool RememberMe { get; set; } = false;
}