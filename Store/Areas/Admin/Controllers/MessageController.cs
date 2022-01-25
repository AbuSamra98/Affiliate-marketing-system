using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blogs.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Store.Base.Models;
using Store.Base.Models.ViewModels;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.Interfaces;


namespace Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MessageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IVendorRepository _vendorRepository;
        private readonly IMarketerRepository _marketerRepository;


        public MessageController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IVendorRepository vendorRepository, IMarketerRepository marketerRepository, IHubContext<NotificationHub> hub)
        {
            _userManager = userManager;
            _db = db;
            _vendorRepository = vendorRepository;
            _marketerRepository = marketerRepository;
            _hub = hub;


        }

        private IHubContext<NotificationHub> _hub { get; set; }



        [HttpGet]
        public IActionResult SendMessage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(SendMessageViewModel sendMessage)      //SendForSelect => (1) for vendor | (2) for marketer | (3) for all vendors | (4) for all marketers
        {
            if (ModelState.IsValid)
            {
                //for specific vendor or marketer
                if(sendMessage.SendForSelect == 1 || sendMessage.SendForSelect == 2)
                {
                    if(sendMessage.UserName == null)
                    {
                        ModelState.AddModelError(string.Empty, "User not found");
                        return View(sendMessage);
                    }
                    var user = await _userManager.FindByNameAsync(sendMessage.UserName);
                    if(user == null)
                    {
                        ModelState.AddModelError(string.Empty, "User not found");
                        return View(sendMessage);
                    }
                    var message = new Message()
                    {
                        ReceiverUserId = user.Id,
                        Text = sendMessage.Text
                    };

                    _db.Messages.Add(message);
                    await _db.SaveChangesAsync();

                    //send notification for receiver
                    await _hub.Clients.User(user.Id).SendAsync("ReceiveMessage", 2, "there's new message from admin");

                    TempData["state"] = 1;
                    TempData["Message"] = "Sent successfully, The message sent to the user " + user.UserName + " successfully";
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
                //for all vendors
                if (sendMessage.SendForSelect == 3)
                {
                    var vendors = await _vendorRepository.GetAll();
                    foreach(var vendor in vendors)
                    {
                        var message = new Message()
                        {
                            Text = sendMessage.Text,
                            ReceiverUserId = vendor.VendorId
                        };
                        _db.Messages.Add(message);
                        await _hub.Clients.User(vendor.VendorId).SendAsync("ReceiveMessage", 2, "there's new message from admin");
                    }
                    await _db.SaveChangesAsync();

                    TempData["state"] = 1;
                    TempData["Message"] = "Sent successfully, The message sent to all vendors successfully";
                    return RedirectToAction("Index", "Home", new { area = "Customer"});


                }
                //for all marketers
                if (sendMessage.SendForSelect == 4)
                {
                    var marketers = await _marketerRepository.GetAll();
                    foreach (var marketer in marketers)
                    {
                        var message = new Message()
                        {
                            Text = sendMessage.Text,
                            ReceiverUserId = marketer.MarketerId
                        };
                        _db.Messages.Add(message);
                        await _hub.Clients.User(marketer.MarketerId).SendAsync("ReceiveMessage", 2, "there's new message from admin");
                    }
                    await _db.SaveChangesAsync();

                    TempData["state"] = 1;
                    TempData["Message"] = "Sent successfully, The message sent to all marketers successfully";
                    return RedirectToAction("Index", "Home", new { area = "Customer" });

                }
            }
            return View();
        }

        //for search vendors
        [HttpGet]
        public async Task<IActionResult> GetVendors()
        {
            var vendors = await _vendorRepository.GetAllVendors();

            return Json(vendors);
        }
        //for search marketers
        public async Task<IActionResult> GetMarketers()
        {
            var marketers = await _marketerRepository.GetAllMarketers();

            return Json(marketers);
        }
    }
}