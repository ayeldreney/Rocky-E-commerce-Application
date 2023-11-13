using System.ComponentModel.DataAnnotations;

namespace Rocky.ViewModels;

public class LoginViewModel
{
	[Required]
	[Display(Name = "User Name")]
	public string UserName { get; set; } = string.Empty;
	[Required]
	[DataType(DataType.Password)]
	[Display(Name = "Password")]
	public string Password { get; set; } = string.Empty;
	[Display(Name = "Remeber Me?")]
	public bool RememberMe { get; set; } = false;
}
