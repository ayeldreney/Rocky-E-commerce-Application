using Microsoft.AspNetCore.Identity;
using Rocky.DTOs;
using Rocky.Models;
using System.Security.Claims;

namespace Rocky.Services;

public class UserService
{
	public UserManager<User> userManager { get; set; }
	public SignInManager<User> signInManager { get; set; }
	public UserService(UserManager<User> _userManager, SignInManager<User> _signInManager)
	{
		userManager = _userManager;
		signInManager = _signInManager;
	}

	public async Task<bool> VerifyUserByNameAndPassword(LoginCredentialsDto credentials)
	{
		User? user = await userManager.FindByNameAsync(credentials.UserName);
		if (user == null) return false;

		bool isPassCorrect = await userManager.CheckPasswordAsync(user, credentials.Password);
		if (isPassCorrect) return false;

		return true;
	}

	public async Task<bool> SignInByNameAndPassword(LoginCredentialsDto credentials)
	{
		bool isUserCredentialsValid = await VerifyUserByNameAndPassword(credentials);
		if (!isUserCredentialsValid) return false;

		SignInResult result = await signInManager.PasswordSignInAsync(credentials.UserName, credentials.Password, credentials.RememberMe, false);
		if (!result.Succeeded) return false;

		return true;
	}


	public async Task<IList<Claim>> GetClaimsAsync(User user)
	{
		return await userManager.GetClaimsAsync(user);
	}


}
