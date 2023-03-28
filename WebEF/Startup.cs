using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEF.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XRoadLib.Extensions.AspNetCore;
using WebEF.Contracts;
using WebEF.Services;
using WebEF.Models;

namespace WebEF
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddDbContext<ServiceManagerDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("ServiceManager")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //X-Road
            services.AddXRoadLib();
            services.AddTransient<IGetActivePaymentsByPIN, GetActivePaymentsByPINService>();
            services.AddTransient<IGetRecipients, GetRecipientsService>();
            services.AddTransient<IMSECDetails, MSECDetailsService>();
            services.AddTransient<ISavePaymentF10, SavePaymentF10Service>();
            services.AddTransient<ISaveNotPaymentF20, SaveNotPaymentF20Service>();
            services.AddTransient<IGetUnemployeeStatus, GetUnemployeeStatusService>();
            services.AddTransient<IAdoptedChildrenReport, AdoptedChildrenReportService>();
            services.AddTransient<ISetPaymentInfo, SetPaymentInfoService>();
            services.AddTransient<ISaveBalagaSuyunchu, SaveBalagaSuyunchuService>();

            services.AddSingleton<MLSDServiceManager>();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseXRoadLib(routes =>
            {
                routes.MapWsdl<MLSDServiceManager>("/wsdl");
                routes.MapWebService<MLSDServiceManager>("/wsdl_endpoint");
            });
        }
    }
}
