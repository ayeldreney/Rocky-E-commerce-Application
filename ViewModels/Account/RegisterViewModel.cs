﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Rocky.ViewModels.Account;

public class RegisterViewModel
{
	[Required, StringLength(100)]
	[DisplayName("FirstName")]
	public string FirstName { get; set; } = string.Empty;

	[Required, StringLength(100)]
	[DisplayName("LastName")]
	public string LastName { get; set; } = string.Empty;

	[Required, StringLength(50, MinimumLength = 4)]
	[DisplayName("UserName")]
	[Remote("VerifyUserAndEmail", "Account", ErrorMessage = "UserNameExists", HttpMethod = "POST", AdditionalFields = "__RequestVerificationToken")]
	public string Username { get; set; } = string.Empty;

	[Required]
	[RegularExpression(@"^[a-zA-Z0-9_]+@[a-zA-Z0-9_]+.[a-zA-Z]{2,4}$", ErrorMessage = "InvalidEmailShouldBeLike")]
	[DisplayName("Email")]
	[Remote("VerifyUserAndEmail", "Account", ErrorMessage = "EmailExists", HttpMethod = "POST", AdditionalFields = "__RequestVerificationToken")]
	public string Email { get; set; }

	[Required, StringLength(256)]
	[DataType(DataType.Password)]
	[DisplayName("Password")]
	public string Password { get; set; } = string.Empty;
}
