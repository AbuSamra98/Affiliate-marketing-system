using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Base.Models;
using Store.Base.Utility;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.Interfaces;

namespace Store.Areas.Vendor.Controllers
{
    [Area("Vendor")]
    [Authorize(Roles = SD.VendorEndUser)]
    public class VendorCheckoutController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IVendorRepository _vendorRepository;


        public VendorCheckoutController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IVendorRepository vendorRepository)
        {
            _userManager = userManager;
            _db = db;
            _vendorRepository = vendorRepository;

        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var vendor = await _vendorRepository.Find(claim.Value);
            if (vendor == null)
            {
                return NotFound();
            }

            ViewData["VendorId"] = vendor.VendorId;
            ViewData["PointValue"] = _db.PointSettings.FirstOrDefault().PointValue;

            return View();
        }

        public async Task<IActionResult> Checkout(string ordreId, DateTime create_time, string status, float value)
        {
            if(!status.Equals("COMPLETED"))
            {
                return Json(0);
            }

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            var vendor = await _vendorRepository.Find(claim.Value);
            //if (vendor == null)
            //{
            //    return NotFound();
            //}
            var pointValue = _db.PointSettings.FirstOrDefault().PointValue;

            

            var addedPoints = (int)(value / pointValue);
            vendor.Points += addedPoints;

            var order = new CheckoutOrder()
            {
                VendorId = claim.Value,
                OrderId = ordreId,
                Total = value,
                PointValue = pointValue,
                AddedPoints = addedPoints,
                CreateDate = create_time
            };
            _db.CheckoutOrders.Add(order);

            await _db.SaveChangesAsync();

            return Json(1);
        }
    }
}