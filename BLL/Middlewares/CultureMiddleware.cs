using System.Globalization;
using System.Linq;

namespace Rocky.BLL.Middlewares;

public class CultureMiddleware
{
	private readonly RequestDelegate _next;
	public string[] supportedCultures = new string[] {
		"en-US",
		"ar-EG"
	};
	public static string[] rtlCultures = new string[] {
		"ar-EG"
	};

	public CultureMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		string culture;
		string? lang = context.Request.Cookies["language"];
		if (lang is not null && Array.Exists(supportedCultures, l => l == lang)) culture = lang;
		else culture = "en-US"; // Get the user's preferred culture here

		Constants.General.LocalizationCulture = culture;
		if (rtlCultures.Contains(culture)) {
			Constants.General.isRightToLeft = true;
		} else {
			Constants.General.isRightToLeft = false;
		}

		CultureInfo.CurrentCulture = new CultureInfo(culture);
		CultureInfo.CurrentUICulture = new CultureInfo(culture);

		await _next(context);
	}
}