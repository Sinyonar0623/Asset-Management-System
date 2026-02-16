"use client";
import { useState } from "react";
import MainLayout from "../components/MainLayout";
import Header from "../components/Header";
import ProtectedRoute from "../components/ProtectedRoute";

const myRepairs = [
  { id: "REP-001", asset: "Oscilloscope Rigol DS1054Z", assetId: "AST-005", reportDate: "2024-01-12", issue: "หน้าจอแสดงผลมีปัญหา กระพริบ", status: "Under Repair", technician: "ช่างสมศักดิ์" },
  { id: "REP-002", asset: "Desktop PC Intel Core i7", assetId: "AST-006", reportDate: "2024-01-08", issue: "เปิดไม่ติด บูทไม่ขึ้น", status: "Pending", technician: "—" },
  { id: "REP-003", asset: "Soldering Iron Kit", assetId: "AST-010", reportDate: "2024-01-03", issue: "หัวแร้งไม่ร้อน สายไฟหลุด", status: "Completed", technician: "ช่างวิชัย" },
];

const assetOptions = [
  { id: "AST-005", name: "Oscilloscope Rigol DS1054Z" },
  { id: "AST-006", name: "Desktop PC Intel Core i7" },
  { id: "AST-010", name: "Soldering Iron Kit" },
  { id: "AST-002", name: "Dell Laptop Latitude 5420 #2" },
];

const statusColors: Record<string, string> = {
  Pending: "bg-yellow-100 text-yellow-700",
  "Under Repair": "bg-orange-100 text-orange-700",
  Completed: "bg-green-100 text-green-700",
  Rejected: "bg-red-100 text-red-700",
};

const urgencyColors: Record<string, string> = {
  Low: "bg-slate-100 text-slate-600",
  Medium: "bg-yellow-100 text-yellow-700",
  High: "bg-red-100 text-red-700",
};

export default function RepairPage() {
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({
    assetId: "",
    issue: "",
    urgency: "Medium",
    note: "",
  });
  const [submitted, setSubmitted] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitted(true);
    setShowForm(false);
    setForm({ assetId: "", issue: "", urgency: "Medium", note: "" });
  };

  return (
    <ProtectedRoute requiredPermission="canCreateRepair">
    <MainLayout>
      <Header
        title="Repair Request"
        subtitle="แจ้งซ่อมทรัพย์สิน"
        actions={
          <button
            onClick={() => { setShowForm(true); setSubmitted(false); }}
            className="flex items-center gap-2 px-4 py-2 bg-orange-500 text-white rounded-lg text-sm font-medium hover:bg-orange-600 transition-colors"
          >
            <span>🔧</span> New Repair Request
          </button>
        }
      />

      <div className="flex-1 p-6 space-y-6">
        {/* Success */}
        {submitted && (
          <div className="bg-orange-50 border border-orange-200 rounded-xl p-4 flex items-center gap-3">
            <div className="text-orange-500 text-xl">🔧</div>
            <div>
              <div className="font-medium text-orange-800">Repair Request Submitted</div>
              <div className="text-sm text-orange-600">คำขอแจ้งซ่อมถูกส่งเรียบร้อยแล้ว ทีมช่างจะดำเนินการโดยเร็ว</div>
            </div>
            <button onClick={() => setSubmitted(false)} className="ml-auto text-orange-400 hover:text-orange-600">✕</button>
          </div>
        )}

        {/* Form */}
        {showForm && (
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-6 py-4 border-b border-slate-100 flex items-center justify-between">
              <h2 className="font-semibold text-slate-800">New Repair Request</h2>
              <button onClick={() => setShowForm(false)} className="text-slate-400 hover:text-slate-600">✕</button>
            </div>
            <form onSubmit={handleSubmit} className="p-6 space-y-5">
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Select Asset <span className="text-red-500">*</span>
                </label>
                <select
                  name="assetId"
                  value={form.assetId}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-orange-400 bg-white"
                >
                  <option value="">-- Select asset to repair --</option>
                  {assetOptions.map((a) => (
                    <option key={a.id} value={a.id}>[{a.id}] {a.name}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Urgency Level
                </label>
                <div className="flex gap-3">
                  {["Low", "Medium", "High"].map((u) => (
                    <label key={u} className="flex-1 cursor-pointer">
                      <input
                        type="radio"
                        name="urgency"
                        value={u}
                        checked={form.urgency === u}
                        onChange={handleChange}
                        className="sr-only"
                      />
                      <div className={`text-center py-2.5 rounded-lg text-sm font-medium border-2 transition-colors ${
                        form.urgency === u
                          ? u === "Low" ? "border-slate-400 bg-slate-100 text-slate-700"
                            : u === "Medium" ? "border-yellow-400 bg-yellow-50 text-yellow-700"
                            : "border-red-400 bg-red-50 text-red-700"
                          : "border-slate-200 text-slate-500 hover:border-slate-300"
                      }`}>
                        {u === "Low" ? "🟢 Low" : u === "Medium" ? "🟡 Medium" : "🔴 High"}
                      </div>
                    </label>
                  ))}
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Issue Description <span className="text-red-500">*</span>
                </label>
                <textarea
                  name="issue"
                  value={form.issue}
                  onChange={handleChange}
                  required
                  rows={4}
                  placeholder="อธิบายปัญหาที่พบ เช่น อาการผิดปกติ สาเหตุที่คาดว่าเกิดขึ้น..."
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-orange-400 resize-none"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Additional Notes
                </label>
                <textarea
                  name="note"
                  value={form.note}
                  onChange={handleChange}
                  rows={2}
                  placeholder="ข้อมูลเพิ่มเติม (ถ้ามี)..."
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-orange-400 resize-none"
                />
              </div>

              <div className="flex justify-end gap-3 pt-2">
                <button type="button" onClick={() => setShowForm(false)} className="px-5 py-2 border border-slate-200 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-50">
                  Cancel
                </button>
                <button type="submit" className="px-5 py-2 bg-orange-500 text-white rounded-lg text-sm font-medium hover:bg-orange-600">
                  Submit Repair Request
                </button>
              </div>
            </form>
          </div>
        )}

        {/* Repair List */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
          <div className="px-5 py-4 border-b border-slate-100">
            <h2 className="font-semibold text-slate-800">Repair History</h2>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="bg-slate-50 border-b border-slate-100 text-left">
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Request ID</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Asset</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Issue</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Report Date</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Technician</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Status</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50">
                {myRepairs.map((r) => (
                  <tr key={r.id} className="hover:bg-slate-50 transition-colors">
                    <td className="px-5 py-3 font-mono text-xs text-slate-600">{r.id}</td>
                    <td className="px-5 py-3">
                      <div className="font-medium text-slate-800">{r.asset}</div>
                      <div className="text-xs text-slate-400">{r.assetId}</div>
                    </td>
                    <td className="px-5 py-3 text-slate-600 max-w-48 truncate">{r.issue}</td>
                    <td className="px-5 py-3 text-slate-500">{r.reportDate}</td>
                    <td className="px-5 py-3 text-slate-600">{r.technician}</td>
                    <td className="px-5 py-3">
                      <span className={`px-2.5 py-1 rounded-full text-xs font-medium ${statusColors[r.status]}`}>
                        {r.status}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </MainLayout>
    </ProtectedRoute>
  );
}
