using backend_app.Data;
using backend_app.DTO;
using backend_app.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace backend_app.EmployeeRepository.BussinessLayer
{
    public class EmployeeCRUD : IEmployeeCRUD
    {
        protected readonly ApplicationDbContext dBContext;

        public EmployeeCRUD(ApplicationDbContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public async Task<(List<Employee>, int)> GetAllEmployee(int pageNumber, int pageSize, string sortBy = "id", bool ascending = true)
        {
            var query = dBContext.Employees.AsQueryable();

            // Sorting expression
            Expression<Func<Employee, object>> orderByExpression = sortBy switch
            {
                "id" => e => e.Id,
                "fullName" => e => e.FullName,
                "email" => e => e.Email,
                "phone" => e => e.Phone,
                _ => e => e.FullName, // Default to sorting by ID
            };

            // Apply sorting
            query = ascending ? query.OrderBy(orderByExpression) : query.OrderByDescending(orderByExpression);

            var totalEmployees = await query.CountAsync();
            var employees = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            return (employees, totalEmployees);
        }



        public async Task<Employee> GetEmployeeById(int id)
        {
            return await dBContext.Employees.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddEmployee(EmployeeDTO employeeDTO)
        {
            var employee = new Employee
            {
                FullName = employeeDTO.FullName,
                Phone = employeeDTO.Phone,
                Email = employeeDTO.Email
            };

            await dBContext.Employees.AddAsync(employee);
            await dBContext.SaveChangesAsync();
        }

        public async Task UpdateEmployee(int id, EmployeeDTO updatedEmployee)
        {
            var employee = await dBContext.Employees.FindAsync(id);

            if (employee != null)
            {
                // Update the employee details
                employee.FullName = updatedEmployee.FullName;
                employee.Phone = updatedEmployee.Phone;
                employee.Email = updatedEmployee.Email;

                // Save changes to the database
                await dBContext.SaveChangesAsync();
            }
        }

        public async Task DeleteEmployee(int id)
        {
            var employee = await dBContext.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (employee != null)
            {
                dBContext.Employees.Remove(employee);
                await dBContext.SaveChangesAsync();
            }
        }

        // Other methods remain unchanged
    }
}
