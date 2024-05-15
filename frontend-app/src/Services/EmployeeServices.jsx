import AxiosServices from "./AxiosServices";
import Configuration from "../Configurations/Configuration";

const axiosServices = new AxiosServices();

export default class EmployeeServices {
  GetAllEmployee() {
    return axiosServices.get(Configuration.GetAllEmployee);
  }

  GetEmployeeById(id) {
    return axiosServices.get(Configuration.GetEmployeeById(id));
  }

  AddEmployee(data) {
    return axiosServices.post(Configuration.AddEmployee, data);
  }

  UpdateEmployee(id, data) {
    return axiosServices.put(Configuration.UpdateEmployee(id), data);
  }

  DeleteEmployee(id) {
    return axiosServices.delete(Configuration.DeleteEmployee(id));
  }
}
