using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rocky.DTOs;
using Rocky.Helper;
using Rocky.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rocky.Services;

public class AuthService
{
	public UserManager<AppUser> userManager { get; set; }
	public SignInManager<AppUser> signInManager { get; set; }
	public JWT _jwt { get; set; }
	public AuthService(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, IOptions<JWT> jwt)
	{
		userManager = _userManager;
		signInManager = _signInManager;
		_jwt = jwt.Value;
	}

	public async Task<bool> CheckEmailExistsAsync(string email) {
		return await userManager.FindByEmailAsync(email) != null;
	}
	public async Task<bool> CheckUsernameExistsAsync(string userName)
	{
		return await userManager.FindByNameAsync(userName) != null;
	}

	public async Task<AuthDto> RegisterAsync(RegisterDto model)
	{
		if (await CheckEmailExistsAsync(model.Email)) return new AuthDto { Errors = new List<string> { "Email is already registered" } };
		if (await CheckUsernameExistsAsync(model.Username)) return new AuthDto { Errors = new List<string> { "Username is already registered" } };

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
			var errorList = new List<string>();
			foreach (var error in result.Errors) errorList.Add(error.Description);
			return new AuthDto { Errors = errorList };
		}

		await userManager.AddToRoleAsync(user, "User");

		var claimList = new List<Claim>() {
			new Claim(ClaimTypes.NameIdentifier, user.Id),
			new Claim(ClaimTypes.Email, user.Email),
			new Claim(ClaimTypes.Role, "User"),
		};

		await userManager.AddClaimsAsync(user, claimList);

		TokenDto token = await GetTokenAsync(user);

		return new AuthDto
		{
			Email = user.Email,
			ExpiresOn = token.ValidTo,
			IsAuthenticated = true,
			Roles = new List<string> { "User" },
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
}