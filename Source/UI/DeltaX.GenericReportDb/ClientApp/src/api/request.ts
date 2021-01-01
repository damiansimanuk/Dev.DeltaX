import axios from "axios";
import { API_URL, API_TIMEOUT } from "@/config";

console.log("axios.create", API_URL)
const request = axios.create({
  baseURL: API_URL,
  timeout: API_TIMEOUT,
  withCredentials: true
});

request.interceptors.request.use(
  config => {
    const accessToken = localStorage.getItem("accessToken");
    console.log("axios accessToken", accessToken)
    if (accessToken) {
      config.headers.Authorization = `Bearer ${accessToken}`;
    }
    return config;
  },
  error => Promise.reject(error)
);

export default request;
