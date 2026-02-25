"use client";
import { useState } from "react";
import MainLayout from "../components/MainLayout";
import Header from "../components/Header";
import { authApi } from "../lib/api";

const ROLE_OPTIONS = [
  { code: "03", label: "นิสิต (Student)" },
  { code: "02", label: "อาจารย์ (Lecturer)" },
  { code: "01", label: "หัวหน้าภาควิชา (Dept Head)" },
  { code: "00", label: "ผู้ดูแลระบบ (Admin)" },
];

export default function UsersPage() {
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({ username: "", email: "", password: "", roleCode: "03" });
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState("");
  const [successMsg, setSuccessMsg] = useState("");

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setFormError("");
    try {
      await authApi.signUp(form.username, form.email, form.password, form.roleCode);
      setSuccessMsg(`เพิ่มผู้ใช้ ${form.username} สำเร็จแล้ว`);
      setShowForm(false);
      setForm({ username: "", email: "", password: "", roleCode: "03" });
    } catch (err) {
      setFormError((err as Error).message || "เกิดข้อผิดพลาด");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <MainLayout>
      <Header
        title="User Management"
        subtitle="จัดการบัญชีผู้ใช้งานในระบบ"
        actions={
          <button
            onClick={() => { setShowForm(!showForm); setFormError(""); setSuccessMsg(""); }}
            className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700"
          >
            <span>+</span> Add User
          </button>
        }
      />

      <div className="flex-1 p-6 space-y-6">
        {successMsg && (
          <div className="bg-green-50 border border-green-200 rounded-xl p-4 flex items-center gap-3">
            <span className="text-green-500">✅</span>
            <span className="font-medium text-green-800 text-sm">{successMsg}</span>
            <button onClick={() => setSuccessMsg("")} className="ml-auto text-green-400">✕</button>
          </div>
        )}

        {showForm && (
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-6 py-4 border-b border-slate-100 flex items-center justify-between">
              <h2 className="font-semibold text-slate-800">Add New User</h2>
              <button onClick={() => setShowForm(false)} className="text-slate-400 hover:text-slate-600">✕</button>
            </div>
            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              {formError && (
                <div className="px-4 py-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">
                  ⚠️ {formError}
                </div>
              )}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1.5">
                    Username <span className="text-red-500">*</span>
                  </label>
                  <input type="text" name="username" value={form.username} onChange={handleChange} required
                    placeholder="e.g. john.doe"
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1.5">
                    Email <span className="text-red-500">*</span>
                  </label>
                  <input type="email" name="email" value={form.email} onChange={handleChange} required
                    placeholder="e.g. john@ku.ac.th"
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1.5">
                    Password <span className="text-red-500">*</span>
                  </label>
                  <input type="password" name="password" value={form.password} onChange={handleChange} required
                    placeholder="Min 8 characters"
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1.5">
                    Role <span className="text-red-500">*</span>
                  </label>
                  <select name="roleCode" value={form.roleCode} onChange={handleChange}
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white">
                    {ROLE_OPTIONS.map((r) => (
                      <option key={r.code} value={r.code}>{r.label}</option>
                    ))}
                  </select>
                </div>
              </div>
              <div className="flex justify-end gap-3 pt-2">
                <button type="button" onClick={() => setShowForm(false)}
                  className="px-5 py-2 border border-slate-200 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-50">
                  Cancel
                </button>
                <button type="submit" disabled={submitting}
                  className="px-5 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-60">
                  {submitting ? "กำลังเพิ่ม..." : "Add User"}
                </button>
              </div>
            </form>
          </div>
        )}

        <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
          <div className="px-6 py-4 border-b border-slate-100">
            <h2 className="font-semibold text-slate-800">Role Overview</h2>
          </div>
          <div className="p-6">
            <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 mb-6">
              {[
                { label: "Students", code: "03", color: "bg-blue-50 border-blue-200 text-blue-700" },
                { label: "Lecturers", code: "02", color: "bg-purple-50 border-purple-200 text-purple-700" },
                { label: "Dept Heads", code: "01", color: "bg-red-50 border-red-200 text-red-700" },
                { label: "Admins", code: "00", color: "bg-orange-50 border-orange-200 text-orange-700" },
              ].map((r) => (
                <div key={r.code} className={`rounded-lg border p-4 text-center ${r.color}`}>
                  <div className="text-sm font-semibold">{r.label}</div>
                  <div className="text-xs mt-0.5 opacity-70">Code: {r.code}</div>
                </div>
              ))}
            </div>
            <div className="text-sm text-slate-600 bg-slate-50 rounded-lg p-4 border border-slate-200 space-y-1">
              <p className="font-medium text-slate-700">หมายเหตุ</p>
              <p>ใช้ปุ่ม <strong>Add User</strong> เพื่อสร้างบัญชีผู้ใช้ใหม่ผ่าน API</p>
              <p>การแสดงรายชื่อผู้ใช้ทั้งหมดต้องเพิ่ม endpoint <code>GET /auth/users</code> ในฝั่ง backend</p>
            </div>
          </div>
        </div>
      </div>
    </MainLayout>
  );
}
