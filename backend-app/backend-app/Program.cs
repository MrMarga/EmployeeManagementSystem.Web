using Microsoft.EntityFrameworkCore;
using backend_app.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using backend_app.EmployeeRepository;
using backend_app.FilesRepository;  
using Amazon.S3;
using backend_app.EmployeeRepository.BussinessLayer;
using backend_app.UserRepository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.FileProviders;
using System;

namespace backend_app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Registering Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("Backend-DB")));

            // Add JWT authentication
            var jwtSecretKey = JwtSecretKeyGenerator.GenerateJwtSecretKey();
            builder.Configuration["Jwt:Key"] = jwtSecretKey;

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            Console.WriteLine(jwtSecretKey);

            // Injecting Dependencies
            builder.Services.AddScoped<IUserServices, UserService>();
            builder.Services.AddScoped<IEmployeeCRUD, EmployeeCRUD>();
            builder.Services.AddScoped<IFileServices, FileServices>(); // Register IFileServices and its implementation

            // Configure AWS services
            builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions("AWS"));
            builder.Services.AddAWSService<IAmazonS3>();

            var app = builder.Build();

            // Configure logging
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<Program>();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Enable CORS before authentication and authorization
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            // Enable authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            var fileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "uploads"));
            var requestPath = "/Uploads";

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = fileProvider,
                RequestPath = requestPath
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = fileProvider,
                RequestPath = requestPath
            });

            // Map logout endpoint
            app.Map("/api/auth/logout", app =>
            {
                app.Run(async context =>
                {
                    await context.SignOutAsync();
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
