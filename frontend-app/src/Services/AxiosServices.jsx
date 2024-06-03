import axios from "axios";

const BASE_URL = "https://localhost:7298/api";

const axiosInstance = axios.create({
  baseURL: BASE_URL,
  timeout: 5000,
});

axiosInstance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default class AxiosServices {
  get(url, config = {}) {
    return axiosInstance.get(url, config);
  }

  post(url, data, config = {}) {
    return axiosInstance.post(url, data, config);
  }

  put(url, data, config = {}) {
    return axiosInstance.put(url, data, config);
  }

  delete(url, config = {}) {
    return axiosInstance.delete(url, config);
  }
}
