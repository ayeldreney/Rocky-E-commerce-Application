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

            if (ModelState.IsValid) {
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
            if (id is null || id is 0) {
                return NotFound();  
            }

            var obj = _context.Category.Find(id);

            if (obj is null) {
                return NotFound();
            }


            return View(obj);
        }

        //post edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj) {

            if (ModelState.IsValid) { 
            _context.Category.Update(obj);
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

            var obj = _context.Category.Find(id);

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
            var obj = _context.Category.Find(id);
         
            if(obj is null) return NotFound();  

                _context.Category.Remove(obj);
                _context.SaveChanges();
                return RedirectToAction("Index");
          


        
        }





    }
}
