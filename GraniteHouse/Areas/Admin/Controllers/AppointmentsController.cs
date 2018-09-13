using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModel;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public object AppointmentsViewModel { get; private set; }

        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        //Get: Index
        public async Task<IActionResult> Index(string searchName = null, string searchEmail = null, string searchPhone = null, string searchDate = null)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            AppointmentViewModel appointmentVM = new AppointmentViewModel()
            {
                Appointments = new List<Appointments>()
            };

            appointmentVM.Appointments = _db.Appointments.Include(a => a.SalesPerson).ToList();

            if (User.IsInRole(SD.AdminEndUser))
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.SalesPersonId == claim.Value).ToList();
            }

            if (searchName != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();
            }

            if (searchEmail != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }

            if (searchPhone != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerPhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToList();
            }

            if (searchDate != null)
            {
                try
                {
                    DateTime appDate = Convert.ToDateTime(searchDate);
                    appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.AppointmentDate.ToShortDateString().Equals(appDate.ToShortDateString())).ToList();
                }
                catch (Exception ex)
                {

                }
            }

            return View(appointmentVM);
        }

        //Get: Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in _db.Products
                                                      join a in _db.ProductsSelectedForAppointment
                                                      on p.Id equals a.ProductId
                                                      where a.AppointmentId == id
                                                      select p).Include("ProductTypes");

            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {
                Appointment = _db.Appointments.Include(s => s.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationUsers.Where(u => u.LockoutEnd == null || u.LockoutEnd < DateTime.Now).ToList(),
                Products = productList.ToList()
            };


            return View(objAppointmentVM);
        }

        //Post: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentDetailsViewModel objAppointmentVM)
        {
            if (ModelState.IsValid)
            {
                objAppointmentVM.Appointment.AppointmentDate = objAppointmentVM.Appointment.AppointmentDate
                                                                .AddHours(objAppointmentVM.Appointment.AppointmentTime.Hour)
                                                                .AddMinutes(objAppointmentVM.Appointment.AppointmentTime.Minute);

                var appointmentFromDb = _db.Appointments.Where(a => a.Id == objAppointmentVM.Appointment.Id).FirstOrDefault();

                appointmentFromDb.CustomerName = objAppointmentVM.Appointment.CustomerName;
                appointmentFromDb.CustomerEmail = objAppointmentVM.Appointment.CustomerEmail;
                appointmentFromDb.AppointmentDate = objAppointmentVM.Appointment.AppointmentDate;
                appointmentFromDb.CustomerPhoneNumber = objAppointmentVM.Appointment.CustomerPhoneNumber;
                appointmentFromDb.IsConfirmed = objAppointmentVM.Appointment.IsConfirmed;

                if (User.IsInRole(SD.SuperAdminEndUser))
                {
                    appointmentFromDb.SalesPersonId = objAppointmentVM.Appointment.SalesPersonId;
                }
                else
                {
                    appointmentFromDb.SalesPersonId = "";
                }

                _db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productList = (IEnumerable<Products>)(from p in _db.Products
                                                          join a in _db.ProductsSelectedForAppointment
                                                          on p.Id equals a.ProductId
                                                          where a.AppointmentId == id
                                                          select p).Include("ProductTypes");

                objAppointmentVM.SalesPerson = _db.ApplicationUsers.Where(u => u.LockoutEnd == null || u.LockoutEnd < DateTime.Now).ToList();
                objAppointmentVM.Products = productList.ToList();
            }

            return View(objAppointmentVM);
        }

        //Get: Details
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in _db.Products
                                                      join a in _db.ProductsSelectedForAppointment
                                                      on p.Id equals a.ProductId
                                                      where a.AppointmentId == id
                                                      select p).Include("ProductTypes");

            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {
                Appointment = _db.Appointments.Include(s => s.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationUsers.Where(u => u.LockoutEnd == null || u.LockoutEnd < DateTime.Now).ToList(),
                Products = productList.ToList()
            };


            return View(objAppointmentVM);
        }
    }
}