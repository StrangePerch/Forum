using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
using Forum.Models;
using Forum.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Forum
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
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ForumUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
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
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            
            CreateRoles(provider).Wait();
        }
        
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ForumUser>>();
            string[] roleNames = { "Admin", "Senior", "Basic", "Novice", "Banned" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            //Create default admin if no admins found
            if (!(await userManager.GetUsersInRoleAsync("Admin")).Any())
            {
                var existingAdminWithoutRole = await userManager.FindByEmailAsync("admin@gmail.com");
                if (existingAdminWithoutRole != null)
                {
                    await userManager.AddToRoleAsync(existingAdminWithoutRole, "Admin");
                }
                else
                {
                    var defaultAdmin = new ForumUser
                    {
                        UserName = "Admin",
                        Email = "admin@gmail.com",
                        JoinedAt = DateTime.Now
                    };
            
                    string userPWD = "!tempPWD123!";

                    var createPowerUser = await userManager.CreateAsync(defaultAdmin, userPWD);
                    if (createPowerUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(defaultAdmin, "Admin");
                    }
                }
            }
            
            //If users without role found then novice role
            foreach (var user in userManager.Users)
            {
                if(!(await userManager.GetRolesAsync(user)).Contains("Novice"))
                    await userManager.AddToRoleAsync(user, "Novice");
            }
        }
    }
}