using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models.ViewModels;

namespace SparkAuto.Pages.Cars
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public CarAndCustomerViewModel CarAndCustVM { get; set; }

        [TempData]
        public string statusMessage { get; set; }

        public async Task<IActionResult> OnGet(string userId = null)
        {
            if (userId == null)
            {
                var _claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = _claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                userId = claim.Value;
            }

            CarAndCustVM = new CarAndCustomerViewModel()
            {
                Cars = await _db.Car.Where(c => c.UserId == userId).ToListAsync(),
                UserObj = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId)
            };

            return Page();
        }

        public async Task<IActionResult> OnPostDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carFromDb = await _db.Car.FindAsync(id);
            if (carFromDb == null)
            {
                return NotFound();
            }
            _db.Car.Remove(carFromDb);
            await _db.SaveChangesAsync();
            statusMessage = "Car deleted sucessfully";
            return RedirectToPage("Index", new { userId = carFromDb.UserId });
        }
    }
}