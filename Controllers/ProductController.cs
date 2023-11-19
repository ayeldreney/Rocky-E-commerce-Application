using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky.BLL.Constants;
using Rocky.BLL.DTOs;
using Rocky.DAL.Data;
using Rocky.DAL.Models;
using Rocky.ViewModels;

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
					string upload = webRootPath + AppSettings.ProductView.ProductImagePath;
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
						string upload = webRootPath + AppSettings.ProductView.ProductImagePath;
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

			string upload = _webHostEnvironment.WebRootPath + AppSettings.ProductView.ProductImagePath;
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
