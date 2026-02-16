"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "../contexts/AuthContext";
import { PERMISSIONS } from "../lib/auth";

type PermissionKey = keyof Omit<typeof PERMISSIONS, "canApproveForAsset">;

interface ProtectedRouteProps {
  children: React.ReactNode;
  requiredPermission?: PermissionKey;
}

export default function ProtectedRoute({
  children,
  requiredPermission,
}: ProtectedRouteProps) {
  const { session, isLoading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!isLoading && !session) {
      router.replace("/login");
    }
  }, [session, isLoading, router]);

  // Hydrating from localStorage
  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-slate-50">
        <div className="w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  // Not logged in – redirect handled in useEffect
  if (!session) return null;

  // Logged in but insufficient permission
  if (requiredPermission && !PERMISSIONS[requiredPermission](session.role)) {
    return <ForbiddenScreen />;
  }

  return <>{children}</>;
}

function ForbiddenScreen() {
  const router = useRouter();
  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-50">
      <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-10 text-center max-w-md w-full">
        <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center text-4xl mx-auto mb-4">
          🚫
        </div>
        <h2 className="text-xl font-semibold text-slate-800 mb-2">
          ไม่มีสิทธิ์เข้าถึง
        </h2>
        <p className="text-slate-500 mb-1 text-sm">403 – Forbidden</p>
        <p className="text-slate-400 mb-8 text-sm">
          คุณไม่มีสิทธิ์ในการเข้าถึงหน้านี้ กรุณาติดต่อผู้ดูแลระบบ
        </p>
        <button
          onClick={() => router.push("/dashboard")}
          className="px-6 py-2.5 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 transition-colors"
        >
          กลับหน้าหลัก
        </button>
      </div>
    </div>
  );
}
