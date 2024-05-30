const Configuration = {
  SignUp: "https://localhost:7298/api/Auth/signup",
  Login: "https://localhost:7298/api/Auth/login",
  Logout: "https://localhost:7298/api/Auth/logout",
  ForgotPassword: "https://localhost:7298/api/Auth/forgot-password",
  ResetPassword: "https://localhost:7298/api/Auth/reset-password",

  //-----------------------EmployeeCRUD-----------------//

  GetAllEmployee: (pageNumber, pageSize) =>
    `https://localhost:7298/api/Employee/GetAllEmployee?pageNumber=${pageNumber}&pageSize=${pageSize}`,

  GetEmployeeById: (id) =>
    `https://localhost:7298/api/Employee/GetEmployeeById/${id}`,

  AddEmployee: "https://localhost:7298/api/Employee/AddEmployee",

  UpdateEmployee: (id) =>
    `https://localhost:7298/api/Employee/UpdateEmployee/${id}/update`,

  DeleteEmployee: (id) =>
    `https://localhost:7298/api/Employee/DeleteEmployee/${id}`,
};

export default Configuration;
