using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityCafeteria8.Data;
using UniversityCafeteria8.Models;

namespace UniversityCafeteria8.Controllers
{
    [Authorize] 
    public class SupplyRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SupplyRequestController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // الإدارة بتشوف كل الطلبات، والمستخدم بشوف طلباته فقط.
            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
            {
                var allRequests = _context.SupplyRequests.Include(s => s.User).ToList();
                return View(allRequests);
            }

            var userId = _userManager.GetUserId(User);
            var myRequests = _context.SupplyRequests.Where(r => r.UserId == userId).ToList();
            return View(myRequests);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Products = _context.Products.ToList();
            return View();
        }

        // كل منتج إله كمية، والكمية صفر معناها إنه مش مطلوب.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int[] productId, int[] quantity)
        {
            var products = _context.Products.ToList();
            if (!ModelState.IsValid || productId.Length != quantity.Length ||
                productId.Distinct().Count() != productId.Length ||
                quantity.Any(q => q < 0) || !quantity.Any(q => q > 0))
            {
                ViewBag.Error = "Enter valid quantities and select at least one product.";
                ViewBag.Products = products;
                return View();
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();
            var newRequest = new SupplyRequest
            {
                RequestDate = DateTime.Now,
                Status = "Pending",
                UserId = userId,
                RequestItems = new List<RequestItem>()
            };

            // ناخذ السعر من قاعدة البيانات ونحسب مجموع كل المنتجات المطلوبة.
            for (var i = 0; i < productId.Length; i++)
            {
                if (quantity[i] == 0) continue;

                var product = products.Find(p => p.Id == productId[i]);
                if (product == null)
                {
                    ViewBag.Error = "A selected product is no longer available.";
                    ViewBag.Products = products;
                    return View();
                }

                newRequest.RequestItems.Add(new RequestItem
                {
                    ProductId = product.Id,
                    Quantity = quantity[i],
                    Price = product.Price
                });
                newRequest.TotalPrice += product.Price * quantity[i];
            }

            // نحفظ الطلب وكل عناصره مع بعض.
            _context.SupplyRequests.Add(newRequest);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var request = _context.SupplyRequests
                .Include(r => r.User)
                .Include(r => r.RequestItems)
                .ThenInclude(ri => ri.Product)
                .FirstOrDefault(r => r.Id == id);

            if (request == null) return NotFound();

            // صاحب الطلب والإدارة فقط بقدروا يشوفوا التفاصيل.
            if (request.UserId != _userManager.GetUserId(User) &&
                !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                return Forbid();
            }

            return View(request);
        }

        // تعديل حالة الطلب وحذفه من صلاحيات الإدارة فقط.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var request = _context.SupplyRequests.Find(id);
            if (request == null) return NotFound();

            return View(request);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string status)
        {
            var requestInDb = _context.SupplyRequests.Find(id);
            if (requestInDb == null) return NotFound();

            // القرار المسموح من الإدارة هو الموافقة أو الرفض.
            if (status != "Approved" && status != "Rejected")
            {
                ViewBag.Error = "Select Approved or Rejected.";
                return View(requestInDb);
            }

            requestInDb.Status = status;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var request = _context.SupplyRequests.Find(id);
            if (request == null) return NotFound();

            return View(request);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var request = _context.SupplyRequests.Find(id);
            if (request != null)
            {
                _context.SupplyRequests.Remove(request);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
