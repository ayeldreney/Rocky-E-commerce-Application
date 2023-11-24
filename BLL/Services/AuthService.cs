using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rocky.BLL.Constants;
using Rocky.BLL.DTOs;
using Rocky.BLL.Helpers;
using Rocky.DAL.Models;
using Rocky.Localizations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rocky.BLL.Services;

/// <summary>
/// Class <c>AuthService</c> provide all CRUD operations and verifications for authenticating and authorizing user.
/// </summary>
public class AuthService
{
	public UserManager<AppUser> userManager { get; set; }
	public SignInManager<AppUser> signInManager { get; set; }
	public JWT _jwt { get; set; }
	public IHttpContextAccessor accessor { get; set; }
	Phrase _phrase { get; set; }
	public AuthService(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager,
		IOptions<JWT> jwt, IHttpContextAccessor _accessor, Phrase phrase)
	{
		userManager = _userManager;
		signInManager = _signInManager;
		_jwt = jwt.Value;
		accessor = _accessor;
		_phrase = phrase;
	}

	/// <summary>
	/// Method <c>CheckEmailExistsAsync</c>
	/// Checks for email entered by user to check if is avaiable or not.
	/// </summary>
	public async Task<bool> CheckEmailExistsAsync(string email)
	{
		return await userManager.FindByEmailAsync(email) != null;
	}

	/// <summary>
	/// Method <c>CheckUsernameExistsAsync</c>
	/// Checks for user name entered by user to check if is avaiable or not.
	/// </summary>
	public async Task<bool> CheckUsernameExistsAsync(string userName)
	{
		return await userManager.FindByNameAsync(userName) != null;
	}

	/// <summary>
	/// Method <c>VerifyEmailAndUserName</c>
	/// Used for verfication of user data in registration and profile editing.
	/// </summary>
	public async Task<AuthDto?> VerifyEmailAndUserName(RegisterDto model)
	{
		
		AuthDto auth = new AuthDto();
		bool emailExists = await CheckEmailExistsAsync(model.Email);
		bool userNameExists = await CheckUsernameExistsAsync(model.Username);
		if (emailExists || userNameExists)
		{
			auth.Errors = new Dictionary<string, string>();
			if (emailExists) auth.Errors.Add("Email", _phrase["Account", "emailAlreadyRegistered"]);
			if (userNameExists) auth.Errors.Add("Username", _phrase["Account", "userAlreadyRegistered"]);
		}
		return auth;
	}

	/// <summary>
	/// Method <c>GetUserOrDefault</c>
	/// Used for getting current user claims principal if user is logged in
	/// </summary>
	/// <returns>
	/// ClaimsPrincipal object if user logged in or null if not
	/// </returns>
	public ClaimsPrincipal? GetUserOrDefault()
	{
		return accessor?.HttpContext?.User as ClaimsPrincipal;
	}

	/// <summary>
	/// Method <c>GetCurrentUser</c>
	/// Used for getting current user data if user is logged in
	/// </summary>
	/// <returns>
	/// AppUser object if user logged in or null if not
	/// </returns>
	public AppUser? GetCurrentUser()
	{
		ClaimsPrincipal? principal = GetUserOrDefault();
		if (principal == null) return null;
		AppUser? user = userManager.GetUserAsync(principal).Result;
		return user;
	}

	/// <summary>
	/// Method <c>GetCurrentUserAsync</c>
	/// Asynchronous Method for for getting current user data if user is logged in
	/// </summary>
	/// <returns>
	/// AppUser object if user logged in or null if not
	/// </returns>
	public async Task<AppUser?> GetCurrentUserAsync()
	{
		string? userName = GetUserOrDefault()?.Identity?.Name;
		if (userName == null) return null;
		// get user data
		return await userManager.FindByNameAsync(userName);
	}

	public async Task<bool> CheckPasswordAsync(AppUser user, string password)
	{
		return await userManager.CheckPasswordAsync(user, password);
	}

