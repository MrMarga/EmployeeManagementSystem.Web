using backend_app.DTO;
using backend_app.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend_app.EmployeeRepository
{
    public interface IEmployeeCRUD
    {
        Task<List<Employee>> GetAllEmployee();

        Task<Employee> GetEmployeeById(int id);

        Task AddEmployee(EmployeeDTO employeeDTO);

        Task UpdateEmployee(int id, EmployeeDTO updatedEmployee);

        Task DeleteEmployee(int id);

    }
}
