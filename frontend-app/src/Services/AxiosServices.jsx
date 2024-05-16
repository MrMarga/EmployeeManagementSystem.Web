import axios from "axios";

export default class AxiosServices {
  get(url, id, data, config = {}) {
    return axios.get(url, id, data, config);
  }

  post(url, id, data, config = {}) {
    return axios.post(url, id, data, config);
  }

  put(url, id, data, config = {}) {
    return axios.put(url, id, data, config);
  }

  delete(url, id, data, config = {}) {
    return axios.delete(url, id, data, config);
  }
}
