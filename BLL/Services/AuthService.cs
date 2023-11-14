using Microsoft.AspNetCore.Identity;
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

	public async Task<bool> CheckEmailExistsAsync(string email)
	{
		return await userManager.FindByEmailAsync(email) != null;
	}
	public async Task<bool> CheckUsernameExistsAsync(string userName)
	{
		return await userManager.FindByNameAsync(userName) != null;
	}

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
		return null;
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
		var claimList = await userManager.GetClaimsAsync(user);

		var algorithm = SecurityAlgorithms.HmacSha256Signature;

		var secretKeyRaw = _jwt.Key;
		var keyInBytes = Encoding.ASCII.GetBytes(secretKeyRaw!);
		var secretKey = new SymmetricSecurityKey(keyInBytes);

		var signingCredentials = new SigningCredentials(secretKey, algorithm);

		DateTime expire = DateTime.Now.Add(persistant ? TimeSpan.FromDays(_jwt.DurationPersistantInDays) : TimeSpan.FromMinutes(_jwt.DurationInMinutes));

		var token = new JwtSecurityToken(
			claims: claimList,
			signingCredentials: signingCredentials,
			expires: expire
		);

		var tokenHandler = new JwtSecurityTokenHandler();

		return new TokenDto()
		{
			Token = tokenHandler.WriteToken(token),
			ValidTo = expire
		};
	}

	public async Task<AppUser?> CheckUserAsync(LoginCredentialsDto credentials)
	{
		AppUser? user = await userManager.FindByNameAsync(credentials.UserName);
		if (user == null) return null;

		bool isPassCorrect = await userManager.CheckPasswordAsync(user, credentials.Password);
		if (!isPassCorrect) return null;

		return user;
	}

	public async Task<TokenDto?> SignInAsync(LoginCredentialsDto credentials)
	{
		AppUser? user = await CheckUserAsync(credentials);
		if (user == null) return null;

		SignInResult result = await signInManager.PasswordSignInAsync(user, credentials.Password, credentials.RememberMe, false);
		if (!result.Succeeded) return null;
		return await GetTokenAsync(user, credentials.RememberMe);
	}

	public async void SignOut()
	{
		await signInManager.SignOutAsync();
		return;
	}
	public ClaimsPrincipal? GetUserOrDefault()
	{
		return accessor?.HttpContext?.User as ClaimsPrincipal;
	}

	public AppUser? GetCurrentUser()
	{
		ClaimsPrincipal? principal = GetUserOrDefault();
		if (principal == null) return null;
		AppUser? user = userManager.GetUserAsync(principal).Result;
		return user;
	}
}