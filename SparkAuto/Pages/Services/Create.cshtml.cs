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
using SparkAuto.Models.ViewModels;
using SparkAuto.Utility;

namespace SparkAuto.Pages.Services
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public CarServiceViewModel CarServiceVM { get; set; }

        public async Task<IActionResult> OnGet(int carId)
        {
            CarServiceVM = new CarServiceViewModel
            {
                Car = await _db.Car.Include(c => c.ApplicationUser).FirstOrDefaultAsync(c => c.Id == carId),
                ServiceHeader = new Models.ServiceHeader()
            };

            List<string> listServiceTypeInShoppingCart = _db.ServiceShoppingCart
                                                            .Include(c => c.ServiceType)
                                                            .Where(c => c.CarId == carId)
                                                            .Select(c => c.ServiceType.Name)
                                                            .ToList();

            IQueryable<ServiceType> listService = from s in _db.ServiceType
                                                  where !(listServiceTypeInShoppingCart.Contains(s.Name))
                                                  select s;
            CarServiceVM.ServiceTypeList = listService.ToList();

            CarServiceVM.ServiceShoppingCart = _db.ServiceShoppingCart
                                                    .Include(c => c.ServiceType)
                                                    .Where(c => c.CarId == carId)
                                                    .ToList();

            CarServiceVM.ServiceHeader.TotalPrice = 0;
            foreach (var item in CarServiceVM.ServiceShoppingCart)
            {
                CarServiceVM.ServiceHeader.TotalPrice += item.ServiceType.Price;
            }

            return Page();                                                
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                CarServiceVM.ServiceHeader.MyProperty = DateTime.Now;
                CarServiceVM.ServiceShoppingCart = _db.ServiceShoppingCart.Include(c => c.ServiceType).Where(s => s.CarId == CarServiceVM.Car.Id).ToList();
                CarServiceVM.ServiceHeader.CarId = CarServiceVM.Car.Id;
                foreach (var item in CarServiceVM.ServiceShoppingCart)
                {
                    CarServiceVM.ServiceHeader.TotalPrice += item.ServiceType.Price;
                }

                _db.ServiceHeader.Add(CarServiceVM.ServiceHeader);
                await _db.SaveChangesAsync();

                foreach (var detail in CarServiceVM.ServiceShoppingCart)
                {
                    ServiceDetails serviceDetails = new ServiceDetails
                    {
                        ServiceHeaderId = CarServiceVM.ServiceHeader.Id,
                        ServiceName = detail.ServiceType.Name,
                        ServicePrice = detail.ServiceType.Price,
                        ServiceTypeId = detail.ServiceId
                    };
                    _db.ServiceDetail.Add(serviceDetails);
                }
                _db.ServiceShoppingCart.RemoveRange(CarServiceVM.ServiceShoppingCart);
                await _db.SaveChangesAsync();

                return RedirectToPage("../Cars/Index", new { userId = CarServiceVM.Car.UserId });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCart()
        {
            ServiceShoppingCart objServiceCart = new ServiceShoppingCart()
            {
                CarId = CarServiceVM.Car.Id,
                ServiceId = CarServiceVM.ServiceDetails.ServiceTypeId
            };

            _db.ServiceShoppingCart.Add(objServiceCart);
            await _db.SaveChangesAsync();
            return RedirectToPage("Create", new { carId = CarServiceVM.Car.Id});
        }

        public async Task<IActionResult> OnPostRemoveFromCart(int serviceId)
        {
            ServiceShoppingCart objServiceCart = _db.ServiceShoppingCart
                .FirstOrDefault(u => u.CarId == CarServiceVM.Car.Id && u.ServiceId == serviceId);

            _db.ServiceShoppingCart.Remove(objServiceCart);
            await _db.SaveChangesAsync();
            return RedirectToPage("Create", new { carId = CarServiceVM.Car.Id });
        }
    }
}