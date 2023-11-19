using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Rocky.BLL.Attributes;

public class AnonymousOnlyAttribute : AuthorizeAttribute, IAuthorizationFilter
{
	public void OnAuthorization(AuthorizationFilterContext filterContext)
	{
		if (filterContext.HttpContext.User.Identity == null) return;
		if (filterContext.HttpContext.User.Identity.IsAuthenticated)
		{
			//filterContext.HttpContext.Response.Redirect("/");
		}
	}
}