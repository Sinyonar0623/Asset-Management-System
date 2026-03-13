// app/lib/auth.ts
// Single source of truth for all auth types, mock users, and permission logic

export type Role = "student" | "lecturer" | "depthead" | "admin";

export interface MockUser {
  id: string;
  username: string;
  password: string;
  name: string;
  email: string;
  role: Role;
  department: string;
  ownedAssetIds: string[];
}

export interface AuthSession {
  userId: string;
  username: string;
  name: string;
  email: string;
  role: Role;
  department: string;
  ownedAssetIds: string[];
  accessToken?: string;
  tokenType?: string;
  expiresAt?: number;
}

// ─── Permission functions ────────────────────────────────────────────────────
// All UI guards and route guards call these — single place to change rules
export const PERMISSIONS = {
  canViewAssets:   (_role: Role) => true,
  canAddAsset:     (role: Role) => role === "lecturer" || role === "depthead" || role === "admin",
  canCreateBorrow: (_role: Role) => true,
  canCreateRepair: (role: Role) => role === "lecturer" || role === "depthead" || role === "admin",
  canApprove:      (role: Role) => role === "lecturer" || role === "depthead" || role === "admin",
  canApproveAll:   (role: Role) => role === "depthead" || role === "admin",
  canManageUsers:  (role: Role) => role === "admin",
  canViewReports:  (role: Role) => role === "depthead" || role === "admin",

  // Complex: lecturer can approve only assets they own; depthead/admin can approve all
  canApproveForAsset: (
    role: Role,
    assetOwnerId: string | null,
    userId: string
  ): boolean => {
    if (role === "depthead" || role === "admin") return true;
    if (role === "lecturer") {
      // Lecturer can approve assets they own, OR assets with no owner (dept assets)
      return assetOwnerId === userId || assetOwnerId === null;
    }
    return false;
  },
} as const;

// ─── Mock Users ──────────────────────────────────────────────────────────────
export const MOCK_USERS: MockUser[] = [
  {
    id: "user-001",
    username: "student1",
    password: "pass123",
    name: "นายสมชาย ใจดี",
    email: "somchai.j@student.ku.ac.th",
    role: "student",
    department: "วิศวกรรมคอมพิวเตอร์",
    ownedAssetIds: [],
  },
  {
    id: "user-002",
    username: "lecturer1",
    password: "pass123",
    name: "ผศ.ดร.วิชัย สุขใจ",
    email: "wichai.s@ku.ac.th",
    role: "lecturer",
    department: "วิศวกรรมคอมพิวเตอร์",
    ownedAssetIds: ["AST-001", "AST-003"],
  },
  {
    id: "user-003",
    username: "depthead1",
    password: "pass123",
    name: "รศ.ดร.สมศักดิ์ ดีงาม",
    email: "somsak.d@ku.ac.th",
    role: "depthead",
    department: "วิศวกรรมคอมพิวเตอร์",
    ownedAssetIds: [],
  },
  {
    id: "user-004",
    username: "admin1",
    password: "pass123",
    name: "นางรัตนา ช่วยดี",
    email: "rattana.c@ku.ac.th",
    role: "admin",
    department: "วิศวกรรมคอมพิวเตอร์",
    ownedAssetIds: [],
  },
];

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

export function validateLogin(username: string, password: string): AuthSession | null {
  const user = MOCK_USERS.find(
    (u) => u.username === username && u.password === password
  );
  if (!user) return null;
  return {
    userId:        user.id,
    username:      user.username,
    name:          user.name,
    email:         user.email,
    role:          user.role,
    department:    user.department,
    ownedAssetIds: user.ownedAssetIds,
  };
}

export function getUserById(id: string): MockUser | undefined {
  return MOCK_USERS.find((u) => u.id === id);
}

export function getLecturers(): MockUser[] {
  return MOCK_USERS.filter((u) => u.role === "lecturer");
}
