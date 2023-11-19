using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Rocky.ViewModels.Account;

public class ProfileViewModel
{
	[Required, StringLength(100)]
	[DisplayName("FirstName")]
	public string FirstName { get; set; } = string.Empty;

	[Required, StringLength(100)]
	[DisplayName("LastName")]
	public string LastName { get; set; } = string.Empty;

	//	[Editable(false), ReadOnly(true)]
	[DisplayName("UserName")]
	public string Username { get; set; } = string.Empty;

	[Required]
	[RegularExpression(@"^[a-zA-Z0-9_]+@[a-zA-Z0-9_]+.[a-zA-Z]{2,4}$", ErrorMessage = "Email should be like john@doe.com")]
	[DisplayName("Email")]
	public string Email { get; set; } = string.Empty;

	[Required(ErrorMessage = "You must enter your password to be able to confirm your changes")]
	[StringLength(100)]
	[DataType(DataType.Password)]
	[DisplayName("Password")]
	[Remote("CheckPassword", "Account", ErrorMessage = "InvalidPassword", HttpMethod = "POST", AdditionalFields = "__RequestVerificationToken")]
	public string Password { get; set; } = string.Empty;

	[Required, StringLength(255)]
	[DisplayName("Address")]
	public string Address { get; set; } = string.Empty;

	[Required(ErrorMessage = "You must provide a phone number")]
	[DisplayName("MobilePhone")]
	[DataType(DataType.PhoneNumber)]
	[RegularExpression(@"^\+?([0-9]{1,3})[-. ]?([0-9]{3})[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
	public string PhoneNumber { get; set; } = string.Empty;
}
