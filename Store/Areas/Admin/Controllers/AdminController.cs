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
using Store.Base.Utility;
using Store.DataAccess.Data;

namespace Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AccessAdminsPolicy")]
    public class AdminController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;

        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.Name);

            var s = claim.Value;

            //get role id for admin role//
            var roleId = _roleManager.Roles.Where(x => x.Name == SD.AdminEndUser).FirstOrDefault().Id;

            //get all admin Ids
            var AdminsId = _db.UserRoles.Where(x => x.RoleId == roleId).ToList().Select(x => x.UserId);

            //get all admins
            var Admins = _userManager.Users.Where(x => AdminsId.Contains(x.Id)).ToList();

            return View(Admins);
        }

        public async Task<IActionResult> Lock(string userName)
        {
            if (userName == null)
            {
                return NotFound();
            }

            var admin = await _userManager.FindByNameAsync(userName);

            if (admin == null)
            {
                return NotFound();
            }
            //check if the user is an admin
            if(!(await _userManager.IsInRoleAsync(admin,SD.AdminEndUser)))
            {
                return NotFound();
            }

            if (admin.LockoutEnd == null || admin.LockoutEnd < DateTime.Now)
            {
                admin.LockoutEnd = DateTime.Now.AddYears(1000);        //Lock
                await _db.SaveChangesAsync();

                return Json(1);
            }
            else
            {
                admin.LockoutEnd = DateTime.Now;       //unLock
                await _db.SaveChangesAsync();

                return Json(2);
            }

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //create new admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AdminRegisterViewModel RegisterUser)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { Email = RegisterUser.Email, UserName = RegisterUser.Username, FullName = RegisterUser.FullName};
                var result = await _userManager.CreateAsync(user, RegisterUser.Password);
                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(user, SD.AdminEndUser);

                    TempData["state"] = 1;
                    TempData["Message"] = "Create new admin successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View();
        }

        //edit roles and claims for admin
        [HttpGet]
        public async Task<IActionResult> EditRolesInUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //get the admin that I want to edit his roles
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }

            var userRole = new UserRoleViewModel();
            userRole.UserId = user.Id;
            userRole.UserName = user.UserName;

            var adminRole = await _roleManager.FindByNameAsync(SD.AdminEndUser);

            //set all claims name
            string[] claims = new string[4]
            {
                SD.AdminControlClaim,
                SD.VendorControlClaim,
                SD.MarketerControlClaim,
                SD.CampainControlClaim,
            };
            foreach(var claim in claims)
            {
                var rolesForUser = new ClaimsForUser
                {
                    ClaimName = claim
                };

                if (_db.UserClaims.Where(x => x.UserId == user.Id && x.ClaimType == claim).Any())       //if the admin has the claim
                {
                    rolesForUser.IsSelected = true;
                }
                else
                {
                    rolesForUser.IsSelected = false;
                }

                userRole.Claims.Add(rolesForUser);
            }

            return View(userRole);
        }


        [HttpPost]
        public async Task<IActionResult> EditRolesInUser(UserRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
                
            if (user == null)
            {
                return NotFound();
            }

            var adminRole = await _roleManager.FindByNameAsync(SD.AdminEndUser);


            for (int i = 0; i < model.Claims.Count; i++)
            {
                string[] claimsList = new string[4]
                {
                SD.AdminControlClaim,
                SD.VendorControlClaim,
                SD.MarketerControlClaim,
                SD.CampainControlClaim,
                };
                
                //var claim = claimsList.Where(x => x == model.Claims[i].ClaimName).FirstOrDefault();

                var claim =  model.Claims[i].ClaimName;

                //if the claim selected and the admin has't the claim
                if (model.Claims[i].IsSelected && !(_db.UserClaims.Where(x => x.UserId == user.Id && x.ClaimType == claim).Any()))
                {
                    await _userManager.AddClaimAsync(user, new Claim(claim, claim));
                }
                //if the claim didn't select and the admin has the claim
                else if (!model.Claims[i].IsSelected && (_db.UserClaims.Where(x => x.UserId == user.Id && x.ClaimType == claim).Any()))
                {
                    await _userManager.RemoveClaimAsync(user, new Claim(claim, claim));
                }
            }

            TempData["state"] = 1;
            TempData["Message"] = "Edit successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}