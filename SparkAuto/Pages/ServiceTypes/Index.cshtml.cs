using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models;
using SparkAuto.Utility;

namespace SparkAuto.Pages.ServiceTypes
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public IList<ServiceType> ServiceType { get; set; }

        public async Task<IActionResult> OnGet()
        {
            ServiceType = await _db.ServiceType.OrderBy(p => p.Name).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDelete(int? id)
        {
            var ServiceTypeFromDb = await _db.ServiceType.FindAsync(id);

            if (ServiceTypeFromDb == null)
            {
                return NotFound();
            }

            _db.ServiceType.Remove(ServiceTypeFromDb);
            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}