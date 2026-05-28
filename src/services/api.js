import axios from "axios";

// Environment variables for local/production switching
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5070/api";
const AI_API_BASE_URL = import.meta.env.VITE_AI_API_BASE_URL || "http://localhost:8000";

// ============================================
// 1. STANDARD .NET BACKEND API CLIENT (api)
// ============================================
const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000, // 30 second timeout
});

// Request interceptor - adds auth token to requests
api.interceptors.request.use((config) => {
  let token = sessionStorage.getItem("token") || localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  console.log(`[NET API Request] ${config.method?.toUpperCase()} ${config.url}`, config);
  return config;
}, (error) => {
  return Promise.reject(error);
});

// Response interceptor - handles logs and 401 redirects
api.interceptors.response.use(
  (response) => {
    console.log(`[NET API Response] ${response.status} ${response.config.url}`, response.data);
    return response;
  },
  (error) => {
    console.error('[NET API Error]', {
      message: error.message,
      url: error.config?.url,
      method: error.config?.method,
      status: error.response?.status,
      data: error.response?.data,
    });

    if (error.response?.status === 401) {
      sessionStorage.removeItem("token");
      localStorage.removeItem("token");
      if (!window.location.pathname.includes('/signin')) {
        window.location.href = "/signin";
      }
    }
    return Promise.reject(error);
  }
);

// ============================================
// 2. STANDALONE FastAPI AI SERVICE CLIENT (aiApi)
// ============================================
const aiApi = axios.create({
  baseURL: AI_API_BASE_URL,
  timeout: 60000, // Extended 60 second timeout for model initialization & loading
});

// Request interceptor for AI calls
aiApi.interceptors.request.use((config) => {
  // Let the browser/Axios automatically set the multipart/form-data boundary when uploading file streams.
  // Setting a custom Content-Type: multipart/form-data manually will corrupt the envelope boundary.
  if (config.data instanceof FormData) {
    delete config.headers['Content-Type'];
  }

  // HuggingFace Spaces might run anonymously, but if we need a token for authorization, we can check.
  // We make token attachment optional so it doesn't break public spaces.
  let token = sessionStorage.getItem("token") || localStorage.getItem("token");
  if (token && config.headers.injectToken !== false) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  console.log(`[AI API Request] ${config.method?.toUpperCase()} ${config.url}`, {
    url: config.url,
    headers: config.headers
  });
  return config;
}, (error) => {
  return Promise.reject(error);
});

// Response interceptor for AI calls
aiApi.interceptors.response.use(
  (response) => {
    console.log(`[AI API Response] ${response.status} ${response.config.url}`, response.data);
    return response;
  },
  (error) => {
    console.error('[AI API Error]', {
      message: error.message,
      url: error.config?.url,
      status: error.response?.status,
      data: error.response?.data,
    });
    return Promise.reject(error);
  }
);

export { aiApi };
export default api;
