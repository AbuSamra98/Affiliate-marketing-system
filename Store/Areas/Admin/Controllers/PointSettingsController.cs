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
    public class PointSettingsController : Controller
    {

        private readonly ApplicationDbContext _db;

        public PointSettingsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var pointSettings = _db.PointSettings.FirstOrDefault();
            return View(pointSettings);
        }

        [HttpPost]
        public IActionResult Index(PointSettings pointSettings)
        {
            if (ModelState.IsValid)
            {
                var pointSettingsFromDB = _db.PointSettings.FirstOrDefault();
                pointSettingsFromDB.PointValue = pointSettings.PointValue;
                pointSettingsFromDB.PercentageForAdmin = pointSettings.PercentageForAdmin;
                _db.Update(pointSettingsFromDB);
                _db.SaveChanges();

                TempData["state"] = 1;
                TempData["Message"] = "Point Settings updated successfully";
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
            return View(pointSettings);
        }
    }
}