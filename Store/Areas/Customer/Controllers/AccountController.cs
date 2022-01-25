using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Base.Models;
using Store.Base.Models.ViewModels;
using Store.Base.Utility;
using Store.DataAccess.Data;

namespace Store.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;


        public VendorRegisterViewModel VendorRegisterVM { get; set; }

        public MarketerRegisterViewModel MarketerRegisterVM { get; set; }



        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            VendorRegisterVM = new VendorRegisterViewModel()
            {
                countries = _db.countries.ToList()
            };
            MarketerRegisterVM = new MarketerRegisterViewModel()
            {
                countries = _db.countries.ToList()
            };
        }


        [HttpGet]
        public IActionResult VendorRegister()
        {
            return View(VendorRegisterVM);
        }


        [HttpPost, ActionName("VendorRegister")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VendorRegisterPost(VendorRegisterViewModel VendorRegisterVM)
        {
            if (ModelState.IsValid)
            {
                //check if the email is user
                if (_userManager.Users.Where(x => x.Email == VendorRegisterVM.Email).Any())
                {
                    ModelState.AddModelError(string.Empty, "The email " + VendorRegisterVM.Email + " is already used");
                    VendorRegisterVM.countries = _db.countries.ToList();
                    return View(VendorRegisterVM);
                }


                var user = new ApplicationUser { Email = VendorRegisterVM.Email, UserName = VendorRegisterVM.Username, FullName = VendorRegisterVM.FullName, PhoneNumber = VendorRegisterVM.PhoneNumber, LockoutEnd = DateTime.Now.AddYears(1000) };
                var result = await _userManager.CreateAsync(user, VendorRegisterVM.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user,SD.VendorEndUser);
                    var vendor = new Vendor() { User = user, OrganizationAndCompany = VendorRegisterVM.OrganizationAndCompany, WebsiteURL =VendorRegisterVM.WebsiteURL, Address = VendorRegisterVM.Address, City = VendorRegisterVM.City, CountryId = VendorRegisterVM.CountryId, ZIP = VendorRegisterVM.ZIP };
                    _db.Vendors.Add(vendor);

                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Lockout));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            VendorRegisterVM.countries = _db.countries.ToList();
            return View(VendorRegisterVM);
        }



        [HttpGet]
        public IActionResult MarketerRegister()
        {
            return View(MarketerRegisterVM);
        }


        [HttpPost, ActionName("MarketerRegister")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarketerRegisterPost(MarketerRegisterViewModel MarketerRegisterVM)
        {
            if (ModelState.IsValid)
            {
                //check if the email is user
                if (_userManager.Users.Where(x => x.Email == MarketerRegisterVM.Email).Any())
                {
                    ModelState.AddModelError(string.Empty, "The email " + MarketerRegisterVM.Email + " is already used");
                    MarketerRegisterVM.countries = _db.countries.ToList();
                    return View(MarketerRegisterVM);
                }


                var user = new ApplicationUser { Email = MarketerRegisterVM.Email, UserName = MarketerRegisterVM.Username, FullName = MarketerRegisterVM.FullName};
                var result = await _userManager.CreateAsync(user, MarketerRegisterVM.Password);
                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(user, SD.MarketerEndUser);
                    var marketer = new Marketer() { User = user, CountryId = MarketerRegisterVM.CountryId, WebsiteURL = MarketerRegisterVM.WebsiteURL, Facebook = MarketerRegisterVM.Facebook, Twitter = MarketerRegisterVM.Twitter, Instagram = MarketerRegisterVM.Instagram, YouTube = MarketerRegisterVM.YouTube, Other = MarketerRegisterVM.Other };
                    _db.Marketers.Add(marketer);

                    await _db.SaveChangesAsync();
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            MarketerRegisterVM.countries = _db.countries.ToList();
            return View(MarketerRegisterVM);
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel LoginUser)
        {
            //if the entered value of the username is an email >> pass the username value
            var user = await _userManager.FindByEmailAsync(LoginUser.Username);
            if (user != null)
            {
                LoginUser.Username = user.UserName;
            }
            var result = await _signInManager.PasswordSignInAsync(LoginUser.Username, LoginUser.Password, true, false);
            if(result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }
        }

        public IActionResult Lockout()
        {
            return View();
        }

        public IActionResult Warning()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}