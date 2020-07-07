using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Email;

namespace SparkAuto.Areas.Identity.Pages.Account
{
    public class VerifyEmailModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailSender _emailSender;

        public VerifyEmailModel(
            ApplicationDbContext db, 
            UserManager<IdentityUser> userManager, 
            EmailSender emailSender
        )
        {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public string Email { get; set; }

        public IActionResult OnGet(string id)
        {
            Email = id;
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string id)
        {
            var userFromDb = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Email == id);

            if (userFromDb != null)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(userFromDb);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = userFromDb.Id, code = code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(userFromDb.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToPage("VerifyEmail", new { id = id });
            }

            return NotFound();

        }

    }
}