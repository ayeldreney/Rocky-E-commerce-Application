using System.ComponentModel.DataAnnotations;

namespace Rocky.ViewModels.Account;

public class RegisterViewModel
{
	[Required, StringLength(100)]
	public string FirstName { get; set; } = string.Empty;

	[Required, StringLength(100)]
	public string LastName { get; set; } = string.Empty;

	[Required, StringLength(50, MinimumLength = 4)]
	public string Username { get; set; } = string.Empty;

	[Required]
	[RegularExpression(@"^[a-zA-Z0-9_]+@[a-zA-Z0-9_]+.[a-zA-Z]{2,4}$", ErrorMessage = "Email should be like john@doe.com")]
	public string Email { get; set; }

	[Required, StringLength(256)]
	[DataType(DataType.Password)]
	public string Password { get; set; } = string.Empty;
}
