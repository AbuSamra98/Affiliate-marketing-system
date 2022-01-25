using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.Base.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Store.DataAccess.Repository.Interfaces;
using Store.DataAccess.Repository.Core;
using AutoMapper;
using Store.Base.Utility;
using Blogs.Hubs;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http;

namespace Store
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();     //to get ip address of client


            //services.AddAuthentication(Options =>
            //{
            //    Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    Options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            //})
            //    .AddJwtBearer(option =>
            //    {
            //        option.SaveToken = true;
            //        option.RequireHttpsMetadata = false;
            //        option.TokenValidationParameters = new TokenValidationParameters()
            //        {
            //            ValidateIssuerSigningKey = true,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecurityKey")),
            //            ValidateIssuer = false,
            //            ValidateAudience = false            //the website accept the token and reply it to third party or third website

            //        };
            //    });


            //services.AddCors(opthions => opthions.AddPolicy("CorsPolicy", builder =>                //for CorsPolicy to connect angular with asp.net(ÊÚØí ÇáÕáÇÍíÇÊ)
            //{
            //    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            //}));

            services.AddSignalR();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AccessAdminsPolicy", policy => policy.RequireAssertion(context =>
                    context.User.IsInRole(SD.SuperAdminEndUser) || context.User.HasClaim(claim => claim.Type == SD.AdminControlClaim)
                    ));
                options.AddPolicy("ApproveVendorsPolicy", policy => policy.RequireAssertion(context =>
                    context.User.IsInRole(SD.SuperAdminEndUser) || context.User.HasClaim(claim => claim.Type == SD.VendorControlClaim)
                    ));
                options.AddPolicy("ApproveMarketersPolicy", policy => policy.RequireAssertion(context =>
                    context.User.IsInRole(SD.SuperAdminEndUser) || context.User.HasClaim(claim => claim.Type == SD.MarketerControlClaim)
                    ));
                options.AddPolicy("ApproveCampaignsPolicy", policy => policy.RequireAssertion(context =>
                    context.User.IsInRole(SD.SuperAdminEndUser) || context.User.HasClaim(claim => claim.Type == SD.CampainControlClaim)
                    ));
            });

            services.AddAutoMapper(typeof(Startup));


            services.AddScoped<IVendorRepository, VendorRepository>();
            services.AddScoped<ICampaignRepository, CampaignRepository>();
            services.AddScoped<IMarketerRepository, MarketerRepository>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            SeedDatabase.initialize(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);

            app.UseForwardedHeaders(new ForwardedHeadersOptions         //to get ip address of client
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();




            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<NotificationHub>("/notificationHub");
            });
        }
    }
}
