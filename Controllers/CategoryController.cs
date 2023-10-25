using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;

using Rocky.Models;

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
        public IActionResult Create() {

            return View();
        }


        //post create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category) {

            _context.Set<Category>().Add(category); 
            _context.SaveChanges();



            return RedirectToAction("Index");

        }



    }
}
