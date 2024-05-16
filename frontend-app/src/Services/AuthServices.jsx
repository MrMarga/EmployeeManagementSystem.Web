import AxiosServices from "./AxiosServices";
import Configuration from "../Configurations/Configuration";

const axiosServices = new AxiosServices();

export default class AuthServices {
  SignUp(data) {
    return axiosServices.post(Configuration.SignUp, data, {
      withCredentials: true,
    });
  }

  Login(data) {
    return axiosServices.post(Configuration.Login, data, {
      withCredentials: true,
    });
  }

  Logout(data) {
    return axiosServices.post(Configuration.Logout, data, {
      withCredentials: true,
    });
  }
}
