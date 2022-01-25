using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Store.Base.Models;
using Store.Base.Models.ViewModels;
using Store.Base.Utility;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.Interfaces;

namespace Store.Controllers
{
    [Area("Vendor")]
    [Authorize(Roles = SD.VendorEndUser)]
    public class CampaignController : Controller
    {
        //private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVendorRepository _vendorRepository;
        private readonly ApplicationDbContext _db;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IHostingEnvironment _hostingEnvironment;


        [BindProperty]
        public CampaignViewModel CampaignVM { get; set; }

        public CampaignController(ICampaignRepository campaignRepository, ApplicationDbContext db, IHostingEnvironment hostingEnvironment, IVendorRepository vendorRepository)
        {
            _db = db;
            _campaignRepository = campaignRepository;
            _hostingEnvironment = hostingEnvironment;
            _vendorRepository = vendorRepository;


            CampaignVM = new CampaignViewModel()
            {
                CampaignTypes = _db.CampaignTypes.ToList(),
                Campaign = new Campaign()
            };
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var campaigns = await _campaignRepository.GetCampaigns(claim.Value);


            return View(campaigns);
        }

        public IActionResult Create()
        {
            return View(CampaignVM);
        }


        //POST Campaign Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (!ModelState.IsValid)
            {
                CampaignVM.CampaignTypes = _db.CampaignTypes.ToList();
                return View(CampaignVM);
            }
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var vendor = await _vendorRepository.Find(claim.Value);
            if(vendor == null)
            {
                return NotFound();
            }
            if(vendor.Points < CampaignVM.Campaign.Points)
            {
                TempData["state"] = 0;
                TempData["Message"] = "You dont have enough Points, your points is " + vendor.Points;
                CampaignVM.CampaignTypes = _db.CampaignTypes.ToList();
                return View(CampaignVM);
            }

            CampaignVM.Campaign.ExpireDate = CampaignVM.Campaign.ExpireDate
                                                            .AddHours(CampaignVM.Campaign.ExpireTime.Hour)
                                                            .AddMinutes(CampaignVM.Campaign.ExpireTime.Minute);


            CampaignVM.Campaign.VendorId = vendor.VendorId;
            await _campaignRepository.Add(CampaignVM.Campaign);
            vendor.Points -= CampaignVM.Campaign.Points;
            await _vendorRepository.Update(vendor);



            //Image being saved
            string webRootPath = _hostingEnvironment.WebRootPath;       //root path
            var files = HttpContext.Request.Form.Files;         //now files have the file or img that uploaded from the form
            var campaignFromDb = await _campaignRepository.Find(CampaignVM.Campaign.Id);

            if (files.Count != 0)
            {
                //image has been uploaded
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);        //find tha path of the upload img.. (so go to ProductImage folder in the root)
                var extension = Path.GetExtension(files[0].FileName);           //get extension of the img(jpg or png or.....)


                using (var filestream = new FileStream(Path.Combine(uploads, CampaignVM.Campaign.Id + extension), FileMode.Create))         //pass the id of the product to be the image name
                {
                    files[0].CopyTo(filestream);        //copy the uploded image to the FileStream and with the new name(product id)
                }

