using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rocky.BLL.Constants;
using Rocky.BLL.Helpers;
using Rocky.DAL.Data;
using Rocky.DAL.Models;
using Rocky.Utility;
using Rocky.ViewModels;
using System.Diagnostics;

namespace Rocky.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly ApplicationDBcontext _db;

	public HomeController(ILogger<HomeController> logger, ApplicationDBcontext applicationDBcontext)
	{
		_logger = logger;
		_db = applicationDBcontext;

	}
	
	public async Task<IActionResult> Index()
	{
		//HomeViewModel homeViewModel = new HomeViewModel()
		//{
		//	Products = _db.Products.Include(x => x.Category).ToList(),
		//	Categories = _db.Categories.ToList(),
		//};

		ViewBag.featuredProducts = await _db.Products.OrderBy(p => Guid.NewGuid()).Take(8).ToListAsync();

		return View("Home");
	}

	public IActionResult Details(int id)
	{

        List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
        if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppSettings.ProductView.SessionCart) != null
            && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppSettings.ProductView.SessionCart).Count() > 0)
        {
            shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppSettings.ProductView.SessionCart);
        }

        //exist in cart default is false;

        var product=_db.Products.Include(_x=>_x.Category).Where(p => p.Id == id).FirstOrDefault(); 
		
		if (product == null) return BadRequest();

		DetailsViewModel detailsViewModel = new DetailsViewModel()
		{
			Product = product,
			ExistsInCart = false,
		};

        foreach (var item in shoppingCartList)
        {
            if (item.ProductId == id)
            {
                detailsViewModel.ExistsInCart = true;
            }
        }

        return View(detailsViewModel);

	}

    [HttpPost, ActionName("Details")]
    public IActionResult DetailsPost(int id)
    {
        List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
        if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppSettings.ProductView.SessionCart) != null
            && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppSettings.ProductView.SessionCart).Count() > 0)
        {
            shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppSettings.ProductView.SessionCart);
        }
        shoppingCartList.Add(new ShoppingCart { ProductId = id });
        HttpContext.Session.Set(AppSettings.ProductView.SessionCart, shoppingCartList);
        return RedirectToAction(nameof(Index));


     

    }


    public IActionResult RemoveFromCart(int id)
    {
        List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
        if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppSettings.ProductView.SessionCart) != null
            && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppSettings.ProductView.SessionCart).Count() > 0)
        {
            shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppSettings.ProductView.SessionCart);
        }

        var itemToRemove = shoppingCartList.SingleOrDefault(r => r.ProductId == id);
        if (itemToRemove != null)
        {
            shoppingCartList.Remove(itemToRemove);
        }

        HttpContext.Session.Set(AppSettings.ProductView.SessionCart, shoppingCartList);
        return RedirectToAction(nameof(Index));
    }




    public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}

	public ActionResult Search(string searchQuery)
	{
		if (string.IsNullOrEmpty(searchQuery))
		{
			ModelState.AddModelError("searchQuery", "Please enter a search query.");
			return View();
		}

		List<Product> filteredProducts = _db.Products.Include(p => p.Category).AsEnumerable()
		.Where(p =>
			p.Name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) != -1
			|| p.Category.Name.Contains(searchQuery)
		).ToList();

		if (filteredProducts.Count() == 0)
		{
			ModelState.AddModelError("searchQuery", "No results found.");
		}

		// In case of user make use of automcomplete to reach certain product
		if (filteredProducts.Count() == 1 && filteredProducts.First().Name == searchQuery)
		{
			return RedirectToAction("View", "Product", new { id = filteredProducts.First().Id });
		}

		ViewBag.searchQuery = searchQuery;

		return View(filteredProducts);
	}

	[HttpGet]
	public JsonResult SearchApi(string q)
	{
		if (string.IsNullOrEmpty(q))
			return Json(null);

		var filteredProducts = _db.Products.Select(p => p.Name).AsEnumerable()
		.Where(p =>	p.StartsWith(q, StringComparison.OrdinalIgnoreCase)).ToList();

		return Json(filteredProducts);
	}

}