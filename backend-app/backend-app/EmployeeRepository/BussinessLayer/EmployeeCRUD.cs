using backend_app.Data;
using backend_app.DTO;
using backend_app.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_app.EmployeeRepository.BussinessLayer
{
    public class EmployeeCRUD : IEmployeeCRUD
    {
        protected readonly ApplicationDbContext dBContext;

        public EmployeeCRUD(ApplicationDbContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public async Task<List<Employee>> GetAllEmployee()
        {
            return await dBContext.Employees.ToListAsync();
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
