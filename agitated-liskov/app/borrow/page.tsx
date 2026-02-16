"use client";
import { useState } from "react";
import MainLayout from "../components/MainLayout";
import Header from "../components/Header";
import Link from "next/link";

// assetOwnerId: null = dept head approves, "user-002" = lecturer1 approves
const myRequests = [
  { id: "BRW-001", asset: "Dell Laptop #12", assetId: "AST-001", assetOwnerId: "user-002", requestDate: "2024-01-15", borrowDate: "2024-01-16", returnDate: "2024-01-20", status: "Pending", purpose: "ใช้ทำโปรเจกต์วิชา CPE441" },
  { id: "BRW-002", asset: "Arduino Kit #5", assetId: "AST-004", assetOwnerId: null, requestDate: "2024-01-10", borrowDate: "2024-01-11", returnDate: "2024-01-18", status: "Approved", purpose: "Lab Assignment" },
  { id: "BRW-003", asset: "Oscilloscope #1", assetId: "AST-005", assetOwnerId: null, requestDate: "2024-01-05", borrowDate: "2024-01-06", returnDate: "2024-01-07", status: "Completed", purpose: "Experiment" },
];

const availableAssets = [
  { id: "AST-001", name: "Dell Laptop Latitude 5420", category: "Computers & Laptops" },
  { id: "AST-003", name: "Epson Projector EB-X51", category: "Audio/Visual" },
  { id: "AST-004", name: "Arduino Uno Kit", category: "Lab Equipment" },
  { id: "AST-006", name: "Desktop PC Intel Core i7", category: "Computers & Laptops" },
  { id: "AST-008", name: "Network Switch 24-Port", category: "Electronic Equipment" },
];

const statusColors: Record<string, string> = {
  Pending: "bg-yellow-100 text-yellow-700",
  Approved: "bg-blue-100 text-blue-700",
  "In Progress": "bg-purple-100 text-purple-700",
  Completed: "bg-green-100 text-green-700",
  Rejected: "bg-red-100 text-red-700",
};

export default function BorrowPage() {
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({
    assetId: "",
    borrowDate: "",
    returnDate: "",
    purpose: "",
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
  };

  return (
    <MainLayout>
      <Header
        title="Borrow Request"
        subtitle="จัดการคำขอยืมทรัพย์สิน"
        actions={
          <button
            onClick={() => { setShowForm(true); setSubmitted(false); }}
            className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 transition-colors"
          >
            <span>+</span> New Borrow Request
          </button>
        }
      />

      <div className="flex-1 p-6 space-y-6">
        {/* Success Banner */}
        {submitted && (
          <div className="bg-green-50 border border-green-200 rounded-xl p-4 flex items-center gap-3">
            <div className="text-green-500 text-xl">✅</div>
            <div>
              <div className="font-medium text-green-800">Request Submitted Successfully</div>
              <div className="text-sm text-green-600">คำขอยืมของคุณถูกส่งเพื่อรอการอนุมัติแล้ว</div>
            </div>
            <button onClick={() => setSubmitted(false)} className="ml-auto text-green-400 hover:text-green-600">✕</button>
          </div>
        )}

        {/* New Request Form */}
        {showForm && (
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-6 py-4 border-b border-slate-100 flex items-center justify-between">
              <h2 className="font-semibold text-slate-800">New Borrow Request</h2>
              <button onClick={() => setShowForm(false)} className="text-slate-400 hover:text-slate-600">✕</button>
            </div>
            <form onSubmit={handleSubmit} className="p-6 space-y-5">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-slate-700 mb-1.5">
                    Select Asset <span className="text-red-500">*</span>
                  </label>
                  <select
                    name="assetId"
                    value={form.assetId}
                    onChange={handleChange}
                    required
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                  >
                    <option value="">-- Select available asset --</option>
                    {availableAssets.map((a) => (
                      <option key={a.id} value={a.id}>
                        [{a.id}] {a.name} ({a.category})
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1.5">
                    Borrow Date <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="date"
                    name="borrowDate"
                    value={form.borrowDate}
                    onChange={handleChange}
                    required
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1.5">
                    Return Date <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="date"
                    name="returnDate"
                    value={form.returnDate}
                    onChange={handleChange}
                    required
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-slate-700 mb-1.5">
                    Purpose / วัตถุประสงค์ <span className="text-red-500">*</span>
                  </label>
                  <textarea
                    name="purpose"
                    value={form.purpose}
                    onChange={handleChange}
                    required
                    rows={3}
                    placeholder="ระบุวัตถุประสงค์การยืมทรัพย์สิน..."
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
                  />
                </div>

                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-slate-700 mb-1.5">
                    Additional Notes
                  </label>
                  <textarea
                    name="note"
                    value={form.note}
                    onChange={handleChange}
                    rows={2}
                    placeholder="หมายเหตุเพิ่มเติม (ถ้ามี)..."
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
                  />
                </div>
              </div>

              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={() => setShowForm(false)}
                  className="px-5 py-2 border border-slate-200 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-50"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-5 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700"
                >
                  Submit Request
                </button>
              </div>
            </form>
          </div>
        )}

        {/* My Requests */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
          <div className="px-5 py-4 border-b border-slate-100">
            <h2 className="font-semibold text-slate-800">My Borrow Requests</h2>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="bg-slate-50 border-b border-slate-100 text-left">
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Request ID</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Asset</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Borrow Date</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Return Date</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Purpose</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Approver</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Status</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50">
                {myRequests.map((r) => (
                  <tr key={r.id} className="hover:bg-slate-50 transition-colors">
                    <td className="px-5 py-3 font-mono text-xs text-slate-600">{r.id}</td>
                    <td className="px-5 py-3">
                      <div className="font-medium text-slate-800">{r.asset}</div>
                      <div className="text-xs text-slate-400">{r.assetId}</div>
                    </td>
                    <td className="px-5 py-3 text-slate-600">{r.borrowDate}</td>
                    <td className="px-5 py-3 text-slate-600">{r.returnDate}</td>
                    <td className="px-5 py-3 text-slate-600 max-w-40 truncate">{r.purpose}</td>
                    <td className="px-5 py-3 text-slate-600 text-xs whitespace-nowrap">
                      {r.assetOwnerId ? "ผศ.ดร.วิชัย สุขใจ" : "รศ.ดร.สมศักดิ์ ดีงาม"}
                    </td>
                    <td className="px-5 py-3">
                      <span className={`px-2.5 py-1 rounded-full text-xs font-medium ${statusColors[r.status]}`}>
                        {r.status}
                      </span>
                    </td>
                    <td className="px-5 py-3">
                      {r.status === "Pending" && (
                        <button className="text-xs text-red-500 hover:underline">Cancel</button>
                      )}
                      {r.status === "Approved" && (
                        <button className="text-xs text-green-600 hover:underline">Confirm Return</button>
                      )}
                      {r.status === "Completed" && (
                        <span className="text-xs text-slate-400">—</span>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </MainLayout>
  );
}
