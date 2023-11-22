using Rocky.DAL.Data;
using Rocky.DAL.Models;

namespace Rocky.BLL.Helpers;

public class CommonHelpers
{
	public static List<Category>? Categories { get; set; } = null;
	public static CommonHelpers? instance = null;

	public CommonHelpers(ApplicationDBcontext _db)
	{
		if (CommonHelpers.Categories == null)
		{
			CommonHelpers.Categories = _db.Categories.ToList();
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
