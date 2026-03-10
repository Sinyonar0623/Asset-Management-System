import api from "./api";
import { Role } from "./auth";

export interface LoginApiRequest {
  email: string;
  password: string;
}

export interface LoginApiResponse {
  accessToken: string;
  tokenType: string;
  expiresIn: number;
  userId: string;
  username: string;
  email: string;
  roleCode: string;
  roleName: string;
}

const roleByCode: Record<string, Role> = {
  "00": "admin",
  "01": "depthead",
  "02": "lecturer",
  "03": "student",
};

export function mapRoleCodeToRole(roleCode: string): Role {
  const role = roleByCode[roleCode];
  if (!role) {
    throw new Error(`Unknown role code: ${roleCode}`);
  }
  return role;
}

export async function loginApi(payload: LoginApiRequest): Promise<LoginApiResponse> {
  const { data } = await api.post<LoginApiResponse>("/auth/login", payload);
  return data;
}

export async function logoutApi(): Promise<void> {
  await api.post("/auth/logout");
}
