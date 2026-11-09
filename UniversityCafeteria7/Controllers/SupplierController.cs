using Microsoft.AspNetCore.Mvc;
using UniversityCafeteria7.Data;
using UniversityCafeteria7.Models;

namespace UniversityCafeteria7.Controllers
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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        [HttpPost]
        public IActionResult Edit(int id, Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return NotFound();
            }

            _context.Suppliers.Update(supplier);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
            {
                return NotFound();
            }

            _context.Suppliers.Remove(supplier);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }
    }
}