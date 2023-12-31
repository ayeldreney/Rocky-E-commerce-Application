using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky.BLL.Constants;
using Rocky.BLL.DTOs;
using Rocky.BLL.Helpers;
using Rocky.DAL.Data;
using Rocky.DAL.Models;
using Rocky.ViewModels;
using System.Linq;

namespace Rocky.Controllers
{
	public class ProductController : Controller
	{
		private readonly ApplicationDBcontext _db;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(ApplicationDBcontext db, IWebHostEnvironment webHostEnvironment)
		{
			_db = db;
			_webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{

			IEnumerable<Product> objList = _db.Products.Include(u => u.Category);

			//foreach(var obj in objList)
			//{
			//    obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
			//    obj.ApplicationType = _db.ApplicationType.FirstOrDefault(u => u.Id == obj.ApplicationTypeId);
			//};

			return View(objList);
		}

		public IActionResult List(int? priceFrom, int? priceTo, string? categoryList, [FromQuery] int? page, [FromQuery] string? sortBy, string? order, int? categoryId)
		{
			if (categoryId != null && categoryList is null)
			{
				categoryList = categoryId.ToString();
			}
			int[]? categories = categoryList.StringToIntArray();
			if (priceFrom == null
				&& priceTo == null
				&& (categories == null || categories.Length == 0)
			) return BadRequest();

			IEnumerable<Product> query = _db.Products.AsEnumerable();

			if (priceFrom > 0) query = query.Where(p => p.Price > priceFrom);
			if (priceTo > 0) query = query.Where(p => p.Price < priceTo);
			if (categories != null && categories.Length > 0)
			{
				query = query.Where(p => categories.Contains(p.CategoryId));
			}

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

			int productsCount = query.Count();
			int totalPages = (int)Math.Ceiling((double)productsCount / productsPerPage);

			if (page > totalPages) page = totalPages;

			List<Product> Products;

			if (sort.Order == "DESC")
			{
				Products = query.OrderByDescending(p => p.GetType().GetProperty(sort.Column)!.GetValue(p, null))
					.Skip(((int)page - 1) * productsPerPage)
					.Take(productsPerPage).ToList();
			}
			else
			{
				Products = query.OrderBy(p => p.GetType().GetProperty(sort.Column)!.GetValue(p, null))
					.Skip(((int)page - 1) * productsPerPage)
					.Take(productsPerPage).ToList();
			}

			ViewBag.priceFrom = priceFrom;
			ViewBag.priceTo = priceTo;
			ViewBag.categories = categories;
			if (categories.Count() == 1)
			{
		//		var theOnlyCategory = _db.Categories.Where(c => c.Id == categories[0]).FirstOrDefault();
		//		ViewBag.CategoryName = theOnlyCategory.Name;
		//		ViewBag.CategoryId = theOnlyCategory.Id;
			}


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

		public async Task<IActionResult> Details(int id)
		{
			var product = _db.Products?.Include(p => p.Category).Include(p => p.UserReviews).Where(p => p.Id == id)?.FirstOrDefault();

			if (product is null) return BadRequest();

			var userRatingCount = product.UserReviews.Select(p => p.Rating).Count();
			var userRatingSum = product.UserReviews.Select(p => p.Rating).Sum();
			var userRating = (userRatingCount > 0 ? userRatingSum / userRatingCount : 0);
			//exist in cart default is false;

			var featured = await _db.Products!
				.Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
				.OrderBy(p => Guid.NewGuid()).Take(5).ToListAsync();

			DetailsViewModel detailsViewModel = new DetailsViewModel()
			{
				Product = product,
				ExistsInCart = false,
				UserRating = userRating,
				UserRatingCount = userRatingCount,
				FeaturedProducts = featured,
			};

			return View(detailsViewModel);
		}

		//get UpSert
		[HttpGet]
		public IActionResult UpSert(int? id)
		{

			/*  IEnumerable<SelectListItem> CategoryDropDown = _db.Category.Select(
              i=>new SelectListItem
              {
                  Text = i.Name,
                  Value = i.Id.ToString()
              });

               ViewBag.CategoryDropDown = CategoryDropDown;        

               Product product = new Product();*/

			ProductViewModel productVM = new ProductViewModel()
			{
				Product = new Product() { },
				CategorySelectList = _db.Categories.Select(i => new SelectListItem
				{
					Text = i.Name,
					Value = i.Id.ToString()
				})
			};

			if (id == null)
			{
				//this is for create
				return View(productVM);
			}
			else
			{
				productVM.Product = _db.Products.Find(id);
				if (productVM.Product == null)
				{
					return NotFound();
				}
				return View(productVM);
			}
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult UpSert(ProductDto productVM)
		{
			if (ModelState.IsValid)
			{
				var files = HttpContext.Request.Form.Files;
				string webRootPath = _webHostEnvironment.WebRootPath;

				if (productVM.Product.Id == 0)
				{
					//Creating
					string upload = webRootPath + AppSettings.Product.ProductImagePath;
					string fileName = Guid.NewGuid().ToString();
					string extension = Path.GetExtension(files[0].FileName);

					using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
					{
						files[0].CopyTo(fileStream);
					}

					productVM.Product.Image = fileName + extension;

					var newProd = new Product()
					{
						Id = productVM.Product.Id,
						Name = productVM.Product.Name,
						ShortDesc = productVM.Product.ShortDesc,
						Description = productVM.Product.Description,
						Price = productVM.Product.Price,
						Image = productVM.Product.Image,
						CategoryId = productVM.Product.CategoryId,
					};
					_db.Products.Add(newProd);
					_db.SaveChanges();
					return RedirectToAction("Index");
				}
				else
				{
					//updating
					var objFromDb = _db.Products.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id);

					if (files.Count > 0)
					{
						string upload = webRootPath + AppSettings.Product.ProductImagePath;
						string fileName = Guid.NewGuid().ToString();
						string extension = Path.GetExtension(files[0].FileName);

						var oldFile = Path.Combine(upload, objFromDb.Image);

						if (System.IO.File.Exists(oldFile))
						{
							System.IO.File.Delete(oldFile);
						}

						using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
						{
							files[0].CopyTo(fileStream);
						}

						productVM.Product.Image = fileName + extension;
					}
					else
					{
						productVM.Product.Image = objFromDb.Image;
					}
					var newProd = new Product()
					{
						Id = productVM.Product.Id,
						Name = productVM.Product.Name,
						ShortDesc = productVM.Product.ShortDesc,
						Description = productVM.Product.Description,
						Price = productVM.Product.Price,
						Image = productVM.Product.Image,
						CategoryId = productVM.Product.CategoryId,
					};
					_db.Products.Update(newProd);
				}

				_db.SaveChanges();
				return RedirectToAction("Index");
			}
			ProductViewModel productvM = new ProductViewModel()
			{
				Product = new Product(),
				CategorySelectList = _db.Categories.Select(i => new SelectListItem
				{
					Text = i.Name,
					Value = i.Id.ToString()
				})
			};
			return View(productvM);
		}

		//GET - DELETE
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Product product = _db.Products.Include(u => u.Category).FirstOrDefault(u => u.Id == id);
			//product.Category = _db.Category.Find(product.CategoryId);
			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		//POST - DELETE
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public IActionResult DeletePost(int? id)
		{
			var obj = _db.Products.Find(id);
			if (obj == null)
			{
				return NotFound();
			}

			string upload = _webHostEnvironment.WebRootPath + AppSettings.Product.ProductImagePath;
			var oldFile = Path.Combine(upload, obj.Image);

			if (System.IO.File.Exists(oldFile))
			{
				System.IO.File.Delete(oldFile);
			}


			_db.Products.Remove(obj);
			_db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}
