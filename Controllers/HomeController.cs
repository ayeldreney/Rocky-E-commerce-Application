using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rocky.DAL.Data;
using Rocky.DAL.Models;
using Rocky.ViewModels;
using System.Diagnostics;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDBcontext _db;   
        public HomeController(ILogger<HomeController> logger,ApplicationDBcontext applicationDBcontext)
        {
            _logger = logger;
            _db = applicationDBcontext; 

        }

      public IActionResult Index()
      {
        HomeViewModel homeViewModel = new HomeViewModel()
        {
          Products = _db.Products.Include(x => x.Category).ToList(),
          Categories = _db.Categories.ToList(),

        };

        return View("Home", homeViewModel);
      }

        public IActionResult Details(int id) {

            //exist in cart default is false;

            DetailsViewModel detailsViewModel = new DetailsViewModel() {
                Product = _db.Products.Include(_x => _x.Category).Where(i => i.Id == id).FirstOrDefault(),
                ExistsInCart=false,

            
            };

        return View();  
        
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
    }

		public ActionResult SearchAboutProduct(string searchQuery)
		{
			if (string.IsNullOrEmpty(searchQuery))
			{
				ModelState.AddModelError("searchQuery", "Please enter a search query.");
				return View();
			}
			var filteredProducts = _db.Products.Include(p => p.Category).Where(p =>
				p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
				p.Category.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
			).ToList();

			if (filteredProducts.Count() == 0)
			{
				ModelState.AddModelError("searchQuery", "No results found.");
			}

			return View(filteredProducts);
		}
	}
}