	public async Task<AuthDto> RegisterAsync(RegisterDto model)
	{
		var checkUserOrEmailExists = await VerifyEmailAndUserName(model);

		if (checkUserOrEmailExists != null)
		{
			return checkUserOrEmailExists; // return errors in case of existance
		}

		var user = new AppUser()
		{
			UserName = model.Username,
			Email = model.Email,
			FirstName = model.FirstName,
			LastName = model.LastName
		};

		var result = await userManager.CreateAsync(user, model.Password);
		if (!result.Succeeded)
		{
			var errorList = new Dictionary<string, string>();
			foreach (var error in result.Errors) errorList.Add("", error.Description);
			return new AuthDto { Errors = errorList };
		}

		await userManager.AddToRoleAsync(user, UserRoles.User);

		var claimList = new List<Claim>() {
			new Claim(ClaimTypes.NameIdentifier, user.Id),
			new Claim(ClaimTypes.Email, user.Email),
			new Claim(ClaimTypes.Role, UserRoles.User),
		};

		await userManager.AddClaimsAsync(user, claimList);

		TokenDto token = await GetTokenAsync(user);

		return new AuthDto
		{
			Email = user.Email,
			ExpiresOn = token.ValidTo,
			IsAuthenticated = true,
			Roles = new List<string> { UserRoles.User },
			Token = token.Token,
			Username = user.UserName,
			Succeed = true
		};
	}

	public async Task<TokenDto> GetTokenAsync(AppUser user, bool persistant = false)
	{
		var userClaims = await userManager.GetClaimsAsync(user);
		SigningCredentials signingCredentials =
			new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwt.Key)),
				SecurityAlgorithms.HmacSha256Signature);

		DateTime expire = DateTime.Now.Add(persistant ? TimeSpan.FromDays(_jwt.DurationPersistantInDays) : TimeSpan.FromMinutes(_jwt.DurationInMinutes));

		//Determine how to generate hashing result

		var jwt = new JwtSecurityToken(
					claims: userClaims,
					notBefore: DateTime.Now,
					issuer: _jwt.Issuer,
					audience: _jwt.Audience,
					expires: expire,
					signingCredentials: signingCredentials);

		var tokenHandler = new JwtSecurityTokenHandler();
		string tokenString = tokenHandler.WriteToken(jwt);

		var tokenDto = new TokenDto()
		{
			Token = tokenString,
			ValidTo = expire
		};

		return tokenDto;
	}

	public async Task<AppUser?> CheckUserAsync(LoginCredentialsDto credentials)
	{
		AppUser? user = await userManager.FindByNameAsync(credentials.UserName);
		if (user == null) return null;

		bool isPassCorrect = await userManager.CheckPasswordAsync(user, credentials.Password);
		if (!isPassCorrect) return null;

		return user;
	}

	public async Task<TokenDto?> SignInAsync(LoginCredentialsDto credentials, ModelStateDictionary? state = null)
	{
		AppUser? user = await CheckUserAsync(credentials);
		if (user == null) return null;

		var isPasswordCorrect = await userManager.CheckPasswordAsync(user, credentials.Password);

		if (!isPasswordCorrect)
		{
			if (await userManager.IsLockedOutAsync(user))
			{
				if (state != null)
				{
					int lockoutTimeSpan = AppSettings.UserSettings.LockoutTimeSpanInMinutes;
					state.AddModelError("", "Too many attempts! Please wait for " + lockoutTimeSpan + " minutes then try again.");
				}
			}
			await userManager.AccessFailedAsync(user);
			return null;
		}
		TokenDto tokenResult = await GetTokenAsync(user, credentials.RememberMe);

		var cookieOptions = new CookieOptions
		{
			HttpOnly = false,
			Secure = true,
			SameSite = SameSiteMode.Unspecified,
			Expires = tokenResult.ValidTo // Adjust as needed
		};

		accessor.HttpContext?.Response.Cookies.Append(AppSettings.UserSettings.TokenBearerCookieName, tokenResult.Token, cookieOptions);

		return tokenResult;
	}

	public async void SignOut()
	{
		await signInManager.SignOutAsync();
		accessor.HttpContext?.Response.Cookies.Delete(AppSettings.UserSettings.TokenBearerCookieName);

		return;
	}

	public async Task<ProfileDto?> UpdateProfileDataAsync(ProfileDto profileData, ModelStateDictionary? state = null)
	{
		string? userName = accessor.HttpContext?.User.Identity?.Name;
		if (userName == null) return null;
		// get user data
		AppUser? user = await userManager.FindByNameAsync(userName);
		if (user == null) return null;
		// verify password
		if (!await userManager.CheckPasswordAsync(user, profileData.Password))
		{
			if (state != null) state.AddModelError("Password", "Invalid Password. Password is required to confirm profile changes.");
			return null;
		}

		user.Email = profileData.Email;
		user.FirstName = profileData.FirstName;
		user.LastName = profileData.LastName;
		user.PhoneNumber = profileData.PhoneNumber;
		user.Address = profileData.Address;

		await userManager.UpdateAsync(user);

		ProfileDto newProfileDto = ObjectMapper.Map<AppUser, ProfileDto>(user);

		return newProfileDto;
	}
}