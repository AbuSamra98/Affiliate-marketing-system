using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Base.Models;
using Store.Base.Utility;
using Store.DataAccess.Data;

namespace Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.SuperAdminEndUser)]

    public class StaticPagesController : Controller
    {
        private readonly ApplicationDbContext _db;


        public StaticPagesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult EditAboutUs()
        {
            return View(_db.AboutUs.FirstOrDefault());
        }

        [HttpPost]
        public IActionResult EditAboutUs(AboutUs aboutUs)
        {
            var aboutUsFromDb = _db.AboutUs.FirstOrDefault();
            aboutUsFromDb.Description = aboutUs.Description;
            _db.AboutUs.Update(aboutUsFromDb);
            _db.SaveChanges();

            TempData["state"] = 1;
            TempData["Message"] = "Edit About Us page successfully";
            return RedirectToAction("AboutUs", "Home" , new { area = "Customer"});
        }

        public IActionResult EditHowItWorks()
        {
            return View(_db.HowItWorks.FirstOrDefault());
        }

        [HttpPost]
        public IActionResult EditHowItWorks(HowItWorks howItWorks)
        {
            var howItWorksFromDb = _db.HowItWorks.FirstOrDefault();
            howItWorksFromDb.Description = howItWorks.Description;
            _db.HowItWorks.Update(howItWorksFromDb);
            _db.SaveChanges();

            TempData["state"] = 1;
            TempData["Message"] = "Edit How It Works page successfully";
            return RedirectToAction("HowItWorks", "Home", new { area = "Customer" });
        }

        public IActionResult EditPrivacy()
        {
            return View(_db.PrivacyPolicy.FirstOrDefault());
        }

        [HttpPost]
        public IActionResult EditPrivacy(PrivacyPolicy privacyPolicy)
        {
            var privacyPolicyFromDb = _db.PrivacyPolicy.FirstOrDefault();
            privacyPolicyFromDb.Description = privacyPolicy.Description;
            _db.PrivacyPolicy.Update(privacyPolicyFromDb);
            _db.SaveChanges();

            TempData["state"] = 1;
            TempData["Message"] = "Edit Privacy Policy page successfully";
            return RedirectToAction("Privacy", "Home", new { area = "Customer" });
        }
    }
}