using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blogs.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Store.Base.Models;
using Store.Base.Utility;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.Interfaces;

namespace Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "ApproveVendorsPolicy")]
    public class VendorController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVendorRepository _vendorRepository;
        private readonly ApplicationDbContext _db;
        private readonly ICampaignRepository _campaignRepository;

        public VendorController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IVendorRepository vendorRepository, ICampaignRepository campaignRepository, IHubContext<NotificationHub> hub)
        {
            _userManager = userManager;
            _db = db;
            _vendorRepository = vendorRepository;
            _campaignRepository = campaignRepository;
            _hub = hub;
        }


        private IHubContext<NotificationHub> _hub { get; set; }


        //get all vendors
        public async Task<IActionResult> Index()
        {
            return View(await _vendorRepository.GetAllVendors());
        }


        public async Task<IActionResult> NotApprovedVendors()
        {
            return View(await _vendorRepository.GetNotApprovedVendors());
        }

        public async Task<IActionResult> Details(string id)
        {
            var vendor = await _db.Vendors.Include(x => x.User).Include(x => x.Country).SingleOrDefaultAsync(x => x.VendorId == id);
            if(vendor == null)
            {
                return NotFound();
            }

            return View(vendor);
        }

        //for the Marketer to show his profile
        [AllowAnonymous]
        public async Task<IActionResult> DetailsForVendor()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var vendor = await _db.Vendors.Include(x => x.User).Include(x => x.Country).SingleOrDefaultAsync(x => x.VendorId == claim.Value);
            if (vendor == null)
            {
                return NotFound();
            }

            return View("Details", vendor);
        }

        [HttpPost]
        public async Task<IActionResult> Lock(string userName)
        {
            if (userName == null)
            {
                return NotFound();
            }

            var vendor = await _userManager.FindByNameAsync(userName);

            if (vendor == null)
            {
                return NotFound();
            }

            //check if the user is a vendor
            if (!(await _userManager.IsInRoleAsync(vendor, SD.VendorEndUser)))
            {
                return NotFound();
            }

            if (vendor.LockoutEnd == null || vendor.LockoutEnd < DateTime.Now)       
            {
                vendor.LockoutEnd = DateTime.Now.AddYears(1000);        //Lock
                await _db.SaveChangesAsync();

                return Json(1);
            }
            else
            {
                vendor.LockoutEnd = DateTime.Now;       //unLock
                await _db.SaveChangesAsync();

                return Json(2);
            }

            
        }


        //get all Requests for update profile
        public IActionResult UpdateProfileRequests()
        {
            return View(_db.UpdateVendorProfileRequests.Include(x => x.country).Include(x => x.User).ToList());
        }

        public async Task<IActionResult> ApproveUpdateProfileRequests(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var vendor = await _vendorRepository.FindVendor(id);
            if(vendor == null)
            {
                return NotFound();
            }
            var updateProfile = await _db.UpdateVendorProfileRequests.FindAsync(id);
            if (updateProfile == null)
            {
                return NotFound();
            }

            //update profile
            vendor.User.FullName = updateProfile.FullName;
            vendor.User.PhoneNumber = updateProfile.PhoneNumber;
            vendor.OrganizationAndCompany = updateProfile.OrganizationAndCompany;
            vendor.WebsiteURL = updateProfile.WebsiteURL;
            vendor.CountryId = updateProfile.CountryId;
            vendor.City = updateProfile.City;
            vendor.Address = updateProfile.Address;
            vendor.ZIP = updateProfile.ZIP;

            await _vendorRepository.Update(vendor);
            updateProfile.User = null;
            var result = _db.UpdateVendorProfileRequests.Remove(updateProfile);
            _db.SaveChanges();

            //send notification for vendor
            await _hub.Clients.User(vendor.VendorId).SendAsync("ReceiveMessage", 1, "Your update profile request is approved");

            TempData["state"] = 1;
            TempData["Message"] = "The vendor " + vendor.User.UserName + "'s profile request approved successfully";
            return RedirectToAction(nameof(UpdateProfileRequests));
        }

    }
}