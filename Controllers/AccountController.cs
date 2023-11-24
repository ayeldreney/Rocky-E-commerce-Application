using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky.BLL.Attributes;
using Rocky.BLL.DTOs;
using Rocky.BLL.Helpers;
using Rocky.BLL.Services;
using Rocky.DAL.Models;
using Rocky.Localizations;
using Rocky.ViewModels.Account;

namespace Rocky.Controllers;

public class AccountController : Controller
{
	AuthService authService;
	Phrase _phrase;

	public AccountController(AuthService _authService, Phrase phrase)
	{
		authService = _authService;
		_phrase = phrase;
	}

	//	[Authorize(Roles = "User")]
	public IActionResult Index()
	{
		return Content(_phrase["Account", "NotFoundMessage", "Hello bedo"]);
	}

	[HttpGet]
	[AllowAnonymous]
	[AnonymousOnly]
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
	[AnonymousOnly]
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
			ModelState.AddModelError("", _phrase["Account", "userNameOrPasswordInvalid"]);
			LoginViewModel loginViewModel = ObjectMapper.Map<LoginCredentialsDto, LoginViewModel>(credentials);

			return View(loginViewModel);
		}
		return RedirectToAction("Index", "Home");
	}

	[AllowAnonymous]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> LoginApi(LoginCredentialsDto credentials)
	{
		var test = HttpContext.Request.QueryString;
		if (!ModelState.IsValid)
		{
			return Json(false);
		}

		TokenDto? result = await authService.SignInAsync(credentials);
		if (result == null)
		{
			return Json(false);
		}

		return Json(true);
	}



	[Authorize]
	//	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Logout()
	{
		authService.SignOut();
		return RedirectToAction("Index", "Home");
	}

	[Authorize]
	[HttpGet]
	public IActionResult Profile()
	{
		var currentUser = authService.GetCurrentUser();
		if (currentUser == null) return Unauthorized();

		var model = new ProfileViewModel()
		{
			Username = currentUser.UserName ?? "",
			Password = "",
			FirstName = currentUser.FirstName,
			LastName = currentUser.LastName,
			Address = currentUser.Address ?? "",
			Email = currentUser.Email ?? "",
			PhoneNumber = currentUser.PhoneNumber ?? string.Empty,
		};

		return View(model);
	}

	[Authorize]
	[HttpPost]
	public async Task<IActionResult> Profile(ProfileViewModel model)
	{
		ViewBag.success = false;
		model.PhoneNumber = model.PhoneNumber;
		if (ModelState.IsValid)
		{
			ProfileDto profileDto = ObjectMapper.Map<ProfileViewModel, ProfileDto>(model);
			ProfileDto? succeed = await authService.UpdateProfileDataAsync(profileDto, ModelState);
			if (succeed != null) ViewBag.success = true;
		}
		return View(model);
	}

	[HttpPost("CheckPassword")]
	[ValidateAntiForgeryToken]
	public async Task<JsonResult> CheckPassword(string Password)
	{
		AppUser? user = await authService.GetCurrentUserAsync();

		if (user != null && !string.IsNullOrEmpty(Password))
		{
			bool isPasswordCorrect = await authService.CheckPasswordAsync(user, Password);
			if (isPasswordCorrect) return Json(true);
		}
		return Json(false);
	}

	public async Task<IActionResult> VerifyUserAndEmail(string? UserName, string? Email)
	{
		if (string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(Email))
		{
			return Json(null);
		}
		if (!string.IsNullOrEmpty(UserName))
		{
			if (await authService.CheckUsernameExistsAsync(UserName) == true)
			{
				return Json(false);	
			}
		}
		if (!string.IsNullOrEmpty(Email))
		{
			if (await authService.CheckEmailExistsAsync(Email) == true)
			{
				return Json(false);
			}
		}

		return Json(true);
	}
}