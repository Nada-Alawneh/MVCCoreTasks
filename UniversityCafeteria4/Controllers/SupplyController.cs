using Microsoft.AspNetCore.Mvc;

namespace UniversityCafeteria2.Controllers
{
    public class SupplyController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Categories = new string[] { "Drinks", "Food", "Snacks", "Hot Drinks", "Cold Drinks" };
            return View();
        }

        public IActionResult Products()
        {
            ViewData["Title"] = "Cafeteria Products";
            ViewData["ManagerName"] = "Ahmad Ali";
            ViewData["ProductsCount"] = 5;
            ViewData["ProductList"] = new string[] { "Water", "Orange Juice", "Coffee", "Sandwich", "Chocolate" };

            return View();
        }

        public IActionResult Suppliers()
        {
            ViewBag.Title = "Our Suppliers";
            ViewBag.SuppliersCount = 3;
            ViewBag.SuppliersList = new string[] { "Fresh Food Company", "Happy Drinks Company", "Daily Supplies Company" };

            return View();
        }

        [HttpGet]
        public IActionResult CreateRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitRequest()
        {
            string empName = Request.Form["EmployeeName"];
            string productName = Request.Form["ProductName"];
            string quantityStr = Request.Form["Quantity"];

            if (string.IsNullOrEmpty(empName))
            {
                ViewBag.ErrorMessage = "Employee Name is required!";
                return View("CreateRequest");
            }

            TempData["Message"] = $"Success! Request for {quantityStr} {productName} submitted by {empName}.";

            int quantity = int.Parse(quantityStr);

            if (quantity < 20)
            {
                TempData["QuantityMessage"] = "Order Size Classification: Small Order";
            }
            else if (quantity >= 20 && quantity <= 50)
            {
                TempData["QuantityMessage"] = "Order Size Classification: Medium Order";
            }
            else
            {
                TempData["QuantityMessage"] = "Order Size Classification: Large Order";
            }

            return RedirectToAction("About");
        }

        public IActionResult About()
        {
            return View();
        }
    }
}