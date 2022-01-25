using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Store.Base.Models;
using Store.Base.Models.ViewModels;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.Interfaces;

namespace Store.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;



        public HomeController(UserManager<ApplicationUser> userManager, ICampaignRepository campaignRepository, ApplicationDbContext db)
        {
            _campaignRepository = campaignRepository;
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int CampaignPage = 1, int SortBy = 1, string searchName = null)
        {
            //get the approved and enabled and not expire campaigns
            var campaigns = await _campaignRepository.GetApprovedCampaigns(true, false, true);        //(bool isApproved, bool isExpired, bool isEnabled)

            if (searchName != null)
            {
                campaigns = campaigns.Where(x => x.Title.ToLower().Contains(searchName.ToLower())).ToList();
            }

            if (SortBy == 1)
            {
                campaigns = campaigns.OrderBy(c => c.ReleaseDate);
            }
            else if (SortBy == 2)
            {
                campaigns = campaigns.OrderBy(c => c.ExpireDate);
            }
            else if (SortBy == 3)
            {
                campaigns = campaigns.OrderBy(c => c.Points);
            }
            else if (SortBy == 4)
            {
                campaigns = campaigns.OrderBy(c => c.SubscribersCount);
            }


            var countDiv6 = (double)(campaigns.Count()) / 6;

            ViewData["SortBy"] = SortBy;

            ViewData["NumberOfPages"] = (int)Math.Ceiling(countDiv6);
            ViewData["currentPage"] = CampaignPage;
            ViewData["searchName"] = searchName;

            campaigns = campaigns.Skip((CampaignPage - 1) * 6).Take(6);              //6 is the number of Campaigns in one page  (Page size)

            return View(campaigns);
        }


        public async Task<IActionResult> CampaignDetails(int CampaignId)
        {
            var campaign = await _campaignRepository.FindCampaignIncludeVendor(CampaignId);
            if (campaign == null)
            {
                return NotFound();
            }
            var OwnerName = campaign.Vendor.User.UserName;
            campaign.Vendor = null;
            var campaignDetails = new CampaignDetailsViewModel()
            {
                Campaign = campaign,
                RelatedCampaigns = await _campaignRepository.GetCampaigns(campaign.VendorId),
                Owner = OwnerName
            };

            return View(campaignDetails);
        }


        //get campaigns for vendor
        public async Task<IActionResult> GetRelatedCampaigns(string userName, int counter)
        {
            var vendor = await _userManager.FindByNameAsync(userName);
            if(vendor == null)
            {
                return NotFound();
            }

            var campaigns = await _campaignRepository.GetApprovedCampaigns(true, false, true);
            campaigns = campaigns.Where(x => x.VendorId == vendor.Id);

            campaigns = campaigns.Skip(counter * 3).Take(3);

            return Json(campaigns);
        }


        public IActionResult AboutUs()
        {
            return View(_db.AboutUs.FirstOrDefault());
        }

        public IActionResult HowItWorks()
        {
            return View(_db.HowItWorks.FirstOrDefault());
        }

        public IActionResult Privacy()
        {
            return View(_db.PrivacyPolicy.FirstOrDefault());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //get messages for vendors and marketers
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMessages(int conter)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(claim.Value);
            if (user == null)
            {
                return null;
            }

            //ViewData["messageCounter"] = 0;

            var messagesList = _db.Messages.Where(x => x.ReceiverUserId == user.Id).ToList();
            messagesList.Reverse();
            IEnumerable<Message> messages = messagesList;
            messages = messages.Skip(conter*5).Take(5);
            return Json(messages);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMessage(int id)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(claim.Value);
            if (user == null)
            {
                return null;
            }

            var message = _db.Messages.Where(x => x.Id == id && x.ReceiverUserId == user.Id).FirstOrDefault();
            message.Read = true;
            _db.Messages.Update(message);
            _db.SaveChanges();
            return View(message);
        }

    }
}
