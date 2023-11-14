using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky.BLL.DTOs;
using Rocky.BLL.Helpers;
using Rocky.BLL.Services;
using Rocky.ViewModels;

namespace Rocky.Controllers;

public class AccountController : Controller,IDisposable
{
	AuthService authService;

	public AccountController(AuthService _authService)
	{
		authService = _authService;
	}

	public IActionResult Index()
	{
		return Content("");
	}

	[HttpGet]
	[AllowAnonymous]
	public IActionResult Register()
	{
		return View();
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
	{
        if (!ModelState.IsValid)
		{
            return View(registerViewModel);
		}
        RegisterDto registerDto = ObjectMapper.Map<RegisterViewModel, RegisterDto>(registerViewModel);
        AuthDto registerResult = await authService.RegisterAsync(registerDto);

		if (registerResult.Succeed == false)
		{
            if (registerResult.Errors != null && registerResult.Errors.Count > 0)
                foreach (KeyValuePair<string, string> error in registerResult.Errors)
				{
					ModelState.AddModelError(error.Key, error.Value);
				}
            return View(registerViewModel);
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


	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Logout()
	{
		authService.SignOut();
		return RedirectToAction("Index", "Home");
	}

	[Authorize]
	public IActionResult Profile()
	{
		var currentUser = authService.GetCurrentUser();
		if (currentUser == null) return Unauthorized();

		var model = new ViewModels.Account.ProfileViewModel();
		model.Username = currentUser.UserName;
		model.Password = "";
		model.FirstName = currentUser.FirstName;
		model.LastName = currentUser.LastName;
		model.Address = currentUser.Address ?? "";
		model.Email = currentUser.Email;
		model.PhoneNumber = currentUser.PhoneNumber ?? string.Empty;

		return View(model);
	}
}