using AcademyGraduationCeremony.Models;
using Microsoft.AspNetCore.Mvc;

namespace AcademyGraduationCeremony.Controllers
{
    public class AcademyGraduationController : Controller
    {
        private List<Offer> GetOffers()
        {
            return new List<Offer>
            {
                new Offer { Id = 1, CategoryId = 1, CategoryName = "Graduation Certificates", SupplierName = "Supplier A", OfferName = "Premium Certificate Package", Quantity = 35, UnitPrice = 2 },
                new Offer { Id = 2, CategoryId = 1, CategoryName = "Graduation Certificates", SupplierName = "Supplier B", OfferName = "Standard Certificate Package", Quantity = 35, UnitPrice = 1.5 },
                new Offer { Id = 3, CategoryId = 1, CategoryName = "Graduation Certificates", SupplierName = "Supplier C", OfferName = "Luxury Certificate Package", Quantity = 35, UnitPrice = 2.5 },

                new Offer { Id = 4, CategoryId = 2, CategoryName = "Hospitality", SupplierName = "Supplier D", OfferName = "Coffee & Drinks Package", Quantity = 100, UnitPrice = 2 },
                new Offer { Id = 5, CategoryId = 2, CategoryName = "Hospitality", SupplierName = "Supplier E", OfferName = "Sandwiches & Snacks Package", Quantity = 100, UnitPrice = 3 },
                new Offer { Id = 6, CategoryId = 2, CategoryName = "Hospitality", SupplierName = "Supplier F", OfferName = "Full Hospitality Package", Quantity = 100, UnitPrice = 4 },

                new Offer { Id = 7, CategoryId = 3, CategoryName = "Graduation Gifts", SupplierName = "Supplier G", OfferName = "Customized Mugs Package", Quantity = 35, UnitPrice = 2 },
                new Offer { Id = 8, CategoryId = 3, CategoryName = "Graduation Gifts", SupplierName = "Supplier H", OfferName = "Graduation Bags Package", Quantity = 35, UnitPrice = 3 },
                new Offer { Id = 9, CategoryId = 3, CategoryName = "Graduation Gifts", SupplierName = "Supplier I", OfferName = "Premium Gift Boxes Package", Quantity = 35, UnitPrice = 4 }
            };
        }
        public IActionResult Index()
        {
            return View(GetOffers());
        }


        public IActionResult SelectOffer(int id, int categoryId)
        {
            HttpContext.Session.SetInt32($"Category_{categoryId}", id);
            TempData["Message"] = "Offer selected successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult Review()
        {
            
            int? cat1Id = HttpContext.Session.GetInt32("Category_1");
            int? cat2Id = HttpContext.Session.GetInt32("Category_2");
            int? cat3Id = HttpContext.Session.GetInt32("Category_3");

            
            if (cat1Id == null || cat2Id == null || cat3Id == null)
            {
                TempData["Error"] = "You must select one offer from ALL 3 categories before reviewing.";
                return RedirectToAction("Index");
            }

            var allOffers = GetOffers();
            var selectedOffers = new List<Offer>
            {
                allOffers.First(o => o.Id == cat1Id),
                allOffers.First(o => o.Id == cat2Id),
                allOffers.First(o => o.Id == cat3Id)
            };

            
            double totalPrice = 0;
            foreach (var offer in selectedOffers)
            {
                totalPrice += (offer.Quantity * offer.UnitPrice);
            }

            double discountPercentage = 0;
            if (totalPrice > 500)
                discountPercentage = 0.15;
            else if (totalPrice > 300)
                discountPercentage = 0.10;
            else
                discountPercentage = 0;

            double discountAmount = totalPrice * discountPercentage;
            double finalTotal = totalPrice - discountAmount;

            
            ViewBag.TotalPrice = totalPrice;
            ViewBag.DiscountPercentage = discountPercentage * 100;
            ViewBag.DiscountAmount = discountAmount;
            ViewBag.FinalTotal = finalTotal;

            return View(selectedOffers);
        }

        
        public IActionResult About()
        {
            return View();
        }
    }
}
