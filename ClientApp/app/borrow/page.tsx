"use client";
import { useState, useEffect, useCallback } from "react";
import MainLayout from "../components/MainLayout";
import Header from "../components/Header";
import { borrowApi, assetApi, BorrowRequest, AssetSummary } from "../lib/api";

const STATUS_COLORS: Record<string, string> = {
  Pending: "bg-yellow-100 text-yellow-700",
  Approved: "bg-blue-100 text-blue-700",
  Returned: "bg-green-100 text-green-700",
  Rejected: "bg-red-100 text-red-700",
};

export default function BorrowPage() {
  const [requests, setRequests] = useState<BorrowRequest[]>([]);
  const [assets, setAssets] = useState<AssetSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState("");
  const [successMsg, setSuccessMsg] = useState("");
  const [form, setForm] = useState({ assetId: "", borrowDate: "", returnDate: "", purpose: "" });

  const loadRequests = useCallback(() => {
    setLoading(true);
    borrowApi.getAll({ myOnly: true })
      .then((r) => setRequests(r.items))
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    loadRequests();
    assetApi.getAll({ status: "Active", pageSize: 100 })
      .then((r) => setAssets(r.items))
      .catch(console.error);
  }, [loadRequests]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setFormError("");
    const sel = assets.find((a) => String(a.id) === form.assetId);
    try {
      await borrowApi.create({
        assetId: Number(form.assetId),
        assetName: sel?.name || "",
        assetRealWorldId: sel?.realWorldId || "",
        borrowDate: form.borrowDate,
        returnDate: form.returnDate,
        purpose: form.purpose,
      });
      setSuccessMsg("ส่งคำขอยืมเรียบร้อยแล้ว รอการอนุมัติ");
      setShowForm(false);
      setForm({ assetId: "", borrowDate: "", returnDate: "", purpose: "" });
      loadRequests();
    } catch (err) {
      setFormError((err as Error).message || "เกิดข้อผิดพลาด");
    } finally {
      setSubmitting(false);
    }
  };

  const handleReturn = async (id: number) => {
    try {
      await borrowApi.markReturned(id);
      setSuccessMsg("บันทึกการคืนทรัพย์สินเรียบร้อย");
      loadRequests();
    } catch (err) {
      alert((err as Error).message);
    }
  };

  return (
    <MainLayout>
      <Header
        title="Borrow Request"
        subtitle="จัดการคำขอยืมทรัพย์สิน"
        actions={
          <button
            onClick={() => { setShowForm(true); setSuccessMsg(""); }}
            className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700"
          >
            <span>+</span> New Borrow Request
          </button>
        }
      />
      <div className="flex-1 p-6 space-y-6">
        {successMsg && (
          <div className="bg-green-50 border border-green-200 rounded-xl p-4 flex items-center gap-3">
            <span className="text-green-500 text-xl">✅</span>
            <span className="font-medium text-green-800 text-sm">{successMsg}</span>
            <button onClick={() => setSuccessMsg("")} className="ml-auto text-green-400 hover:text-green-600">✕</button>
          </div>
        )}

        {showForm && (
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-6 py-4 border-b border-slate-100 flex items-center justify-between">
              <h2 className="font-semibold text-slate-800">New Borrow Request</h2>
              <button onClick={() => setShowForm(false)} className="text-slate-400 hover:text-slate-600">✕</button>
            </div>
            <form onSubmit={handleSubmit} className="p-6 space-y-5">
              {formError && (
                <div className="px-4 py-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">
                  ⚠️ {formError}
                </div>
              )}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-slate-700 mb-1.5">
                    Select Asset <span className="text-red-500">*</span>
                  </label>
                  <select name="assetId" value={form.assetId} onChange={handleChange} required
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white">
                    <option value="">-- Select available asset --</option>
                    {assets.map((a) => (
                      <option key={a.id} value={String(a.id)}>[{a.realWorldId}] {a.name}</option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1.5">
                    Borrow Date <span className="text-red-500">*</span>
                  </label>
                  <input type="date" name="borrowDate" value={form.borrowDate} onChange={handleChange} required
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1.5">
                    Return Date <span className="text-red-500">*</span>
                  </label>
                  <input type="date" name="returnDate" value={form.returnDate} onChange={handleChange} required
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                </div>
                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-slate-700 mb-1.5">
                    Purpose <span className="text-red-500">*</span>
                  </label>
                  <textarea name="purpose" value={form.purpose} onChange={handleChange} required rows={3}
                    placeholder="ระบุวัตถุประสงค์การยืมทรัพย์สิน..."
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none" />
                </div>
              </div>
              <div className="flex justify-end gap-3 pt-2">
                <button type="button" onClick={() => setShowForm(false)}
                  className="px-5 py-2 border border-slate-200 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-50">
                  Cancel
                </button>
                <button type="submit" disabled={submitting}
                  className="px-5 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-60">
                  {submitting ? "กำลังส่ง..." : "Submit Request"}
                </button>
              </div>
            </form>
          </div>
        )}

        <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
          <div className="px-5 py-4 border-b border-slate-100">
            <h2 className="font-semibold text-slate-800">My Borrow Requests</h2>
          </div>
          {loading ? (
            <div className="flex items-center justify-center py-12">
              <div className="w-6 h-6 border-4 border-blue-600 border-t-transparent rounded-full animate-spin" />
            </div>
          ) : requests.length === 0 ? (
            <div className="text-center py-12 text-slate-400">
              <div className="text-3xl mb-2">📋</div>
              <div>ยังไม่มีคำขอยืม</div>
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="bg-slate-50 border-b border-slate-100 text-left">
                    {["ID","Asset","Borrow Date","Return Date","Purpose","Status","Actions"].map((h) => (
                      <th key={h} className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase">{h}</th>
                    ))}
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-50">
                  {requests.map((r) => (
                    <tr key={r.id} className="hover:bg-slate-50">
                      <td className="px-5 py-3 font-mono text-xs text-slate-600">#{r.id}</td>
                      <td className="px-5 py-3">
                        <div className="font-medium text-slate-800">{r.assetName}</div>
                        <div className="text-xs text-slate-400">{r.assetRealWorldId}</div>
                      </td>
                      <td className="px-5 py-3 text-slate-600">{r.borrowDate?.slice(0,10) ?? "—"}</td>
                      <td className="px-5 py-3 text-slate-600">{r.returnDate?.slice(0,10) ?? "—"}</td>
                      <td className="px-5 py-3 text-slate-600 max-w-40 truncate">{r.purpose}</td>
                      <td className="px-5 py-3">
                        <span className={`px-2.5 py-1 rounded-full text-xs font-medium ${STATUS_COLORS[r.status] ?? "bg-slate-100 text-slate-600"}`}>
                          {r.status}
                        </span>
                      </td>
                      <td className="px-5 py-3">
                        {r.status === "Approved"
                          ? <button onClick={() => handleReturn(r.id)} className="text-xs text-green-600 hover:underline">Confirm Return</button>
                          : <span className="text-xs text-slate-400">—</span>}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </MainLayout>
  );
}
