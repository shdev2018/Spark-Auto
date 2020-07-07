using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models;
using SparkAuto.Models.ViewModels;
using SparkAuto.Utility;

namespace SparkAuto.Pages.Users
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
        public UsersListViewModel UserListVM { get; set; }

        public async Task<IActionResult> OnGet(int productPage = 1, string searchName = null, string searchEmail = null, string searchPhone = null)
        {
            UserListVM = new UsersListViewModel()
            {
                ApplicationUserList = await _db.ApplicationUser.OrderBy(p => p.Name).ToListAsync()
            };

            StringBuilder param = new StringBuilder();
            param.Append("/users?productPage=:");
            if (searchEmail != null)
            {
                param.Append("&searchEmail=");
                param.Append(searchEmail);
            }
            if (searchPhone != null)
            {
                param.Append("&searchPhone=");
                param.Append(searchPhone);
            }
            if (searchName != null)
            {
                param.Append("&searchName=");
                param.Append(searchName);
                UserListVM.ApplicationUserList = await _db.ApplicationUser.Where(u => u.Name.ToLower().Contains(searchName.ToLower())).ToListAsync();
            }
            else
            {
                if (searchEmail != null)
                {
                    UserListVM.ApplicationUserList = await _db.ApplicationUser.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower())).ToListAsync();
                }
                else
                {
                    if (searchPhone != null)
                    {
                        UserListVM.ApplicationUserList = await _db.ApplicationUser.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToListAsync();
                    }
                }
            }

            var count = UserListVM.ApplicationUserList.Count;

            UserListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = SD.PaginationUsersPageSize,
                TotalItems = count,
                UrlParam = param.ToString()
            };

            UserListVM.ApplicationUserList = UserListVM.ApplicationUserList.OrderBy(p => p.Name)
                .Skip((productPage - 1) * SD.PaginationUsersPageSize)
                .Take(SD.PaginationUsersPageSize).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostDelete(string id)
        {
            var userFromDb = await _db.ApplicationUser.FindAsync(id);

            if (userFromDb == null)
            {
                return NotFound();
            }

            _db.ApplicationUser.Remove(userFromDb);
            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}