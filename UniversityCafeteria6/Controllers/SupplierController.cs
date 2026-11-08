using Microsoft.AspNetCore.Mvc;
using System.Linq;
using UniversityCafeteria6.Data;
using UniversityCafeteria6.Models;

namespace UniversityCafeteria.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var suppliers = _context.Suppliers.ToList();
            return View(suppliers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Supplier supplier)
        {
                _context.Suppliers.Add(supplier);
                _context.SaveChanges();
                return RedirectToAction("Index");
        }
    }
}