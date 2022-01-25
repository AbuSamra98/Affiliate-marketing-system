using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Base.Models;
using Store.Base.Utility;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.Interfaces;

namespace Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "ApproveMarketersPolicy")]
    public class MarketerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IMarketerRepository _marketerRepository;


        public MarketerController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IMarketerRepository marketerRepository)
        {
            _userManager = userManager;
            _db = db;
            _marketerRepository = marketerRepository;
        }


        public async Task<IActionResult> Index()
        {
            return View(await _marketerRepository.GetAllMarketers());
        }



        public async Task<IActionResult> Details(string id)
        {
            var marketer = await _db.Marketers.Include(x => x.User).Include(x => x.Country).SingleOrDefaultAsync(x => x.MarketerId == id);
            if (marketer == null)
            {
                return NotFound();
            }

            return View(marketer);
        }

        //for the Marketer to show his profile
        [AllowAnonymous]
        public async Task<IActionResult> DetailsForMarketer()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var marketer = await _db.Marketers.Include(x => x.User).Include(x => x.Country).SingleOrDefaultAsync(x => x.MarketerId == claim.Value);
            if (marketer == null)
            {
                return NotFound();
            }

            return View("Details", marketer);
        }


        [HttpPost]
        public async Task<IActionResult> Lock(string userName)
        {
            if (userName == null)
            {
                return NotFound();
            }

            var marketer = await _userManager.FindByNameAsync(userName);

            if (marketer == null)
            {
                return NotFound();
            }

            //check if the user is a marketer
            if (!(await _userManager.IsInRoleAsync(marketer, SD.MarketerEndUser)))
            {
                return NotFound();
            }

            if (marketer.LockoutEnd == null || marketer.LockoutEnd < DateTime.Now)
            {
                marketer.LockoutEnd = DateTime.Now.AddYears(1000);        //Lock
                await _db.SaveChangesAsync();

                return Json(1);
            }
            else
            {
                marketer.LockoutEnd = DateTime.Now;       //unLock
                await _db.SaveChangesAsync();

                return Json(2);
            }


        }
    }
}