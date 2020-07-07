using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models;

namespace SparkAuto.Pages.Cars
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Car Car { get; set; }

        [TempData]
        public string statusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            Car = await _db.Car.FirstOrDefaultAsync(c => c.Id == id);
            
            if (Car == null)
            {
                return NotFound();
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var carFromDb = await _db.Car.FirstOrDefaultAsync(c => c.Id == id);
            carFromDb.Make = Car.Make;
            carFromDb.Model = Car.Model;
            carFromDb.VIN = Car.VIN;
            carFromDb.Colour = Car.Colour;
            carFromDb.Style = Car.Style;
            carFromDb.Year = Car.Year;
            carFromDb.Miles = Car.Miles;

            await _db.SaveChangesAsync();
            statusMessage = "Car edited successfully.";
            return RedirectToPage("./Index", new { userId = carFromDb.UserId });
        }
    }
}
