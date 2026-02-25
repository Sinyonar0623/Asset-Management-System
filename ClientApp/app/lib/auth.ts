// app/lib/auth.ts
// Single source of truth for all auth types and permission logic

export type Role = "student" | "lecturer" | "depthead" | "admin";

export interface AuthSession {
  userId: string;
  username: string;
  name: string;
  email: string;
  role: Role;
  roleCode: string;
  roleName: string;
}

// Map backend role codes to frontend roles
// 00 = admin, 01 = depthead, 02 = lecturer, 03 = student
export function rolecodeToRole(roleCode: string): Role {
  switch (roleCode) {
    case "00": return "admin";
    case "01": return "depthead";
    case "02": return "lecturer";
    case "03": return "student";
    default: return "student";
  }
}

// ─── Permission functions ────────────────────────────────────────────────────
export const PERMISSIONS = {
  canViewAssets:   (_role: Role) => true,
  canAddAsset:     (role: Role) => role === "lecturer" || role === "depthead" || role === "admin",
  canCreateBorrow: (_role: Role) => true,
  canCreateRepair: (role: Role) => role === "lecturer" || role === "depthead" || role === "admin",
  canApprove:      (role: Role) => role === "lecturer" || role === "depthead" || role === "admin",
  canApproveAll:   (role: Role) => role === "depthead" || role === "admin",
  canManageUsers:  (role: Role) => role === "admin",
  canViewReports:  (role: Role) => role === "depthead" || role === "admin",

  canApproveForAsset: (
    role: Role,
    assetOwnerId: string | null,
    userId: string
  ): boolean => {
    if (role === "depthead" || role === "admin") return true;
    if (role === "lecturer") {
      return assetOwnerId === userId || assetOwnerId === null;
    }
    return false;
  },
} as const;

// ─── Role display labels ─────────────────────────────────────────────────────
export const ROLE_LABELS: Record<Role, string> = {
  student:  "นิสิต",
  lecturer: "อาจารย์",
  depthead: "หัวหน้าภาควิชา",
  admin:    "ผู้ดูแลระบบ",
};

export const ROLE_COLORS: Record<Role, string> = {
  student:  "bg-blue-100 text-blue-700",
  lecturer: "bg-purple-100 text-purple-700",
  depthead: "bg-red-100 text-red-700",
  admin:    "bg-orange-100 text-orange-700",
};

// ─── Helpers ─────────────────────────────────────────────────────────────────
export const LOCAL_STORAGE_KEY = "ce_asset_auth_session";
