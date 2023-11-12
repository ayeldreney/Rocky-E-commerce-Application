using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocky.DTOs;
using Rocky.Models;
using Rocky.Services;

namespace Rocky.Controllers;

public class UserController : Controller
{
	AuthService authService;
	UserManager<AppUser> userManager;

	public UserController(AuthService _authService, UserManager<AppUser> _userManager)
	{
		authService = _authService;
		userManager = _userManager;
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
            return View(credentials);
        }

		var result = await authService.SignInAsync(credentials);
		if (result == null)
		{
			ModelState.AddModelError("", "Username or password is invalid!");
            return View(credentials);
        }

        return View("LoginSuccess");
    }
}
