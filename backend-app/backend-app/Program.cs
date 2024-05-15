
using backend_app.Data;
using backend_app.DataAccessLayer;
using backend_app.EmployeeRepository;
using backend_app.EmployeeRepository.BussinessLayer;
using Microsoft.EntityFrameworkCore;

namespace backend_app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Registering Database
            builder.Services.AddDbContext<ApplicationDbContext>(option => 
            option.UseNpgsql(builder.Configuration.GetConnectionString("Backend-DB")));
            
            //Injectiong Dependecies
            builder.Services.AddScoped<IAuthentication, Authentication>();
            builder.Services.AddScoped<IEmployeeCRUD, EmployeeCRUD>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Configure CORS
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });

            app.MapControllers();

            app.Run();
        }
    }
}
