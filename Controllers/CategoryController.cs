using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rocky.BLL.Constants;
using Rocky.DAL.Data;
using Rocky.DAL.Models;
using Rocky.ViewModels;

namespace Rocky.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ApplicationDBcontext _context;


		public CategoryController(ApplicationDBcontext applicationDBcontext)
		{
			_context = applicationDBcontext;
		}


		public IActionResult Index()
		{

			IEnumerable<Category> categories = _context.Set<Category>().AsNoTracking().ToList();
			return View(categories);
		}


		//get create
		[HttpGet]
		public IActionResult Create()
		{

			return View();
		}


		//post create 
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(Category category)
		{

			if (ModelState.IsValid)
			{
				_context.Set<Category>().Add(category);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}
			else { return View(category); }


		}

		//get edit
		[HttpGet]
		public IActionResult Edit(int? id)
		{
			if (id is null || id is 0)
			{
				return NotFound();
			}

			var obj = _context.Categories.Find(id);

			if (obj is null)
			{
				return NotFound();
			}


			return View(obj);
		}

		//post edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Category obj)
		{

			if (ModelState.IsValid)
			{
				_context.Categories.Update(obj);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}


			return View(obj);
		}




		//get delete
		[HttpGet]

		public IActionResult Delete(int? id)
		{
			if (id is null || id is 0)
			{
				return NotFound();
			}

			var obj = _context.Categories.Find(id);

			if (obj is null)
			{
				return NotFound();
			}
			return View(obj);
		}






		//post delete
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult DeletePost(int? id)
		{
			var obj = _context.Categories.Find(id);

			if (obj is null) return NotFound();

			_context.Categories.Remove(obj);
			_context.SaveChanges();
			return RedirectToAction("Index");




		}


		public async Task<IActionResult> View(int? id, [FromQuery] int? page, [FromQuery] string? sortBy, string? order)
		{
			if (id == null) return BadRequest();
			if (page == null || page < 1)
			{
				page = 1;
			}
			var productsPerPage = AppSettings.Product.ProductsPerPage;

			SortListBase? sort = AppSettings.Product.SortByList[0];
			if (sortBy != null)
			{
				if (order == null || !new string[] { "ASC", "DESC" }.Contains(order))
					order = "ASC";

				for (int i = 0; i < AppSettings.Product.SortByList.Length; i++)
				{
					if (AppSettings.Product.SortByList[i].Column.ToLower() == sortBy.ToLower())
					{
						if (order == AppSettings.Product.SortByList[i].Order)
						{
							sort = AppSettings.Product.SortByList[i];
							break;
						}
					}
				}
				ViewBag.sortBy = sort.Column;
				ViewBag.order = order;
			}

			int productsCount = _context.Products.Where(p => p.CategoryId == id).Count();
			int totalPages = (int)Math.Ceiling((double)productsCount / productsPerPage);

			if (page > totalPages) page = totalPages;

			IEnumerable<Product> ProductQuery = _context.Products
					.Where(p => p.CategoryId == id).AsEnumerable();

			List<Product> Products;

			if (sort.Order == "DESC")
			{
				Products = ProductQuery.OrderByDescending(p => p.GetType().GetProperty(sort.Column)!.GetValue(p, null))
					.Skip(((int)page - 1) * productsPerPage)
					.Take(productsPerPage).ToList();
			}
			else
			{
				Products = ProductQuery.OrderBy(p => p.GetType().GetProperty(sort.Column)!.GetValue(p, null))
					.Skip(((int)page - 1) * productsPerPage)
					.Take(productsPerPage).ToList();
			}
			var category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
			ViewBag.CategoryName = category?.Name;
			ViewBag.CategoryId = category?.Id;
			ViewBag.priceTo = null;
			ViewBag.priceFrom = null;

			ProductListViewModel model = new ProductListViewModel()
			{
				Products = Products,
				TotalPages = totalPages,
				ProductsCount = productsCount,
				CurrentPage = (int)page,
				ProductsPerPage = productsPerPage,
				NextPage = (page + 1 <= totalPages ? (int)page + 1 : 0),
				PreviousPage = (page == 1 ? (int)0 : (int)page - 1),
				OrderBy = sort.Phrase,
			};

			return View("ProductListView", model);
		}


	}
}
