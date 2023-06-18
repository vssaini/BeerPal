﻿using BeerPal.Web.Entities;
using BeerPal.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace BeerPal.Web
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
                options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add PayPal client factory.
            services.AddSingleton(factory => new PayPalHttpClientFactory(
                Configuration["PayPal:ClientId"],
                Configuration["PayPal:ClientSecret"],
                Convert.ToBoolean(Configuration["PayPal:IsLive"]))); // Is Live Environment?

            // Add Stripe client factory
            services.AddSingleton(factory => new StripeApiFactory(
                Configuration["Stripe:SecretKey"],
                Configuration["Stripe:PublishableKey"]));

            // Add Braintree client factory
            services.AddSingleton(factory => new BraintreeApiFactory(
                Configuration["Braintree:MerchantId"],
                Configuration["Braintree:PublicKey"],
                Configuration["Braintree:PrivateKey"]));

            services.AddTransient<SeedDatabaseService>();
            services.AddTransient<GatewaySwitchService>();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("areas", "{area=paypal}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
