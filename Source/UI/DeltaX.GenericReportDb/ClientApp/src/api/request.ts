import axios from "axios";
import { API_URL, API_TIMEOUT } from "@/config";

const request = axios.create({
    baseURL: API_URL,
    timeout: API_TIMEOUT,
    withCredentials: true
});

request.interceptors.request.use(
    config => {
        const accessToken = localStorage.getItem("access_token");
        if (accessToken) {
            config.headers.Authorization = `Bearer ${accessToken}`;
        }
        return config;
    },
    error => Promise.reject(error)
);

export default request;