                campaignFromDb.Image = @"\" + SD.ImageFolder + @"\" + CampaignVM.Campaign.Id + extension;
            }
            else
            {
                //if the user didn't uplode the image=> add a default image
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + CampaignVM.Campaign.Id + ".png");
                campaignFromDb.Image = @"\" + SD.ImageFolder + @"\" + CampaignVM.Campaign.Id + ".png";
            }
            await _db.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }


        ////Get Campaign
        //public IActionResult Edit(int? id)
        //{
        //    if(id==null)
        //    {
        //        return NotFound();
        //    }
        //    var claimsIdentity = (ClaimsIdentity)this.User.Identity;
        //    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        //    CampaignVM.Campaign = _campaignRepository.Get(x => x.Id == id && x.VendorId == claim.Value).FirstOrDefault();

        //    if (CampaignVM.Campaign == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(CampaignVM);
        //}


        ////POST Edit
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(CampaignVM);
        //    }

        //    CampaignVM.Campaign.ExpireDate = CampaignVM.Campaign.ExpireDate
        //                                                    .AddHours(CampaignVM.Campaign.ExpireTime.Hour)
        //                                                    .AddMinutes(CampaignVM.Campaign.ExpireTime.Minute);

        //    string webRootPath = _hostingEnvironment.WebRootPath;
        //    var files = HttpContext.Request.Form.Files;

        //    var campaignFromDb = await _campaignRepository.Find(CampaignVM.Campaign.Id);

        //    if (files.Count != 0 && files[0] != null)
        //    {
        //        //if the user upload a new image
        //        var uploads = Path.Combine(webRootPath, SD.ImageFolder);
        //        var extension_new = Path.GetExtension(files[0].FileName);
        //        var extension_old = Path.GetExtension(campaignFromDb.Image);

        //        if (System.IO.File.Exists(Path.Combine(uploads, CampaignVM.Campaign.Id + extension_old)))
        //        {
        //            System.IO.File.Delete(Path.Combine(uploads, CampaignVM.Campaign.Id + extension_old));
        //        }


        //        using (var filestream = new FileStream(Path.Combine(uploads, CampaignVM.Campaign.Id + extension_new), FileMode.Create))         //pass the id of the product to be the image name
        //        {
        //            files[0].CopyTo(filestream);        //copy the uploded image to the FileStream and with the new name(product id)
        //        }

        //        CampaignVM.Campaign.Image = @"\" + SD.ImageFolder + @"\" + CampaignVM.Campaign.Id + extension_new;
        //    }

        //    if (CampaignVM.Campaign.Image != null)
        //    {
        //        campaignFromDb.Image = CampaignVM.Campaign.Image;
        //    }

        //    campaignFromDb.Title = CampaignVM.Campaign.Title;
        //    campaignFromDb.Desc = CampaignVM.Campaign.Desc;
        //    campaignFromDb.Points = CampaignVM.Campaign.Points;
        //    campaignFromDb.ExpireDate = CampaignVM.Campaign.ExpireDate;
        //    campaignFromDb.CampaignTypeId = CampaignVM.Campaign.CampaignTypeId;

        //    await _db.SaveChangesAsync();


        //    return RedirectToAction(nameof(Index));

        //}


        //Get Campaign

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            CampaignVM.Campaign = await _campaignRepository.Find(id);

            if (CampaignVM.Campaign == null)
            {
                return NotFound();
            }
            var subCount = _db.CompainsSubscribedByMarketers.Where(x => x.CampaignId == id).Count();
            CampaignVM.SubscribersCount = subCount;

            return View(CampaignVM);
        }


        //Get Campaign
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            CampaignVM.Campaign = await _campaignRepository.Find(id);

            if (CampaignVM.Campaign == null)
            {
                return NotFound();
            }

            var subCount = _db.CompainsSubscribedByMarketers.Where(x => x.CampaignId == id).Count();
            CampaignVM.SubscribersCount = subCount;

            return View(CampaignVM);
        }

        //POST Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Campaign campaign = await _campaignRepository.Find(id);

            if (campaign == null)
            {
                return NotFound();
            }

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(campaign.VendorId!=claim.Value)
            {
                return NotFound();
            }

            var uploads = Path.Combine(webRootPath, SD.ImageFolder);
            var extension = Path.GetExtension(campaign.Image);


            if (System.IO.File.Exists(Path.Combine(uploads, campaign.Id + extension)))
            {
                System.IO.File.Delete(Path.Combine(uploads, campaign.Id + extension));
            }
            await _campaignRepository.Remove(campaign);

            return RedirectToAction(nameof(Index));

            
        }


        public async Task<IActionResult> AddPoints(int id, int points)
        {
            if(points>0)
            {
                var campaign = await _campaignRepository.FindCampaignIncludeVendor(id);

                if (campaign == null)
                {
                    return NotFound();
                }
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                var vendor = await _vendorRepository.Find(claim.Value);
                if(vendor == null || campaign.VendorId != claim.Value)
                {
                    return NotFound();
                }

                if(vendor.Points < points)
                {
                    TempData["state"] = 0;
                    TempData["Message"] = "You dont have enough Points, your points is "+ vendor.Points;
                    return RedirectToAction(nameof(Index));
                }

                campaign.Points = campaign.Points + points;
                await _campaignRepository.Update(campaign);
                vendor.Points -= points;
                await _vendorRepository.Update(vendor);

                //return Json(new { camTitle = campaign.Title, state = 1 });
                TempData["state"] = 1;
                TempData["Message"] = "Added "+ points +" points to the campaign "+ campaign.Title +" successfully";
                return RedirectToAction(nameof(Index));
                //return Json(new { Points = "Successed", state = 1 });
            }
            return RedirectToAction("Warning", "Account", new { area = "Customer" });
        }


        public async Task<IActionResult> EditExpireDate(int id, DateTime expireDate, DateTime expireTime)
        {
            var campaign = await _campaignRepository.FindCampaignIncludeVendor(id);

            if (campaign == null)
            {
                return NotFound();
            }

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (campaign.VendorId != claim.Value)
            {
                return NotFound();
            }

            expireDate = expireDate
                                .AddHours(expireTime.Hour)
                                .AddMinutes(expireTime.Minute);

            campaign.ExpireDate = expireDate;

            await _campaignRepository.Update(campaign);

            return RedirectToAction(nameof(Index));
        }

        

       


        public async Task<IActionResult> Lock(int id)
        {

            var campaign = await _campaignRepository.FindCampaignIncludeVendor(id);

            if (campaign == null)
            {
                return NotFound();
            }

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (campaign.VendorId != claim.Value)
            {
                return NotFound();
            }

            if (campaign.Enable)
            {
                campaign.Enable = false;
                await _campaignRepository.Update(campaign);


                return Json(1);
            }
            else
            {
                campaign.Enable = true;
                await _campaignRepository.Update(campaign);


                return Json(2);
            }
        }


    }
}