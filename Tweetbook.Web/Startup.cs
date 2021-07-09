using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tweetbook.Web.Data;

namespace Tweetbook.Web
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
            // sql server
            services.AddDbContext<WebContext>(options => options.UseInMemoryDatabase(databaseName: "GoogleIdenity"));


            // Add Elcamino Azure Table Identity services.
            services.AddIdentity<IdentityUser, IdentityRole>((options) =>
            {
                // User settings.
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = String.Empty; //  "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<WebContext>()
            .AddDefaultTokenProviders();


            services.AddRazorPages();

            services.AddAuthentication()
            .AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection = Configuration.GetSection("Google:Identity");

                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
                options.SignInScheme = IdentityConstants.ExternalScheme;
            });

            services.ConfigureApplicationCookie(options =>
            {

                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);

                options.LoginPath = "/Login/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
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
                endpoints.MapRazorPages();
            });
        }
    }
}
