using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Store.Base.Models;
using Store.Base.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Store.DataAccess.Data
{
    public class SeedDatabase
    {
        public async static void initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();


            context.Database.EnsureCreated();

            //add defualt Point Settings
            if (!context.PointSettings.Any())
            {
                context.PointSettings.Add(new PointSettings { PointValue= 0.1f , PercentageForAdmin=20 });
            }
            if (!context.CampaignTypes.Any())
            {
                context.CampaignTypes.Add(new CampaignTypes { Name="Click" });
                context.CampaignTypes.Add(new CampaignTypes { Name = "Load" });
            }
            if (!context.AboutUs.Any())
            {
                context.AboutUs.Add(new AboutUs {Description="<p>Soon</p>" });
                context.SaveChanges();
            }
            if (!context.HowItWorks.Any())
            {
                context.HowItWorks.Add(new HowItWorks { Description = "<p>Soon</p>" });
                context.SaveChanges();
            }
            if (!context.PrivacyPolicy.Any())
            {
                context.PrivacyPolicy.Add(new PrivacyPolicy { Description = "<p>Soon</p>" });
                context.SaveChanges();
            }
            if (!context.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(SD.SuperAdminEndUser));
                await roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
                await roleManager.CreateAsync(new IdentityRole(SD.VendorEndUser));
                await roleManager.CreateAsync(new IdentityRole(SD.MarketerEndUser));
            }
            //add super admin
            if (!context.Users.Any())
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Email = "mohd@gmail.com",
                    UserName = "mohd@gmail.com",
                    FullName = "Mohammed Abu Samra"
                };
                await userManager.CreateAsync(user, "Admin123*");
                await userManager.AddToRoleAsync(user, SD.SuperAdminEndUser);
            }
            if (!context.countries.Any())
            {
                CultureInfo[] cultureInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

                List<string> countries = new List<string>();

                foreach (CultureInfo culture in cultureInfoList)
                {
                    RegionInfo getRegionInfo = new RegionInfo(culture.LCID);
                    if(!(countries.Contains(getRegionInfo.EnglishName)))
                    {
                        countries.Add(getRegionInfo.EnglishName);
                    }
                }
                countries.Sort();
                foreach(string countryName in countries)
                {
                    Country country = new Country()
                    {
                        Name = countryName
                    };
                    context.countries.Add(country);
                }
            }
            context.SaveChanges();

        }
    }
}
