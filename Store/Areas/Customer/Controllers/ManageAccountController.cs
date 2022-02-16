using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Base.Models;
using Store.Base.Models.ViewModels;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.Interfaces;

namespace Store.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ManageAccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IVendorRepository _vendorRepository;
        private readonly IMarketerRepository _marketerRepository;
        private readonly ApplicationDbContext _db;


        public UpdateVendorProfileViewModel UpdateVendorVM { get; set; }

        public UpdateMarketerProfileViewModel UpdateMarketerVM { get; set; }

        public ManageAccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IVendorRepository vendorRepository, ApplicationDbContext db, IMarketerRepository marketerRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _vendorRepository = vendorRepository;
            _marketerRepository = marketerRepository;
            _db = db;

            UpdateVendorVM = new UpdateVendorProfileViewModel()
            {
                Countries = _db.countries.ToList(),
                UpdateVendorProfileRequest = new UpdateVendorProfileRequest()
            };
            UpdateMarketerVM = new UpdateMarketerProfileViewModel()
            {
                countries = _db.countries.ToList()
            };
        }

        [HttpGet]
        public async Task<IActionResult> UpdateVendorProfile()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var vendor = await _vendorRepository.FindVendor(claim.Value);
            if(vendor == null)
            {
                return NotFound();
            }
            UpdateVendorVM.UpdateVendorProfileRequest.FullName = vendor.User.FullName;
            UpdateVendorVM.UpdateVendorProfileRequest.PhoneNumber = vendor.User.PhoneNumber;
            UpdateVendorVM.UpdateVendorProfileRequest.OrganizationAndCompany = vendor.OrganizationAndCompany;
            UpdateVendorVM.UpdateVendorProfileRequest.WebsiteURL = vendor.WebsiteURL;
            UpdateVendorVM.UpdateVendorProfileRequest.Address = vendor.Address;
            UpdateVendorVM.UpdateVendorProfileRequest.City = vendor.City;
            UpdateVendorVM.UpdateVendorProfileRequest.CountryId = vendor.CountryId;
            UpdateVendorVM.UpdateVendorProfileRequest.ZIP = vendor.ZIP;


            return View(UpdateVendorVM);
        }

        [HttpPost]
        public IActionResult UpdateVendorProfile(UpdateVendorProfileViewModel UpdateVendorVM)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                UpdateVendorVM.UpdateVendorProfileRequest.Id = claim.Value;
                //if the request is exist before >> update it
                if (_db.UpdateVendorProfileRequests.Where(x => x.Id == UpdateVendorVM.UpdateVendorProfileRequest.Id).Any())
                {
                    _db.UpdateVendorProfileRequests.Update(UpdateVendorVM.UpdateVendorProfileRequest);
                    _db.SaveChanges();

                    TempData["state"] = 1;
                    TempData["Message"] = "Updated successfully, Update profile request sent to admin, wait for approval";
                    return RedirectToAction("Index", "Home");
                }


                _db.UpdateVendorProfileRequests.Add(UpdateVendorVM.UpdateVendorProfileRequest);
                _db.SaveChanges();

                TempData["state"] = 1;
                TempData["Message"] = "Updated successfully, Update profile request sent to admin, wait for approval";
                return RedirectToAction("Index", "Home");
            }
            return View(UpdateVendorVM);
        }



        [HttpGet]
        public async Task<IActionResult> UpdateMarketerProfile()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var marketer = await _marketerRepository.FindMarketer(claim.Value);
            if (marketer == null)
            {
                return NotFound();
            }
            UpdateMarketerVM.FullName = marketer.User.FullName;
            UpdateMarketerVM.CountryId = marketer.CountryId;
            UpdateMarketerVM.WebsiteURL = marketer.WebsiteURL;
            UpdateMarketerVM.Facebook = marketer.Facebook;
            UpdateMarketerVM.Twitter = marketer.Twitter;
            UpdateMarketerVM.Instagram = marketer.Instagram;
            UpdateMarketerVM.YouTube = marketer.YouTube;
            UpdateMarketerVM.Other = marketer.Other;


            return View(UpdateMarketerVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMarketerProfile(UpdateMarketerProfileViewModel updateMarketerVM)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                var marketer = await _marketerRepository.FindMarketer(claim.Value);

                marketer.User.FullName = updateMarketerVM.FullName;
                marketer.CountryId = updateMarketerVM.CountryId;
                marketer.WebsiteURL = updateMarketerVM.WebsiteURL;
                marketer.Facebook = updateMarketerVM.Facebook;
                marketer.Twitter = updateMarketerVM.Twitter;
                marketer.Instagram = updateMarketerVM.Instagram;
                marketer.YouTube = updateMarketerVM.YouTube;
                marketer.Other = updateMarketerVM.Other;

                await _marketerRepository.Update(marketer);

                TempData["state"] = 1;
                TempData["Message"] = "Updated successfully";
                return RedirectToAction("Index", "Home");
            }
            return View(updateMarketerVM);
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel ChangePasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, ChangePasswordVM.OldPassword, ChangePasswordVM.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            await _signInManager.RefreshSignInAsync(user);
            //StatusMessage = "Your password has been changed.";

            TempData["state"] = 1;
            TempData["Message"] = "Your password has been changed.";
            return RedirectToAction("Index", "Home");
        }
    }
}