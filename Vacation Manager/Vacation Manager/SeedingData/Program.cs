using Business_Layer;
using Data_Layer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Data;

namespace SeedindDataConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            await EnsureRolesCreated(serviceProvider);
            await EnsureAdminCreated(serviceProvider);
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    services.AddDbContext<VacationManagerDbContext>(options =>
                        options.UseSqlServer("Server=DESKTOP-3HGDURE\\SQLEXPRESS;Database=VacationManagerDb;Trusted_Connection=True;TrustServerCertificate=True;"));
                    services.AddIdentity<User, IdentityRole>()
                        .AddEntityFrameworkStores<VacationManagerDbContext>()
                        .AddDefaultTokenProviders();
                });

        static async Task EnsureRolesCreated(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in Enum.GetNames(typeof(Role)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"Role {role} created.");
                }
            }
        }

        static async Task EnsureAdminCreated(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            string adminEmail = "admin@example.com";
            string adminPassword = "Admin@1234";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    Name = "Admin",
                    Surname = "Admin",
                    EmailConfirmed = true
                    // Optionally assign a Team if one exists:
                    // Team = someTeam,
                    // TeamId = someTeam?.Id,
                    // Vacations will be initialized by the constructor.
                };


                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Role.CEO.ToString());
                    Console.WriteLine("Admin user created.");
                }
                else
                {
                    Console.WriteLine("Failed to create admin user:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
            }
            else
            {
                Console.WriteLine("Admin user already exists.");
            }


        }

    }

}
