using System.ComponentModel.DataAnnotations;

namespace Rocky.ViewModels.Account;

public class ProfileViewModel
{
	[Required, StringLength(100)]
	public string FirstName { get; set; } = string.Empty;

	[Required, StringLength(100)]
	public string LastName { get; set; } = string.Empty;

	public string Username { get; set; } = string.Empty;

	[Required]
	[RegularExpression(@"^[a-zA-Z0-9_]+@[a-zA-Z0-9_]+.[a-zA-Z]{2,4}$", ErrorMessage = "Email should be like john@doe.com")]
	public string Email { get; set; } = string.Empty;

	[StringLength(256)]
	[DataType(DataType.Password)]
	public string? Password { get; set; }

	[Required, StringLength(255)]
	public string Address { get; set; } = string.Empty;

	[Required(ErrorMessage = "You must provide a phone number")]
	[Display(Name = "Mobile Phone")]
	[DataType(DataType.PhoneNumber)]
	[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
	public string PhoneNumber { get; set; } = string.Empty;

}
