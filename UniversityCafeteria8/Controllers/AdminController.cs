using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversityCafeteria8.Data;

namespace UniversityCafeteria8.Controllers
{
    // Requirement: Only SuperAdmin can access this controller.
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Display all registered users and roles
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Users = _userManager.Users.ToList();

            return View();
        }

        // نعرض الدور الحالي والأدوار المتاحة بصفحة التعديل.
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            ViewBag.CurrentRoles = string.Join(", ", await _userManager.GetRolesAsync(user));
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View(user);
        }

        // نستبدل الدور القديم بالجديد ونحفظ العمليتين مع بعض.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string roleId)
        {
            if (!ModelState.IsValid) return BadRequest("Select a valid role.");

            var user = await _userManager.FindByIdAsync(id);
            var role = await _roleManager.FindByIdAsync(roleId);
            if (user == null || role?.Name == null) return NotFound();

            await using var transaction = await _context.Database.BeginTransactionAsync();
            var currentRoles = await _userManager.GetRolesAsync(user);

            if (!currentRoles.Contains(role.Name))
            {
                var added = await _userManager.AddToRoleAsync(user, role.Name);
                if (!added.Succeeded) return BadRequest("Could not add the selected role.");
            }

            var oldRoles = currentRoles.Where(name => name != role.Name).ToList();
            if (oldRoles.Count > 0)
            {
                var removed = await _userManager.RemoveFromRolesAsync(user, oldRoles);
                if (!removed.Succeeded) return BadRequest("Could not remove the previous roles.");
            }

            await transaction.CommitAsync();
            return RedirectToAction("Index");
        }

        // Delete confirmation page
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // Allow SuperAdmin to delete users
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
    }
}
