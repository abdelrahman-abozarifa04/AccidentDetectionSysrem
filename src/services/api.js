import axios from "axios";
import { mockApi } from "./mockApi";

// Use environment variable for API base URL - supports local/production switching
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5070/api";

// Flag to use mock API instead of real backend
const USE_MOCK_API = false; // Mock auth, real AI backend

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000, // 30 second timeout for file uploads
});

// Request interceptor - adds auth token to requests
api.interceptors.request.use((config) => {
  // Check sessionStorage first, then localStorage
  let token = sessionStorage.getItem("token");
  
  if (!token) {
    token = localStorage.getItem("token");
  }

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  // Log requests for debugging
  console.log(`[API Request] ${config.method?.toUpperCase()} ${config.url}`, config);

  return config;
});

// Response interceptor - handles errors and logging
api.interceptors.response.use(
  (response) => {
    console.log(`[API Response] ${response.status} ${response.config.url}`, response.data);
    return response;
  },
  (error) => {
    console.error('[API Error]', {
      message: error.message,
      url: error.config?.url,
      method: error.config?.method,
      status: error.response?.status,
      statusText: error.response?.statusText,
      data: error.response?.data,
    });

    if (error.response?.status === 401) {
      // Clear both storage locations
      sessionStorage.removeItem("token");
      localStorage.removeItem("token");
      
      // Only redirect if not already on signin page
      if (!window.location.pathname.includes('/signin')) {
        window.location.href = "/signin";
      }
    }
    return Promise.reject(error);
  }
);



export default api;
