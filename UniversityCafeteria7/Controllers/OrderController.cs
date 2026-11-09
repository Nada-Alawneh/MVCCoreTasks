using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UniversityCafeteria7.Data;
using UniversityCafeteria7.Models;

namespace UniversityCafeteria7.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _context.Orders.ToList();
            return View(orders);
        }

        [HttpGet]
        public IActionResult Create()
        {
            LoadLists();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            LoadLists();
            var order = _context.Orders.Find(id);
            return View(order);
        }

        [HttpPost]
        public IActionResult Edit(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var order = _context.Orders.Find(id);
            return View(order);
        }

        private void LoadLists()
        {
            ViewBag.Employee = new SelectList(_context.Employees.ToList(), "Id", "FullName");
        }
    }
}