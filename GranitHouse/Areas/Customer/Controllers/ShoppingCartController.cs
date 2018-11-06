using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GranitHouse.Data;
using GranitHouse.Extenstions;
using GranitHouse.Models;
using GranitHouse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GranitHouse.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCartVM = new ShoppingCartViewModel
            {
                Products = new List<Models.Products>()
            };
        }

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public async Task<IActionResult> Index()
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstShoppingCart.Count > 0)
            {
                foreach (int cartItem in lstShoppingCart)
                {
                    Products product = _db.Products.Include(p => p.SpecialTagTypes).Include(p => p.ProductTypes).Where(p => p.Id == cartItem).FirstOrDefault();
                    ShoppingCartVM.Products.Add(product);
                }
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            List<int> lstCarItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            ShoppingCartVM.Appointments.AppointmentDate = ShoppingCartVM.Appointments.AppointmentDate
                                                            .AddHours(ShoppingCartVM.Appointments.AppointmentTime.Hour)
                                                            .AddMinutes(ShoppingCartVM.Appointments.AppointmentTime.Minute);

            Appointments appointments = ShoppingCartVM.Appointments;
            _db.Appointments.Add(appointments);
            _db.SaveChanges();

            int appointmentId = appointments.Id;

            foreach (int productId in lstCarItems)
            {
                ProductSelectedForAppointment productSelectedForAppointment = new ProductSelectedForAppointment()
                {
                    AppointmentId = appointmentId,
                    ProductId = productId
                };

                _db.ProductSelectedForAppointments.Add(productSelectedForAppointment);
            }

            _db.SaveChanges();
            lstCarItems = new List<int>();
            HttpContext.Session.Set("ssShoppingCart", lstCarItems);

            return RedirectToAction("AppoinemtConfirmation", "ShoppingCart", new { Id = appointmentId });
        }

        public IActionResult Remove(int id)
        {
            List<int> lstCarItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if (lstCarItems.Count > 0)
            {
                if (lstCarItems.Contains(id))
                {
                    lstCarItems.Remove(id);
                }
            }

            HttpContext.Session.Set("ssShoppingCart", lstCarItems);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AppoinemtConfirmation(int id)
        {
            ShoppingCartVM.Appointments = _db.Appointments.Where(a => a.Id == id).FirstOrDefault();
            List<ProductSelectedForAppointment> objProducts = _db.ProductSelectedForAppointments.Where(p => p.AppointmentId == id).ToList();

            foreach (ProductSelectedForAppointment prodAptObj in objProducts)
            {
                ShoppingCartVM.Products.Add(_db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTagTypes).Where(p => p.Id == prodAptObj.ProductId).FirstOrDefault());
            }

            return View(ShoppingCartVM);
        }
    }
}