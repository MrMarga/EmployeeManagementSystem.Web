import AxiosServices from "./AxiosServices";
import Configuration from "../Configurations/Configuration";

const axiosServices = new AxiosServices();

export default class AuthServices {
  SignUp(formData) {
    const config = {
      headers: {
        "content-type": "multipart/form-data",
      },
    };

    return axiosServices.post(Configuration.SignUp, formData, config);
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

  ForgotPassword(data) {
    return axiosServices.post(Configuration.ForgotPassword, data, {
      withCredentials: true,
    });
  }

  ResetPassword(data) {
    return axiosServices.post(Configuration.ResetPassword, data, {
      withCredentials: true,
    });
  }

  GetUserById(id) {
    return axiosServices.get(Configuration.GetUserById(id));
  }
}
