const Configuration = {
  SignUp: "https://localhost:7298/api/Backend/SignUp",
  Login: "https://localhost:7298/api/Backend/Login",

  //-----------------------EmployeeCRUD-----------------//

  GetAllEmployee: "https://localhost:7298/api/Employee/GetAllEmployee",

  GetEmployeeById: (id) =>
    `https://localhost:7298/api/Employee/GetEmployeeById/${id}`,

  AddEmployee: "https://localhost:7298/api/Employee/AddEmployee",

  UpdateEmployee: (id) =>
    `https://localhost:7298/api/Employee/UpdateEmployee/${id}/update`,

  DeleteEmployee: (id) =>
    `https://localhost:7298/api/Employee/DeleteEmployee/${id}`,
};

export default Configuration;
