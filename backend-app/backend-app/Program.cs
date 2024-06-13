using backend_app.Data;
using backend_app.EmployeeRepository.BussinessLayer;
using backend_app.EmployeeRepository;
using backend_app.FilesRepository;
using backend_app.UserRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


namespace backend_app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register services
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pappa´s API", Version = "v1" });

                // Define the OAuth2.0 scheme that's in use (i.e., Implicit Flow)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            // Registering Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("Backend-DB")));

            // Add JWT authentication
            var jwtSecretKey = JwtSecretKeyGenerator.GenerateJwtSecretKey();
            builder.Configuration["Jwt:Key"] = jwtSecretKey;

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Injecting Dependencies
            builder.Services.AddScoped<IUserServices, UserService>();
            builder.Services.AddScoped<IEmployeeCRUD, EmployeeCRUD>();
            builder.Services.AddScoped<IFileServices, FileServices>();

            // Enabling Directory Access
            builder.Services.AddDirectoryBrowser();
            var app = builder.Build();

            // Retrieve IWebHostEnvironment from the app
            var environment = app.Services.GetRequiredService<IWebHostEnvironment>();

            // Configure the HTTP request pipeline
            if (environment.IsDevelopment())
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

            var fileProvider = new PhysicalFileProvider(Path.Combine(environment.ContentRootPath, "uploads"));
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

            app.Run();
        }
    }
}
