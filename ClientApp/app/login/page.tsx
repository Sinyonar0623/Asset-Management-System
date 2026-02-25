"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "../contexts/AuthContext";
import { ROLE_LABELS } from "../lib/auth";

const TEST_ACCOUNTS = [
  { email: "admin@ce.ku.ac.th",    role: "admin"    as const },
  { email: "depthead@ce.ku.ac.th", role: "depthead" as const },
  { email: "lecturer@ce.ku.ac.th", role: "lecturer" as const },
  { email: "student@ce.ku.ac.th",  role: "student"  as const },
];

export default function LoginPage() {
  const { login, session, isLoading } = useAuth();
  const router = useRouter();
  const [form, setForm] = useState({ email: "", password: "" });
  const [error, setError] = useState("");
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (!isLoading && session) {
      router.replace("/dashboard");
    }
  }, [session, isLoading, router]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setError("");
    const result = await login(form.email.trim(), form.password);
    if (!result.ok) {
      setError(result.error || "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง");
      setSubmitting(false);
    }
  };

  const quickFill = (email: string) => {
    setForm({ email, password: "Admin@1234" });
    setError("");
  };

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-slate-100">
        <div className="w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-100 p-4">
      <div className="w-full max-w-sm">
        <div className="text-center mb-8">
          <div className="w-14 h-14 bg-blue-600 rounded-2xl flex items-center justify-center text-white font-bold text-xl mx-auto mb-4 shadow-lg">AM</div>
          <h1 className="text-2xl font-bold text-slate-800">Asset Management</h1>
          <p className="text-slate-500 text-sm mt-1">ภาควิชาวิศวกรรมคอมพิวเตอร์ มหาวิทยาลัยศรีนครินทรวิโรฒ</p>
        </div>

        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-8">
          <h2 className="text-lg font-semibold text-slate-800 mb-6">เข้าสู่ระบบ</h2>
          {error && (
            <div className="mb-4 px-4 py-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700 flex items-center gap-2">
              <span>⚠️</span>{error}
            </div>
          )}
          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1.5">อีเมล (Email)</label>
              <input type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })}
                required autoComplete="email" placeholder="กรอกอีเมล"
                className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1.5">รหัสผ่าน (Password)</label>
              <input type="password" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })}
                required autoComplete="current-password" placeholder="กรอกรหัสผ่าน"
                className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
            </div>
            <button type="submit" disabled={submitting}
              className="w-full py-2.5 bg-blue-600 text-white rounded-lg text-sm font-semibold hover:bg-blue-700 transition-colors disabled:opacity-60 mt-2">
              {submitting ? "กำลังเข้าสู่ระบบ..." : "เข้าสู่ระบบ"}
            </button>
          </form>
        </div>

        <div className="mt-4 bg-white rounded-2xl border border-amber-200 shadow-sm p-5">
          <div className="flex items-center gap-2 mb-3">
            <span className="text-amber-500">🧪</span>
            <span className="text-xs font-semibold text-amber-700 uppercase tracking-wide">บัญชีทดสอบ (Dev Mode)</span>
          </div>
          <p className="text-xs text-slate-400 mb-3">คลิกเพื่อกรอกข้อมูลอัตโนมัติ</p>
          <div className="space-y-1.5">
            {TEST_ACCOUNTS.map((acc) => (
              <button key={acc.email} type="button" onClick={() => quickFill(acc.email)}
                className="w-full flex items-center justify-between px-3 py-2 rounded-lg text-sm hover:bg-slate-50 border border-slate-100 transition-colors group">
                <span className="font-medium text-slate-700 group-hover:text-blue-600 truncate text-left">{acc.email}</span>
                <span className={`text-xs px-2 py-0.5 rounded-full font-medium shrink-0 ml-2 ${
                  acc.role === "student" ? "bg-blue-100 text-blue-600" :
                  acc.role === "lecturer" ? "bg-purple-100 text-purple-600" :
                  acc.role === "depthead" ? "bg-red-100 text-red-600" : "bg-orange-100 text-orange-600"
                }`}>{ROLE_LABELS[acc.role]}</span>
              </button>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
