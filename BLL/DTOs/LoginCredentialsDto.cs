﻿using System.ComponentModel.DataAnnotations;

namespace Rocky.BLL.DTOs;

public class LoginCredentialsDto
{
	[Required(ErrorMessage = "userNameRequired")]
	public string UserName { get; set; } = string.Empty;
	[Required]
	public string Password { get; set; } = string.Empty;
	public string? Email { get; set; }
	public bool RememberMe { get; set; } = false;
}