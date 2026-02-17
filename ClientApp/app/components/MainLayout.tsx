"use client";

import Sidebar from "./Sidebar";
import ProtectedRoute from "./ProtectedRoute";

export default function MainLayout({ children }: { children: React.ReactNode }) {
  return (
    <ProtectedRoute>
      <div className="flex min-h-screen bg-slate-50">
        <Sidebar />
        <main className="flex-1 flex flex-col min-w-0">
          {children}
        </main>
      </div>
    </ProtectedRoute>
  );
}
