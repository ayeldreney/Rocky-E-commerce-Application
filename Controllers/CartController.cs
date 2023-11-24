using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Rocky.BLL.Constants;
using Rocky.DAL.Data;
using Rocky.DAL.Models;
using Rocky.Utility;

namespace Rocky.Controllers
{
    public class CartController : Controller
    {
       private readonly ApplicationDBcontext _db;
       private readonly IWebHostEnvironment _webHostEnvironment;


        public CartController(ApplicationDBcontext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            
        }



        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppSettings.ProductView.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppSettings.ProductView.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppSettings.ProductView.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Products.Where(u => prodInCart.Contains(u.Id));

            return View(prodList);
         
        }

        public IActionResult Remove(int id)
        {

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppSettings.ProductView.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppSettings.ProductView.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppSettings.ProductView.SessionCart);
            }

            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.ProductId == id));
            HttpContext.Session.Set(AppSettings.ProductView.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }


    }
}
