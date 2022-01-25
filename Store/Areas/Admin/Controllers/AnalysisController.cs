using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Base.Models;
using Store.Base.Models.ViewModels;
using Store.Base.Utility;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.Interfaces;

namespace Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.SuperAdminEndUser)]
    public class AnalysisController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IVendorRepository _vendorRepository;
        private readonly IMarketerRepository _marketerRepository;



        public AnalysisController(ApplicationDbContext db, IVendorRepository vendorRepository, IMarketerRepository marketerRepository)
        {
            _db = db;
            _vendorRepository = vendorRepository;
            _marketerRepository = marketerRepository;
        }

        public async Task<IActionResult> Index()
        {
            var vendors = await _vendorRepository.GetAllVendors();
            var marketers = await _marketerRepository.GetAllMarketers();

            //get checkoutOrders include vendors
            IEnumerable<CheckoutOrder> checkoutOrders = _db.CheckoutOrders.Include(x => x.Vendor).Include(x => x.Vendor.User).ToList();

            var analysis = new AnalysisViewModel()
            {
                Vendors = vendors,
                Marketers = marketers,
                CheckoutOrders = checkoutOrders,
                UsersCountInCountries = new List<UsersCountInCountry>()
            };


            //add vendors count for every counrty
            foreach (var vendor in vendors)
            {
                if(analysis.UsersCountInCountries.Where(x => x.CountryId == vendor.CountryId).Any())
                {
                    analysis.UsersCountInCountries.Where(x => x.CountryId == vendor.CountryId).FirstOrDefault().VendorsCount += 1;
                    analysis.UsersCountInCountries.Where(x => x.CountryId == vendor.CountryId).FirstOrDefault().AllUsersCount += 1;
                }
                else
                {
                    analysis.UsersCountInCountries.Add(new UsersCountInCountry
                    {
                        CountryId = vendor.CountryId,
                        VendorsCount = 1,
                        MarketersCount = 0,
                        AllUsersCount=1
                    });
                }
            }

            //add vendors count for every counrty
            foreach (var marketer in marketers)
            {
                if (analysis.UsersCountInCountries.Where(x => x.CountryId == marketer.CountryId).Any())
                {
                    analysis.UsersCountInCountries.Where(x => x.CountryId == marketer.CountryId).FirstOrDefault().MarketersCount += 1;
                    analysis.UsersCountInCountries.Where(x => x.CountryId == marketer.CountryId).FirstOrDefault().AllUsersCount += 1;
                }
                else
                {
                    analysis.UsersCountInCountries.Add(new UsersCountInCountry
                    {
                        CountryId = marketer.CountryId,
                        VendorsCount = 0,
                        MarketersCount = 1,
                        AllUsersCount=1
                    });
                }
            }

            //join for counrty name
            for(int i = 0; i< analysis.UsersCountInCountries.Count; i++ )
            {
                analysis.UsersCountInCountries[i].Country = _db.countries.Find(analysis.UsersCountInCountries[i].CountryId).Name;
            }

            analysis.UsersCountInCountries = analysis.UsersCountInCountries.OrderByDescending(x => x.AllUsersCount).ToList();

            for(int i = analysis.UsersCountInCountries.Count; i < 6; i++)
            {
                analysis.UsersCountInCountries.Add(new UsersCountInCountry
                {
                    Country = "No data",
                    AllUsersCount = 0
                });
            }

            return View(analysis);
        }
    }
}