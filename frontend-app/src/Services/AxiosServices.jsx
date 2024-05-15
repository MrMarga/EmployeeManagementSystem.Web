import axios from "axios";

export default class AxiosServices {
  get(url, id, data, IsRequired = false, Header) {
    return axios.get(url, id, data, IsRequired && Header);
  }

  post(url, id, data, IsRequired = false, Header) {
    return axios.post(url, id, data, IsRequired && Header);
  }

  put(url, id, data, IsRequired = false, Header) {
    return axios.put(url, id, data, IsRequired && Header);
  }

  delete(url, id, data, IsRequired = false, Header) {
    return axios.delete(url, id, data, IsRequired && Header);
  }
}
