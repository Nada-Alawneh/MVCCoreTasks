using Microsoft.AspNetCore.Mvc;
using UniversityCafeteria6.Data;
using UniversityCafeteria6.Models;


namespace UniversityCafeteria.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
        }
    }
}