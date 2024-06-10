using backend_app.DTO;
using backend_app.EmployeeRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace backend_app.Controllers
{
    [Authorize]
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeCRUD _employeeCRUD;

        public EmployeeController(IEmployeeCRUD employeeCRUD)
        {
            _employeeCRUD = employeeCRUD;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeCRUD.GetEmployeeById(id);
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        
        [HttpPut("{id}/update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeDTO updatedEmployee)
        {
            var existingEmployee = await _employeeCRUD.GetEmployeeById(id);
            if (existingEmployee == null)
                return NotFound();

            existingEmployee.FullName = updatedEmployee.FullName;
            existingEmployee.Phone = updatedEmployee.Phone;
            existingEmployee.Email = updatedEmployee.Email;

            await _employeeCRUD.UpdateEmployee(id, updatedEmployee);

            return NoContent();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllEmployee([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (employees, totalEmployees) = await _employeeCRUD.GetAllEmployee(pageNumber, pageSize);

            var response = new
            {
                TotalCount = totalEmployees,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Employees = employees
            };

            return Ok(response);
        }

       
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEmployee(EmployeeDTO employee)
        {
            await _employeeCRUD.AddEmployee(employee);
            return CreatedAtAction(nameof(GetAllEmployee), null);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var existingEmployee = await _employeeCRUD.GetEmployeeById(id);
            if (existingEmployee == null)
                return NotFound();

            await _employeeCRUD.DeleteEmployee(id);
            return NoContent();
        }

        
    }
}