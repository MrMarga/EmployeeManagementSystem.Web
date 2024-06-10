import axios from "axios";
import { jwtDecode } from "jwt-decode";

const BASE_URL = "https://localhost:7298/api";

const axiosInstance = axios.create({
  baseURL: BASE_URL,
});

axiosInstance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("AccessToken");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

axiosInstance.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    const originalRequest = error.config;

    if (error.response.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      const refreshToken = localStorage.getItem("RefreshToken");
      const deviceId = localStorage.getItem("deviceId");

      try {
        const response = await axios.post(`${BASE_URL}/Auth/refresh-token`, {
          refreshToken,
          deviceId,
        });

        const newAccessToken = response.data.accessToken;

        const getTokenExpiration = (accessToken) => {
          try {
            const decoded = jwtDecode(accessToken);
            console.log(decoded);
            if (decoded && decoded.exp) {
              return decoded.exp;
            }
            return null;
          } catch (error) {
            console.error("Invalid token", error);
            return null;
          }
        };

        const expTime = getTokenExpiration(newAccessToken);

        const decodeExpirationTime = (expTime) => {
          const date = new Date(expTime * 1000);
          return date.toLocaleString();
        };

        const expirationTime = decodeExpirationTime(expTime);
        console.log("Expiration Date:", expirationTime);
        localStorage.setItem("ExpTime", expirationTime);

        console.log(newAccessToken);

        // Update the access token in localStorage
        localStorage.setItem("AccessToken", newAccessToken);

        // Modify the original request's Authorization header with the new token
        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;

        // Retry the original request
        return axiosInstance(originalRequest);
      } catch (refreshError) {
        console.error("Failed to refresh access token:", refreshError);
        // Perform logout or redirect to login page
      }
    }

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
