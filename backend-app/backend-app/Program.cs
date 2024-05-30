using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using backend_app.Data;
using Microsoft.AspNetCore.Authentication;
using backend_app.EmployeeRepository;
using backend_app.UserRepository;
using backend_app.EmployeeRepository.BussinessLayer;

using Twileloop.Timezone;

namespace backend_app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Registering Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("Backend-DB")));

            // Configure cookie settings
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
            });

            // Configure authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/api/auth/login";
                    options.AccessDeniedPath = "/api/auth/accessdenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                    options.SlidingExpiration = true;
                });

            // Injecting Dependencies
            builder.Services.AddScoped<IUserServices, UserService>();
            builder.Services.AddScoped<IEmployeeCRUD, EmployeeCRUD>();

            var app = builder.Build();

            // Configure logging
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<Program>();

            var utcTime  = DateTime.UtcNow;
            var NepalTime = utcTime.UtcToSystemTimezone();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Enable authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Enable CORS
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            // Map logout endpoint
            app.Map("/api/auth/logout", app =>
            {
                app.Run(async context =>
                {
                    // Sign out the user
                    await context.SignOutAsync();

                    // Redirect to login page or any other page after logout
                    context.Response.Redirect("/api/auth/login");
                });
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Log information about startup
            logger.LogInformation("Application started.");

            app.Run();
        }
    }
}
