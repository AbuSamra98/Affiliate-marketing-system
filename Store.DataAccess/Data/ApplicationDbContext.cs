using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Base.Models;

namespace Store.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<CampaignTypes> CampaignTypes { get; set; }
        public DbSet<Country> countries { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Marketer> Marketers { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<CompainsSubscribedByMarketer> CompainsSubscribedByMarketers { get; set; }
        public DbSet<UpdateVendorProfileRequest> UpdateVendorProfileRequests { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<PrivacyPolicy> PrivacyPolicy { get; set; }
        public DbSet<HowItWorks> HowItWorks { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<PointSettings> PointSettings { get; set; }
        public DbSet<CheckoutOrder> CheckoutOrders { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<Click> Clicks { get; set; }





    }
}
