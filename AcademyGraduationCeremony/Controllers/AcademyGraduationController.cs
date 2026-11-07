using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using AcademyGraduationCeremony.Models;

namespace AcademyGraduationCeremony.Controllers
{
    public class AcademyGraduationController : Controller
    {
        private readonly AppDbContext _context;

        public AcademyGraduationController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (!_context.Categories.Any())
            {
                SeedData(); 
            }

            var offers = _context.Offers
                .Include(o => o.Supplier)
                .ThenInclude(s => s.Category)
                .ToList();

            return View(offers);
        }

        public IActionResult SelectOffer(int id, int categoryId)
        {
            var sessionData = HttpContext.Session.GetString("SelectedOffersList");
            List<Offer> selectedOffers = string.IsNullOrEmpty(sessionData)
                 ? new List<Offer>()
                 : JsonSerializer.Deserialize<List<Offer>>(sessionData);

            var existingOffer = selectedOffers.FirstOrDefault(o => o.CategoryId == categoryId);
            if (existingOffer != null)
            {
                selectedOffers.Remove(existingOffer);
            }

            var newOffer = _context.Offers
                .Include(o => o.Supplier)
                .ThenInclude(s => s.Category)
                .FirstOrDefault(o => o.Id == id);

            if (newOffer != null)
            {
                selectedOffers.Add(newOffer);
            }

            HttpContext.Session.SetString("SelectedOffersList", JsonSerializer.Serialize(selectedOffers));

            TempData["Message"] = "Offer selected successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult Review()
        {
            var sessionData = HttpContext.Session.GetString("SelectedOffersList");
            List<Offer> selectedOffers = string.IsNullOrEmpty(sessionData)
                ? new List<Offer>()
                : JsonSerializer.Deserialize<List<Offer>>(sessionData);

            if (selectedOffers.Count < 3)
            {
                TempData["Error"] = "You must select one offer from ALL 3 categories before reviewing.";
                return RedirectToAction("Index");
            }

            var selectedIds = selectedOffers.Select(o => o.Id).ToList();
            var fullOffers = _context.Offers
                .Include(o => o.Supplier)
                .ThenInclude(s => s.Category)
                .Where(o => selectedIds.Contains(o.Id))
                .ToList();

            double totalPrice = 0;
            foreach (var offer in fullOffers) 
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

          
            return View(fullOffers);
        }

        public IActionResult About()
        {
            return View();
        }

        private void SeedData()
        {
            var cat1 = new Category { Name = "Graduation Certificates" };
            var cat2 = new Category { Name = "Hospitality" };
            var cat3 = new Category { Name = "Graduation Gifts" };
            _context.Categories.AddRange(cat1, cat2, cat3);
            _context.SaveChanges();

            var supA = new Supplier { Name = "Supplier A", Category = cat1 };
            var supB = new Supplier { Name = "Supplier B", Category = cat1 };
            var supC = new Supplier { Name = "Supplier C", Category = cat1 };
            var supD = new Supplier { Name = "Supplier D", Category = cat2 };
            var supE = new Supplier { Name = "Supplier E", Category = cat2 };
            var supF = new Supplier { Name = "Supplier F", Category = cat2 };
            var supG = new Supplier { Name = "Supplier G", Category = cat3 };
            var supH = new Supplier { Name = "Supplier H", Category = cat3 };
            var supI = new Supplier { Name = "Supplier I", Category = cat3 };
            _context.Suppliers.AddRange(supA, supB, supC, supD, supE, supF, supG, supH, supI);
            _context.SaveChanges();

            _context.Offers.AddRange(
                new Offer { OfferName = "Premium Certificate Package", Quantity = 35, UnitPrice = 2, Supplier = supA },
                new Offer { OfferName = "Standard Certificate Package", Quantity = 35, UnitPrice = 1.5, Supplier = supB },
                new Offer { OfferName = "Luxury Certificate Package", Quantity = 35, UnitPrice = 2.5, Supplier = supC },
                new Offer { OfferName = "Coffee & Drinks Package", Quantity = 100, UnitPrice = 2, Supplier = supD },
                new Offer { OfferName = "Sandwiches & Snacks Package", Quantity = 100, UnitPrice = 3, Supplier = supE },
                new Offer { OfferName = "Full Hospitality Package", Quantity = 100, UnitPrice = 4, Supplier = supF },
                new Offer { OfferName = "Customized Mugs Package", Quantity = 35, UnitPrice = 2, Supplier = supG },
                new Offer { OfferName = "Graduation Bags Package", Quantity = 35, UnitPrice = 3, Supplier = supH },
                new Offer { OfferName = "Premium Gift Boxes Package", Quantity = 35, UnitPrice = 4, Supplier = supI }
            );
            _context.SaveChanges();
        }
    }
}