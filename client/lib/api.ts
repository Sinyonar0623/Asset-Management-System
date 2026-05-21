import axios, { AxiosHeaders } from "axios";
import { AUTH_SESSION_COOKIE_KEY, LOCAL_STORAGE_KEY } from "./auth";

const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL?.trim();

if (!apiBaseUrl) {
  throw new Error(
    "Missing NEXT_PUBLIC_API_BASE_URL. Define it in client/.env.local (e.g. NEXT_PUBLIC_API_BASE_URL=http://localhost:5176)."
  );
}

const api = axios.create({
  baseURL: apiBaseUrl,
  headers: {
    "Content-Type": "application/json",
  },
});

function clearBrowserSession() {
  localStorage.removeItem(LOCAL_STORAGE_KEY);
  document.cookie = `${AUTH_SESSION_COOKIE_KEY}=; path=/; max-age=0; samesite=lax`;
}

api.interceptors.request.use((config) => {
  if (typeof window === "undefined") return config;

  try {
    const raw = localStorage.getItem(LOCAL_STORAGE_KEY);
    if (!raw) return config;

    const parsed = JSON.parse(raw) as {
      accessToken?: string;
      tokenType?: string;
    };

    if (!parsed.accessToken) return config;

    const authHeader = `${parsed.tokenType ?? "Bearer"} ${parsed.accessToken}`;
    const headers = AxiosHeaders.from(config.headers);
    headers.set("Authorization", authHeader);
    config.headers = headers;
  } catch {
    localStorage.removeItem(LOCAL_STORAGE_KEY);
  }

  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (typeof window === "undefined") return Promise.reject(error);

    const status = error?.response?.status;
    const requestUrl = error?.config?.url as string | undefined;
    const isLoginRequest = requestUrl?.includes("/auth/login") ?? false;

    if (status === 401 && !isLoginRequest) {
      clearBrowserSession();

      if (window.location.pathname !== "/login") {
        window.location.assign("/login");
      }
    }

    return Promise.reject(error);
  }
);

export default api;
