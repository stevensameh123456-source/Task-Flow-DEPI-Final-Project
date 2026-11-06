
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data.Context;
using TaskFlow.Data.Entities;
using TaskFlow.Data.Interfaces;
using TaskFlow.Data.Repositories;
using TaskFlow.Data.UnitOfWork;

namespace TaskFlow.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =====================================
            // 1. Register MVC
            // =====================================

            builder.Services.AddControllersWithViews();

            // =====================================
            // 2. Register Database Context
            // =====================================

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );

            // =====================================
            // Register Existing UI Services from main
            // =====================================

            builder.Services.AddScoped<
                TaskFlow.Web.Mock.IWorkspaceUiService,
                TaskFlow.Web.Mock.FakeWorkspaceUiService>();

            builder.Services.AddScoped<
                TaskFlow.Web.Mock.ITeamUiService,
                TaskFlow.Web.Mock.FakeTeamUiService>();

            // =====================================
            // 3. Register Repository & Unit of Work
            // =====================================

            builder.Services.AddScoped(
                typeof(IGenericRepository<>),
                typeof(GenericRepository<>)
            );

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // =====================================
            // 4. Configure ASP.NET Core Identity
            // =====================================

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                // Password Settings
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;

                // User Settings
                options.User.RequireUniqueEmail = true;

                // Lockout Settings
                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(15);

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Sign In Settings
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // =====================================
            // 5. Configure Authentication Cookie
            // =====================================

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";

                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });

            var app = builder.Build();

            // =====================================
            // 6. Seed Default Roles
            // =====================================

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider
                    .GetRequiredService<RoleManager<IdentityRole<int>>>();

                string[] roles =
                {
                    "Employee",
                    "TeamLeader",
                    "ProjectManager",
                    "HR",
                    "CEO"
                };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        var result = await roleManager.CreateAsync(
                            new IdentityRole<int>(role)
                        );

                        if (!result.Succeeded)
                        {
                            var errors = string.Join(
                                ", ",
                                result.Errors.Select(e => e.Description)
                            );

                            throw new Exception(
                                $"Failed to create role {role}: {errors}"
                            );
                        }
                    }
                }
            }

            // =====================================
            // 7. Configure HTTP Request Pipeline
            // =====================================

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            // Authentication must come before Authorization
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
