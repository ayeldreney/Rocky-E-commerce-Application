﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
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
            HomeViewModel homeViewModel = new HomeViewModel() {
                Products = _db.Product.Include(x => x.Category).ToList(),
                Categories = _db.Category.ToList(), 
            
            }; 

            return View(homeViewModel);
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
}