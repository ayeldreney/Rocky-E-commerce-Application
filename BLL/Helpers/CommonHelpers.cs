using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rocky.DAL.Data;
using Rocky.DAL.Models;

namespace Rocky.BLL.Helpers;

public class CommonHelpers
{
	public static List<Category>? Categories { get; set; } = null;
	public static Cart? Cart { get; set; } = null;

	public static CommonHelpers? instance = null;

	public CommonHelpers(ApplicationDBcontext _db, IHttpContextAccessor _httpContextAccessor, UserManager<AppUser> user)
	{
		var context = _httpContextAccessor.HttpContext;
		if (CommonHelpers.Categories == null)
		{
			CommonHelpers.Categories = _db.Categories.ToList();
		}

		if (context.User.Identity.IsAuthenticated)
		{
			if (CommonHelpers.Cart == null)
			{
				var userId = user.GetUserId(context.User);
				CommonHelpers.Cart = _db.Carts.Where(c => c.UserId == userId).Include(c => c.Items).ThenInclude(i => i.Product).FirstOrDefault();
				var item = "";
			}
		}
		else
		{
			CommonHelpers.Cart = null;
		}

		if (instance == null)
		{
			CommonHelpers.instance = this;
		}


	}

	static CommonHelpers()
	{
	}
}
