using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvcCore.AuthorizationRequirements;
using MvcCore.Models;
using MvcCore.Repositories;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcCore
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
            services.AddControllersWithViews();
            //services.AddDbContext<TaskManagerContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("TaskManagerDatabase")));

            //-- Temporary For Authentication -- START
            services.AddDbContext<TaskManagerContext>(config =>
            {
                config.UseInMemoryDatabase("Memory");
            });

            services.AddIdentity<IdentityUser, IdentityRole>(config =>
            {
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequiredLength = 3;
                config.Password.RequireDigit = false;
                config.Password.RequireUppercase = false;
                config.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<TaskManagerContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "Identity.Cookie";
                config.LoginPath = "/Task/Login";
            });
            var mailKitOptions = Configuration.GetSection("Email").Get<MailKitOptions>();

            services.AddMailKit(config => config.UseMailKit(mailKitOptions));

            services.AddAuthorization(config =>
            {
                //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                //var defaultAuthPolicy = defaultAuthBuilder
                //.RequireAuthenticatedUser()
                //.RequireClaim(ClaimTypes.DateOfBirth)
                //.Build();

                //config.DefaultPolicy = defaultAuthPolicy;

                //config.AddPolicy("Claim.DoB", policyBuilder =>
                //{
                //    policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
                //});
                
                config.AddPolicy("Claim.DoB", policyBuilder =>
                {
                    policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
                });
            });

            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();

            //-- END

            services.AddTransient<ITaskRepository, TaskRepository>();


            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<TaskManagerContext>();
            services.AddRazorPages();
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Task}/{action=Index}/{id?}");

                //endpoints.MapDefaultControllerRoute();

                endpoints.MapRazorPages();
            });
        }
    }
}
