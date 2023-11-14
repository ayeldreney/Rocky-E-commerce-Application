using System.ComponentModel.DataAnnotations;

namespace Rocky.ViewModels;

public class LoginViewModel
{
	[Required(ErrorMessage = "userNameRequired")]
	[Display(Name = "UserName")]
	public string UserName { get; set; } = string.Empty;
	[Required]
	[DataType(DataType.Password)]
	[Display(Name = "Password")]
	public string Password { get; set; } = string.Empty;
	[Display(Name = "RemeberMe")]
	public bool RememberMe { get; set; } = false;
}