using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky.BLL.DTOs;
using Rocky.BLL.Helpers;
using Rocky.BLL.Services;
using Rocky.ViewModels;

namespace Rocky.Controllers;

public class AccountController : Controller
{
	AuthService authService;

	public AccountController(AuthService _authService)
	{
		authService = _authService;
	}

	public IActionResult? Index()
	{
		return null;
	}

	[HttpGet]
	[AllowAnonymous]
	public IActionResult Register()
	{
		return View();
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> Register(RegisterDto register)
	{
		if (!ModelState.IsValid)
		{
			return View(register);
		}

		AuthDto registerResult = await authService.RegisterAsync(register);
		if (registerResult.Succeed == false)
		{
			if (registerResult.Errors != null) 
				foreach(var error in registerResult.Errors)
				{
					ModelState.AddModelError("", error);
				}
			return View(register);
		}

		return View("RegisterSuccess");
	}

	[AllowAnonymous]
	[HttpGet]
	public IActionResult Login()
	{
        return View();
	}

	[AllowAnonymous]
	[HttpPost]
	public async Task<IActionResult> Login(LoginCredentialsDto credentials)
	{
		if (!ModelState.IsValid)
		{
			LoginViewModel loginViewModel = ObjectMapper.Map<LoginCredentialsDto, LoginViewModel>(credentials);
			return View(loginViewModel);
		}

		var result = await authService.SignInAsync(credentials);
		if (result == null)
		{
			ModelState.AddModelError("", "Username or password is invalid!");
			LoginViewModel loginViewModel = ObjectMapper.Map<LoginCredentialsDto, LoginViewModel>(credentials);

			return View(loginViewModel);
        }

        return View("LoginSuccess");
    }
}