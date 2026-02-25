// API client for Asset Management System backend
const API_BASE = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

const TOKEN_KEY = "ce_asset_access_token";

export function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(TOKEN_KEY);
}

export function setToken(token: string): void {
  localStorage.setItem(TOKEN_KEY, token);
}

export function removeToken(): void {
  localStorage.removeItem(TOKEN_KEY);
}

async function request<T>(
  path: string,
  options: RequestInit = {}
): Promise<T> {
  const token = getToken();
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    ...(options.headers as Record<string, string>),
  };
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const res = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers,
  });

  if (!res.ok) {
    let errorMsg = `HTTP ${res.status}`;
    try {
      const data = await res.json();
      errorMsg = data.message || errorMsg;
    } catch {}
    throw new Error(errorMsg);
  }

  if (res.status === 204) return undefined as T;

  return res.json();
}

// ─── Auth ────────────────────────────────────────────────────────────────────

export interface LoginResponse {
  accessToken: string;
  tokenType: string;
  expiresInSeconds: number;
  userId: string;
  username: string;
  email: string;
  roleCode: string;
  roleName: string;
}

export interface MeResponse {
  userId: string;
  username: string;
  email: string;
  roleCode: string;
  roleName: string;
}

export const authApi = {
  login: (email: string, password: string) =>
    request<LoginResponse>("/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    }),

  logout: () =>
    request<void>("/auth/logout", { method: "POST" }),

  me: () => request<MeResponse>("/auth/me"),

  signUp: (username: string, email: string, password: string, roleCode: string) =>
    request<{ userId: string }>("/auth/signup/user", {
      method: "POST",
      body: JSON.stringify({ username, email, password, roleCode }),
    }),
};

// ─── Assets ──────────────────────────────────────────────────────────────────

export interface AssetSummary {
  id: number;
  realWorldId: string;
  brand: string;
  name: string;
  serialNo: string;
  type: string;
  status: string;
  amount: number;
  laboratoryName?: string;
  roomNo?: string;
}

export interface AssetDetail {
  id: number;
  realWorldId: string;
  brand: string;
  name: string;
  serialNo: string;
  description: string;
  type: string;
  status: string;
  amount: number;
  remark: string;
  ownerId: string;
  laboratory?: {
    id: number;
    laboratoryName: string;
    roomNo: string;
    teacherId: string;
    description: string;
  };
  components: Array<{
    id: number;
    realWorldId: string;
    brand: string;
    name: string;
    serialNo: string;
    description: string;
    type: string;
    remark: string;
  }>;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CreateAssetPayload {
  realWorldId: string;
  brand: string;
  name: string;
  serialNo: string;
  description: string;
  type: string;
  amount: number;
  remark: string;
  ownerId: string;
}

export interface UpdateAssetPayload {
  brand: string;
  name: string;
  description: string;
  type: string;
  status: string;
  amount: number;
  remark: string;
}

export const assetApi = {
  getAll: (params?: { page?: number; pageSize?: number; search?: string; type?: string; status?: string }) => {
    const query = new URLSearchParams();
    if (params?.page) query.set("page", String(params.page));
    if (params?.pageSize) query.set("pageSize", String(params.pageSize));
    if (params?.search) query.set("search", params.search);
    if (params?.type) query.set("type", params.type);
    if (params?.status) query.set("status", params.status);
    return request<PagedResult<AssetSummary>>(`/assets?${query}`);
  },

  getById: (id: number) => request<AssetDetail>(`/assets/${id}`),

  create: (payload: CreateAssetPayload) =>
    request<{ id: number }>("/assets", {
      method: "POST",
      body: JSON.stringify(payload),
    }),

  update: (id: number, payload: UpdateAssetPayload) =>
    request<void>(`/assets/${id}`, {
      method: "PUT",
      body: JSON.stringify(payload),
    }),

  delete: (id: number) =>
    request<void>(`/assets/${id}`, { method: "DELETE" }),

  getLaboratories: () =>
    request<Array<{ id: number; laboratoryName: string; roomNo: string; teacherId: string; description: string }>>("/assets/laboratories"),

  createLaboratory: (payload: { laboratoryName: string; roomNo: string; teacherId: string; description: string }) =>
    request<{ id: number }>("/assets/laboratories", {
      method: "POST",
      body: JSON.stringify(payload),
    }),
};

// ─── Borrow Requests ─────────────────────────────────────────────────────────

export interface BorrowRequest {
  id: number;
  requesterId: string;
  requesterName: string;
  assetId: number;
  assetName: string;
  assetRealWorldId: string;
  borrowDate: string;
  returnDate: string;
  purpose: string;
  status: string;
  approvalRemark?: string;
  approvedBy?: string;
  approvedAt?: string;
  actualReturnDate?: string;
  createOn?: string;
}

export interface BorrowPagedResult {
  items: BorrowRequest[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export const borrowApi = {
  getAll: (params?: { page?: number; pageSize?: number; status?: string; myOnly?: boolean }) => {
    const query = new URLSearchParams();
    if (params?.page) query.set("page", String(params.page));
    if (params?.pageSize) query.set("pageSize", String(params.pageSize));
    if (params?.status) query.set("status", params.status);
    if (params?.myOnly) query.set("myOnly", "true");
    return request<BorrowPagedResult>(`/borrow-requests?${query}`);
  },

  getById: (id: number) => request<BorrowRequest>(`/borrow-requests/${id}`),

  create: (payload: { assetId: number; assetName: string; assetRealWorldId: string; borrowDate: string; returnDate: string; purpose: string }) =>
    request<{ id: number }>("/borrow-requests", {
      method: "POST",
      body: JSON.stringify(payload),
    }),

  approve: (id: number, remark?: string) =>
    request<void>(`/borrow-requests/${id}/approve`, {
      method: "PUT",
      body: JSON.stringify({ remark }),
    }),

  reject: (id: number, remark?: string) =>
    request<void>(`/borrow-requests/${id}/reject`, {
      method: "PUT",
      body: JSON.stringify({ remark }),
    }),

  markReturned: (id: number) =>
    request<void>(`/borrow-requests/${id}/return`, { method: "PUT", body: JSON.stringify({}) }),
};

// ─── Repair Requests ─────────────────────────────────────────────────────────

export interface RepairRequest {
  id: number;
  requesterId: string;
  requesterName: string;
  assetId: number;
  assetName: string;
  assetRealWorldId: string;
  problemDescription: string;
  status: string;
  approvalRemark?: string;
  approvedBy?: string;
  approvedAt?: string;
  completedAt?: string;
  technicianNote?: string;
  createOn?: string;
}

export interface RepairPagedResult {
  items: RepairRequest[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export const repairApi = {
  getAll: (params?: { page?: number; pageSize?: number; status?: string; myOnly?: boolean }) => {
    const query = new URLSearchParams();
    if (params?.page) query.set("page", String(params.page));
    if (params?.pageSize) query.set("pageSize", String(params.pageSize));
    if (params?.status) query.set("status", params.status);
    if (params?.myOnly) query.set("myOnly", "true");
    return request<RepairPagedResult>(`/repair-requests?${query}`);
  },

  getById: (id: number) => request<RepairRequest>(`/repair-requests/${id}`),

  create: (payload: { assetId: number; assetName: string; assetRealWorldId: string; problemDescription: string }) =>
    request<{ id: number }>("/repair-requests", {
      method: "POST",
      body: JSON.stringify(payload),
    }),

  approve: (id: number, remark?: string) =>
    request<void>(`/repair-requests/${id}/approve`, {
      method: "PUT",
      body: JSON.stringify({ remark }),
    }),

  reject: (id: number, remark?: string) =>
    request<void>(`/repair-requests/${id}/reject`, {
      method: "PUT",
      body: JSON.stringify({ remark }),
    }),

  startRepair: (id: number) =>
    request<void>(`/repair-requests/${id}/start`, { method: "PUT", body: JSON.stringify({}) }),

  complete: (id: number, technicianNote?: string) =>
    request<void>(`/repair-requests/${id}/complete`, {
      method: "PUT",
      body: JSON.stringify({ technicianNote }),
    }),
};

// ─── Parameters ──────────────────────────────────────────────────────────────

export interface Parameter {
  id: number;
  group: string;
  value: string;
  description: string;
  active: boolean;
}

export const parameterApi = {
  getAll: (group?: string) => {
    const query = group ? `?group=${encodeURIComponent(group)}` : "";
    return request<Parameter[]>(`/parameters${query}`);
  },

  create: (payload: { group: string; value: string; description: string }) =>
    request<{ id: number }>("/parameters", {
      method: "POST",
      body: JSON.stringify(payload),
    }),

  disable: (id: number) =>
    request<void>(`/parameters/${id}/disable`, { method: "PUT", body: JSON.stringify({}) }),

  enable: (id: number) =>
    request<void>(`/parameters/${id}/enable`, { method: "PUT", body: JSON.stringify({}) }),

  delete: (id: number) =>
    request<void>(`/parameters/${id}`, { method: "DELETE" }),
};
