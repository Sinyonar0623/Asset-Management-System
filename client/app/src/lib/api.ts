import axios, { AxiosHeaders } from "axios";
import { LOCAL_STORAGE_KEY } from "./auth";

const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL

const api = axios.create({
  baseURL: apiBaseUrl,
  headers: {
    "Content-Type": "application/json",
  },
});

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

export default api;
