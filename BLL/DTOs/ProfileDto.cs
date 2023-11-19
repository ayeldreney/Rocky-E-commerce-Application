namespace Rocky.BLL.DTOs;

public class ProfileDto
{
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string Password { get; set; }
	public string Address { get; set; } = string.Empty;
	public string PhoneNumber { get; set; } = string.Empty;
}