using Microsoft.AspNetCore.Mvc;
using System.Linq;
using UniversityCafeteria6.Models;
using UniversityCafeteria6.Data;

namespace UniversityCafeteria.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
             return RedirectToAction("Index");
        }
    }
}