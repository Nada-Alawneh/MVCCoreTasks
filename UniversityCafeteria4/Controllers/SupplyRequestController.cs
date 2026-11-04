using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UniversityCafeteria2.Models;

namespace UniversityCafeteria2.Controllers
{
    public class SupplyRequestController : Controller
    {

        public static List<SupplyRequest> requests = new List<SupplyRequest>();

        public static int requestCounter = 1;
        [HttpGet]
        public IActionResult Index()
        {

            string preferredPriority = Request.Cookies["PreferredPriority"];

            ViewBag.PreferredPriority = preferredPriority;

            return View();
        }
        [HttpPost]
        public IActionResult AddItem(string ProductName, string Category, int Quantity, decimal UnitPrice, string priority)
        {

            if (string.IsNullOrWhiteSpace(ProductName))
            {
                ViewBag.Error = "Product name is required";
                return View("Index");
            }

            if (Quantity <= 0)
            {
                ViewBag.Error = "Quantity must be greater than zero";
                return View("Index");
            }

            if (UnitPrice <= 0)
            {
                ViewBag.Error = "Unit price must be greater than zero";
                return View("Index");
            }

            var items = HttpContext.Session.GetString("Products");

            List<SupplyItem> products;

            if (items == null)
            {
                products = new List<SupplyItem>();
            }
            else
            {
                products = JsonSerializer.Deserialize<List<SupplyItem>>(items) ?? new List<SupplyItem>();
            }

            SupplyItem product = new SupplyItem();

            product.ProductName = ProductName;
            product.Quantity = Quantity;
            product.UnitPrice = UnitPrice;
            product.Priority = priority;

            products.Add(product);

            HttpContext.Session.SetString("Products", JsonSerializer.Serialize(products));

            return RedirectToAction("Index");
        }
        public IActionResult Review()
        {
            var items = HttpContext.Session.GetString("Products");
            if (items == null)
            {
                return RedirectToAction("Index");
            }
            List<SupplyItem> products = JsonSerializer.Deserialize<List<SupplyItem>>(items) ?? new List<SupplyItem>();

            if (products == null || products.Count == 0)
            {
                return RedirectToAction("Index");
            }
            decimal subtotal = 0;
            foreach (var product in products)
            {
                subtotal += product.TotalCost;
            }
            // Determine Priority
            string requestPriority = "Normal";

            if (products.Any(p => p.Priority == "Emergency"))
            {
                requestPriority = "Emergency";
            }
            else if (products.Any(p => p.Priority == "Urgent"))
            {
                requestPriority = "Urgent";
            }

            decimal preparationFee = 0;

            switch (requestPriority)
            {
                case "Normal":
                    preparationFee = 2;
                    break;

                case "Urgent":
                    preparationFee = 5;
                    break;

                case "Emergency":
                    preparationFee = 10;
                    break;
            }
            decimal discount = 0;

            if (subtotal > 200)
            {
                discount = subtotal * 0.15m;
            }
            else if (subtotal > 100)
            {
                discount = subtotal * 0.10m;
            }

            decimal finalTotal = subtotal + preparationFee - discount;

            var model = new SupplyRequestViewModel
            {
                Items = products,
                Subtotal = subtotal,
                PreparationFee = preparationFee,
                Discount = discount,
                FinalTotal = finalTotal
            };

            return View(model);
        }
        public IActionResult ConfirmRequest()
        {
            var items = HttpContext.Session.GetString("Products");

            if (items == null)
            {
                return RedirectToAction("Index");
            }

            List<SupplyItem> products =
                JsonSerializer.Deserialize<List<SupplyItem>>(items) ?? new List<SupplyItem>();

            if (products == null || products.Count == 0)
            {
                return RedirectToAction("Index");
            }

            decimal subtotal = 0;

            foreach (var product in products)
            {
                subtotal += product.TotalCost;
            }

            string requestPriority = "Normal";

            if (products.Any(p => p.Priority == "Emergency"))
            {
                requestPriority = "Emergency";
            }
            else if (products.Any(p => p.Priority == "Urgent"))
            {
                requestPriority = "Urgent";
            }

            decimal preparationFee = 0;

            switch (requestPriority)
            {
                case "Normal":
                    preparationFee = 2;
                    break;

                case "Urgent":
                    preparationFee = 5;
                    break;

                case "Emergency":
                    preparationFee = 10;
                    break;
            }

            decimal discount = 0;

            if (subtotal > 200)
            {
                discount = subtotal * 0.15m;
            }
            else if (subtotal > 100)
            {
                discount = subtotal * 0.10m;
            }

            decimal finalTotal = subtotal + preparationFee - discount;

            SupplyRequest request = new SupplyRequest();

            request.RequestId = requestCounter++;

            request.NumberOfProducts = products.Count;

            request.Subtotal = subtotal;

            request.PreparationFee = preparationFee;

            request.Discount = discount;

            request.FinalTotal = finalTotal;

            request.Status = "Confirmed";

            requests.Add(request);

            HttpContext.Session.Remove("Products");

            return RedirectToAction("MyRequests");
        }
        public IActionResult ClearDraft()
        {
            HttpContext.Session.Remove("Products");

            return RedirectToAction("Index");
        }
        public IActionResult SavePreference(string priority)
        {

            CookieOptions options = new CookieOptions();

            options.Expires = DateTime.Now.AddDays(7);

            Response.Cookies.Append("PreferredPriority", priority, options);

            return RedirectToAction("Index");
        }
        public IActionResult MyRequests()
        {
            return View(requests);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}