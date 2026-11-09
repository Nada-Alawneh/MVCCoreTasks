using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UniversityCafeteria7.Data;
using UniversityCafeteria7.Models;

namespace UniversityCafeteria7.Controllers
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
            LoadLists();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            LoadLists();
            var product = _context.Products.Find(id);
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.Find(id);
            return View(product);
        }

        private void LoadLists()
        {
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            ViewBag.Suppliers = new SelectList(_context.Suppliers.ToList(), "Id", "Name");
        }
    }
}