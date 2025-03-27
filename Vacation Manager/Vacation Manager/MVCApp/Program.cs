using Microsoft.EntityFrameworkCore;
using Data_Layer;
using Business_Layer;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MVCApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("VacationManagerDbContextConnection") ?? throw new InvalidOperationException("Connection string 'VacationManagerDbContextConnection' not found.");

            builder.Services.AddDbContext<VacationManagerDbContext>(options => options.UseSqlServer("Server=DESKTOP-3HGDURE\\SQLEXPRESS;Database=VacationManagerDb;Trusted_Connection=True;TrustServerCertificate=True;"));

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            }).AddEntityFrameworkStores<VacationManagerDbContext>()
            .AddDefaultTokenProviders();

            //builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<VacationManagerDbContext>();
            
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
    
            // Register the DbContext for EF Core with SQL Server.
            builder.Services.AddScoped<VacationManagerDbContext, VacationManagerDbContext>();

            builder.Services.AddScoped<ProjectContext>();
            builder.Services.AddScoped<TeamContext>();
            builder.Services.AddScoped<IdentityContext, IdentityContext>();
            builder.Services.AddScoped<RoleManager<IdentityRole>>();

            builder.Services.AddSingleton<IEmailSender, EmailSender.EmailSender>();

            // Register your ProjectContext (CRUD repository) as a scoped service.
            builder.Services.AddScoped<IDb<Project, int>, ProjectContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();
            app.Run();
        }
    }
}
