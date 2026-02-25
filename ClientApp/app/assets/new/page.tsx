"use client";
import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import MainLayout from "../../components/MainLayout";
import Header from "../../components/Header";
import Link from "next/link";
import { useAuth } from "../../contexts/AuthContext";
import { assetApi } from "../../lib/api";

const ASSET_TYPES = [
  "Computers & Laptops",
  "Electronic Equipment",
  "Lab Equipment",
  "Audio/Visual",
  "Furniture & Others",
];

const STATUS_OPTIONS = ["Active", "Inactive", "Borrowed", "Under Repair"];

export default function AddNewAssetPage() {
  const { session } = useAuth();
  const router = useRouter();

  const [form, setForm] = useState({
    realWorldId: "",
    name: "",
    brand: "",
    serialNo: "",
    type: "",
    description: "",
    amount: "1",
    status: "Active",
    remark: "",
    ownerId: "",
  });

  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (session) {
      setForm((f) => ({ ...f, ownerId: session.userId }));
    }
  }, [session]);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>
  ) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setError("");
    try {
      await assetApi.create({
        realWorldId: form.realWorldId,
        brand: form.brand,
        name: form.name,
        serialNo: form.serialNo,
        description: form.description,
        type: form.type,
        amount: Number(form.amount) || 1,
        remark: form.remark,
        ownerId: form.ownerId || session?.userId || "",
      });
      router.push("/assets");
    } catch (err) {
      setError((err as Error).message || "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง");
      setSubmitting(false);
    }
  };

  return (
    <MainLayout>
      <Header
        title="Add New Asset"
        subtitle="เพิ่มทรัพย์สินใหม่เข้าสู่ระบบ"
        actions={
          <Link
            href="/assets"
            className="px-4 py-2 border border-slate-200 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-50"
          >
            Cancel
          </Link>
        }
      />

      <div className="flex-1 p-6">
        <form onSubmit={handleSubmit} className="max-w-3xl mx-auto space-y-6">
          {error && (
            <div className="px-4 py-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700 flex items-center gap-2">
              <span>⚠️</span> {error}
            </div>
          )}

          {/* Basic Information */}
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-6 py-4 border-b border-slate-100">
              <h2 className="font-semibold text-slate-800 flex items-center gap-2">
                <span className="w-6 h-6 bg-blue-100 text-blue-600 rounded text-xs font-bold flex items-center justify-center">
                  1
                </span>
                Basic Information
              </h2>
            </div>
            <div className="p-6 grid grid-cols-1 md:grid-cols-2 gap-5">
              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Asset Name <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  name="name"
                  value={form.name}
                  onChange={handleChange}
                  required
                  placeholder="e.g. Dell Laptop Latitude 5420"
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Real World ID (Asset Code)
                </label>
                <input
                  type="text"
                  name="realWorldId"
                  value={form.realWorldId}
                  onChange={handleChange}
                  placeholder="e.g. CE-2024-001"
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Serial Number
                </label>
                <input
                  type="text"
                  name="serialNo"
                  value={form.serialNo}
                  onChange={handleChange}
                  placeholder="e.g. DL5420-2024-001"
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Brand
                </label>
                <input
                  type="text"
                  name="brand"
                  value={form.brand}
                  onChange={handleChange}
                  placeholder="e.g. Dell, Apple, Cisco"
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Amount
                </label>
                <input
                  type="number"
                  name="amount"
                  value={form.amount}
                  onChange={handleChange}
                  min={1}
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Description
                </label>
                <textarea
                  name="description"
                  value={form.description}
                  onChange={handleChange}
                  rows={3}
                  placeholder="รายละเอียดเพิ่มเติมของทรัพย์สิน..."
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
                />
              </div>
            </div>
          </div>

          {/* Classification & Status */}
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-6 py-4 border-b border-slate-100">
              <h2 className="font-semibold text-slate-800 flex items-center gap-2">
                <span className="w-6 h-6 bg-blue-100 text-blue-600 rounded text-xs font-bold flex items-center justify-center">
                  2
                </span>
                Classification &amp; Status
              </h2>
            </div>
            <div className="p-6 grid grid-cols-1 md:grid-cols-2 gap-5">
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Type / Category <span className="text-red-500">*</span>
                </label>
                <select
                  name="type"
                  value={form.type}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                >
                  <option value="">-- Select Type --</option>
                  {ASSET_TYPES.map((t) => (
                    <option key={t} value={t}>
                      {t}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Status
                </label>
                <select
                  name="status"
                  value={form.status}
                  onChange={handleChange}
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                >
                  {STATUS_OPTIONS.map((s) => (
                    <option key={s} value={s}>
                      {s}
                    </option>
                  ))}
                </select>
              </div>

              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Remark
                </label>
                <input
                  type="text"
                  name="remark"
                  value={form.remark}
                  onChange={handleChange}
                  placeholder="หมายเหตุ (ถ้ามี)"
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
            </div>
          </div>

          {/* Submit */}
          <div className="flex items-center justify-end gap-3 pb-6">
            <Link
              href="/assets"
              className="px-6 py-2.5 border border-slate-200 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-50 transition-colors"
            >
              Cancel
            </Link>
            <button
              type="submit"
              disabled={submitting}
              className="px-6 py-2.5 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 transition-colors disabled:opacity-60 flex items-center gap-2"
            >
              {submitting ? "กำลังบันทึก..." : "Add Asset"}
            </button>
          </div>
        </form>
      </div>
    </MainLayout>
  );
}
