using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Web;

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

	public static int ParseInt(this string value, int defaultIntValue = 0)
	{
		int parsedInt;
		if (int.TryParse(value, out parsedInt))
		{
			return parsedInt;
		}

		return defaultIntValue;
	}

	public static int? ParseNullableInt(this string value)
	{
		if (string.IsNullOrEmpty(value))
			return null;

		return value.ParseInt();
	}

	public static int[]? StringToIntArray(this string? myNumbers)
	{
		if (myNumbers == null) return null;
		List<int> myIntegers = new List<int>();
		Array.ForEach(myNumbers.Split(",".ToCharArray()), s =>
		{
			int currentInt;
			if (Int32.TryParse(s, out currentInt))
				myIntegers.Add(currentInt);
		});
		return myIntegers.ToArray();
	}

	public static string TextToHtml(this string text)
	{
		if (text == null)
		{
			return String.Empty;
		}
		text = HttpUtility.HtmlEncode(text);
		text = text.Replace("\r\n", "\r");
		text = text.Replace("\n", "\r");
		text = text.Replace("\r", "<br>\r\n");
		text = text.Replace("  ", " &nbsp;");
		return text;
	}
}
