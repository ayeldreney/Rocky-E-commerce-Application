namespace Rocky.DTOs;

public class AuthDto
{
	public bool IsAuthenticated { get; set; } = false;
	public string Username { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public List<string> Roles { get; set; } = new List<string>();
	public string Token { get; set; } = string.Empty;
	public DateTime ExpiresOn { get; set; }
	public IEnumerable<string>? Errors { get; set; } = null;
	public bool Succeed { get; set; } = false;
}
