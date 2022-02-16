using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blogs.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Store.Base.Utility;
using Store.DataAccess.Repository.Interfaces;

namespace Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "ApproveCampaignsPolicy")]
    public class CampaignController : Controller
    {


        private readonly ICampaignRepository _campaignRepository;
        private readonly IHostingEnvironment _hostingEnvironment;


        public CampaignController(ICampaignRepository campaignRepository, IHubContext<NotificationHub> hub, IHostingEnvironment hostingEnvironment)
        {
            _campaignRepository = campaignRepository;
            _hub = hub;
            _hostingEnvironment = hostingEnvironment;
        }


        private IHubContext<NotificationHub> _hub { get; set; }

        //get all campaigns
        public async Task<IActionResult> Index()
        {
            return View(await _campaignRepository.GetCampaignsIncludeVendor());
        }


        public async Task<IActionResult> NotApprovedCampaigns()
        {
            return View(await _campaignRepository.GetApprovedCampaigns(false));
        }


        public async Task<IActionResult> Lock(int id)
        {

            var campaign = await _campaignRepository.FindCampaignIncludeVendor(id);

            if (campaign == null)
            {
                return NotFound();
            }
            if (campaign.IsApproved)
            {
                campaign.IsApproved = false;            //block the campaign
                await _campaignRepository.Update(campaign);

                await _hub.Clients.User(campaign.VendorId).SendAsync("ReceiveMessage", 0, "The campaign " + campaign.Title + " blocked");

                return Json(1);
            }
            else
            {
                campaign.IsApproved = true;         //approve the campaign
                await _campaignRepository.Update(campaign);

                await _hub.Clients.User(campaign.VendorId).SendAsync("ReceiveMessage", 1, "The campaign " + campaign.Title + " approved");

                return Json(2);
            }
        }


        //Get Campaign
        public async Task<IActionResult> Details(int id)
        {
            var campaign = await _campaignRepository.FindCampaignIncludeType(id);

            if (campaign == null)
            {
                return NotFound();
            }

            return View(campaign);
        }


        //Get Campaign
        public async Task<IActionResult> Delete(int id)
        {
            var campaign = await _campaignRepository.FindCampaignIncludeType(id);

            if (campaign == null)
            {
                return NotFound();
            }

            return View(campaign);
        }


        //POST Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            var campaign = await _campaignRepository.Find(id);

            if (campaign == null)
            {
                return NotFound();
            }
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(campaign.Image);

                //delete the image of the campaign if Exists
                if (System.IO.File.Exists(Path.Combine(uploads, campaign.Id + extension)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, campaign.Id + extension));
                }
                await _campaignRepository.Remove(campaign);

                TempData["state"] = 1;
                TempData["Message"] = "The campaign " + campaign.Title + " deleted successfully";
                return RedirectToAction(nameof(Index));

            }
        }

    }
}