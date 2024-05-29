using backend_app.DTO;
using backend_app.Model;


namespace backend_app.EmployeeRepository
{
    public interface IEmployeeCRUD
    {
        Task<(List<Employee>, int)> GetAllEmployee(int pageNumber, int pageSize, string sortBy = "id", bool ascending = true);

        Task<Employee> GetEmployeeById(int id);

        Task AddEmployee(EmployeeDTO employeeDTO);

        Task UpdateEmployee(int id, EmployeeDTO updatedEmployee);

        Task DeleteEmployee(int id);

    }
}
