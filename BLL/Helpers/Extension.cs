using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Rocky.DAL.Models;
using System.Net.NetworkInformation;

namespace Rocky.BLL.Helpers;

public static class Extension
{
	public static bool IsAuthenticated(this RazorPageBase page)
	{
		return page.User.Identity != null &&
			page.User.Identity.IsAuthenticated;
	}

	public static bool IsUser(this RazorPageBase page)
	{
		return page.User.Identity != null &&
			page.User.Identity.IsAuthenticated &&
			page.User.IsInRole(Constants.UserRoles.User);
	}

	public static bool IsAdmin(this RazorPageBase page)
	{
		return page.User.Identity != null &&
			page.User.Identity.IsAuthenticated &&
			page.User.IsInRole(Constants.UserRoles.Admin);
	}

}
