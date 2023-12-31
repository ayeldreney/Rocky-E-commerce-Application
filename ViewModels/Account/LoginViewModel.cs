using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Rocky.ViewModels.Account;

public class LoginViewModel
{
	[Required(ErrorMessage = "userNameRequired")]
	[Display(Name = "UserName")]
	public string UserName { get; set; } = string.Empty;
	[Required]
	[DataType(DataType.Password)]
	[Display(Name = "Password")]
//	[Remote("CheckPassword", "Account", ErrorMessage = "InvalidPassword", HttpMethod = "POST", AdditionalFields = "__RequestVerificationToken")]
	public string Password { get; set; } = string.Empty;
	[Display(Name = "RemeberMe")]
	public bool RememberMe { get; set; } = false;
}