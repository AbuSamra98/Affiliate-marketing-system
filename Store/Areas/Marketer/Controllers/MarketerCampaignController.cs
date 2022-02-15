using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Blogs.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Store.Base.Models;
using Store.Base.Utility;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.Interfaces;

namespace Store.Areas.Marketer.Controllers
{
    [Area("Marketer")]
    [Authorize(Roles = SD.MarketerEndUser)]
    public class MarketerCampaignController : Controller
    {

        private readonly ICampaignRepository _campaignRepository;
        private readonly IMarketerRepository _marketerRepository;
        private readonly ApplicationDbContext _db;
        private IHttpContextAccessor _accessor;



        public MarketerCampaignController(IHttpContextAccessor accessor, ICampaignRepository campaignRepository, IHubContext<NotificationHub> hub, ApplicationDbContext db, IMarketerRepository marketerRepository)
        {
            _campaignRepository = campaignRepository;
            _hub = hub;
            _db = db;
            _marketerRepository = marketerRepository;
            _accessor = accessor;

        }


        private IHubContext<NotificationHub> _hub { get; set; }



        public async Task<IActionResult> Index(int CampaignPage = 1, string searchName = null)
        {

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            //get Subscribed campaigns by marketer
            IEnumerable<Campaign> campaigns = await _db.CompainsSubscribedByMarketers.Include(x => x.Campaigns).Include(x => x.Campaigns.CampaignTypes)
                                    .Where(x => x.MarketerId == claim.Value).Select(x => x.Campaigns)
                                    .Where(x => x.IsApproved == true && x.ExpireDate > DateTime.Now && x.Enable == true).ToListAsync();

            if (searchName != null)
            {
                campaigns = campaigns.Where(x => x.Title.ToLower().Contains(searchName.ToLower())).ToList();
            }

            var countDiv6 = (double)(campaigns.Count()) / 6;

            ViewData["NumberOfPages"] = (int)Math.Ceiling(countDiv6);
            ViewData["currentPage"] = CampaignPage;
            ViewData["searchName"] = searchName;

            campaigns = campaigns.Skip((CampaignPage - 1) * 6).Take(6);              //6 is the number of Campaigns in one page  (Page size)

            return View(campaigns);
        }


        public async Task<IActionResult> Subscribe(int? CampaignId)
        {
            if (CampaignId == null)
            {
                return Json(-1);
            }
            var campaign = await _campaignRepository.Find(CampaignId);
            if (campaign == null)
            {
                return Json(-1);
            }
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (_db.CompainsSubscribedByMarketers.Where(x => x.CampaignId == campaign.Id && x.MarketerId == claim.Value).Any())
            {
                return Json(0);
            }

            var MarketerSubCampaign = new CompainsSubscribedByMarketer()
            {
                CampaignId = campaign.Id,
                MarketerId = claim.Value
            };

            await _db.CompainsSubscribedByMarketers.AddAsync(MarketerSubCampaign);
            campaign.SubscribersCount += 1;
            await _db.SaveChangesAsync();

            await _hub.Clients.User(campaign.VendorId).SendAsync("ReceiveMessage", 2, "New subscribe to campaign " + campaign.Title);

            //return Json(new { status = 1,cam = campaign });

            return Json(1);

        }


        public async Task<IActionResult> UnSubscribe(int? CampaignId)
        {
            if (CampaignId == null)
            {
                return NotFound();
            }
            var campaign = await _campaignRepository.Find(CampaignId);
            if (campaign == null)
            {
                return NotFound();
            }
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var MarketerUnSubCampaign = _db.CompainsSubscribedByMarketers.Where(x => x.CampaignId == campaign.Id && x.MarketerId == claim.Value).FirstOrDefault();
            if (MarketerUnSubCampaign == null)
            {
                return NotFound();
            }

            _db.CompainsSubscribedByMarketers.Remove(MarketerUnSubCampaign);
            campaign.SubscribersCount -= 1;
            await _db.SaveChangesAsync();

            await _hub.Clients.User(campaign.VendorId).SendAsync("ReceiveMessage", 3, "there is a marketer unsubscribe the campaign " + campaign.Title);

            TempData["state"] = 2;
            TempData["Message"] = "UnSubscibed successfully, You unsubscribed the campaign " + campaign.Title + " successfully";
            return RedirectToAction(nameof(Index));

        }

        //var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecurityKey"));
        //                    var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512);


        public async Task<IActionResult> CreateLink(int? CampaignId)
        {
            if (CampaignId == null)
            {
                return Json(new { status = -1 });
            }
            var campaign = await _campaignRepository.Find(CampaignId);
            if (campaign == null)
            {
                return Json(new { status = -1 });
            }

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var marketer = await _marketerRepository.Find(claim.Value);
            if (marketer == null)
            {
                return Json(new { status = -1 });
            }

            var link = _db.Links.Where(x => x.MarketerId == marketer.MarketerId && x.CampaignId == campaign.Id).FirstOrDefault();

            if (link == null)
            {
                link = new Link()
                {
                    MarketerId = marketer.MarketerId,
                    CampaignId = campaign.Id,
                    URL = campaign.URL
                };
                _db.Links.Add(link);
                _db.SaveChanges();
            }


            return Json(new { status = 1, cam = campaign, linkId = link.Id });

        }

        [AllowAnonymous]
        public async Task<IActionResult> Click(int? linkId)
        {
            if(linkId == null)
            {
                return NotFound();
            }

            var link = _db.Links.Find(linkId);
            if(link == null)
            {
                return NotFound();
            }


            var httpClient = new HttpClient();
            var ip = await httpClient.GetStringAsync("https://api.ipify.org");


            //this will return ::1 in localhost(solve when its published)
            //var remoteIpAddress = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            //check if this click exist with the same day and the same ip address
            if (!(_db.Clicks.Where(x => x.LinkId == linkId && x.IPAddress == ip && DateTime.Now > x.Date && DateTime.Now.AddDays(1) < x.Date ).Any()))
            {
                var click = new Click()
                {
                    LinkId = link.Id,
                    IPAddress = ip,
                    Date = DateTime.Now
                };

                var marketer = await _marketerRepository.Find(link.MarketerId);
                if(marketer == null)
                {
                    return NotFound();
                }

                var campaign = await _campaignRepository.Find(link.CampaignId);
                if(campaign == null)
                {
                    return NotFound();
                }

                var PointsSettings = await _db.PointSettings.FirstAsync();
                var transferedMoney = PointsSettings.PointValue * ((100 - PointsSettings.PercentageForAdmin) / 100);

                if (campaign.Points > 0)
                {
                    campaign.Points -= 1;
                    marketer.Points += 1;
                    marketer.Salary += transferedMoney;
                }
                _db.Clicks.Add(click);

                _db.SaveChanges();

            }

            return Redirect(link.URL);

        }
    }
}