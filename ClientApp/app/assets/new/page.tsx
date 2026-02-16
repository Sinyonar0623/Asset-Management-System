"use client";
import { useState } from "react";
import MainLayout from "../../components/MainLayout";
import Header from "../../components/Header";
import ProtectedRoute from "../../components/ProtectedRoute";
import Link from "next/link";
import { useAuth } from "../../contexts/AuthContext";
import { MOCK_USERS, getLecturers } from "../../lib/auth";

const categories = [
  "Computers & Laptops",
  "Electronic Equipment",
  "Lab Equipment",
  "Audio/Visual",
  "Furniture & Others",
];

const locations = [
  "ห้อง Lab 1",
  "ห้อง Lab 2",
  "ห้อง Lab 3",
  "ห้องเรียน 201",
  "ห้องเรียน 301",
  "ห้อง Server",
  "สำนักงาน",
  "คลังพัสดุ",
];

export default function AddNewAssetPage() {
  const { session } = useAuth();
  const lecturers = getLecturers();

  const [form, setForm] = useState({
    name: "",
    assetCode: "",
    serial: "",
    category: "",
    location: "",
    purchaseDate: "",
    purchasePrice: "",
    condition: "Good",
    status: "Available",
    description: "",
    brand: "",
    model: "",
    supplier: "",
    department: "วิศวกรรมคอมพิวเตอร์",
    ownerLecturerId: session?.role === "lecturer" ? session.userId : "",
    warrantyExpiry: "",
  });

  const [submitted, setSubmitted] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitted(true);
  };

  if (submitted) {
    return (
      <ProtectedRoute requiredPermission="canAddAsset">
      <MainLayout>
        <Header title="Add New Asset" />
        <div className="flex-1 p-6 flex items-center justify-center">
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm p-10 text-center max-w-md w-full">
            <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center text-3xl mx-auto mb-4">
              ✅
            </div>
            <h2 className="text-xl font-semibold text-slate-800 mb-2">Asset Added Successfully!</h2>
            <p className="text-slate-500 mb-6 text-sm">ทรัพย์สินถูกเพิ่มเข้าระบบเรียบร้อยแล้ว</p>
            <div className="bg-slate-50 rounded-lg p-4 text-left text-sm mb-6">
              <div className="flex justify-between py-1 border-b border-slate-100">
                <span className="text-slate-500">Asset Name</span>
                <span className="font-medium text-slate-800">{form.name}</span>
              </div>
              <div className="flex justify-between py-1 border-b border-slate-100">
                <span className="text-slate-500">Asset Code</span>
                <span className="font-medium text-slate-800 font-mono">{form.assetCode || "Auto-generated"}</span>
              </div>
              <div className="flex justify-between py-1">
                <span className="text-slate-500">Category</span>
                <span className="font-medium text-slate-800">{form.category}</span>
              </div>
            </div>
            <div className="flex gap-3">
              <Link href="/assets" className="flex-1 px-4 py-2 border border-slate-200 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-50 text-center">
                View All Assets
              </Link>
              <button
                onClick={() => setSubmitted(false)}
                className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700"
              >
                Add Another
              </button>
            </div>
          </div>
        </div>
      </MainLayout>
      </ProtectedRoute>
    );
  }

  return (
    <ProtectedRoute requiredPermission="canAddAsset">
    <MainLayout>
      <Header
        title="Add New Asset"
        subtitle="เพิ่มทรัพย์สินใหม่เข้าสู่ระบบ"
        actions={
          <Link href="/assets" className="px-4 py-2 border border-slate-200 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-50">
            Cancel
          </Link>
        }
      />

      <div className="flex-1 p-6">
        <form onSubmit={handleSubmit} className="max-w-4xl mx-auto space-y-6">

          {/* Basic Information */}
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-6 py-4 border-b border-slate-100">
              <h2 className="font-semibold text-slate-800 flex items-center gap-2">
                <span className="w-6 h-6 bg-blue-100 text-blue-600 rounded text-xs font-bold flex items-center justify-center">1</span>
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
                  Asset Code
                </label>
                <input
                  type="text"
                  name="assetCode"
                  value={form.assetCode}
                  onChange={handleChange}
                  placeholder="e.g. AST-001 (auto if empty)"
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Serial Number
                </label>
                <input
                  type="text"
                  name="serial"
                  value={form.serial}
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
                  Model
                </label>
                <input
                  type="text"
                  name="model"
                  value={form.model}
                  onChange={handleChange}
                  placeholder="e.g. Latitude 5420"
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

          {/* Classification & Location */}
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-6 py-4 border-b border-slate-100">
              <h2 className="font-semibold text-slate-800 flex items-center gap-2">
                <span className="w-6 h-6 bg-blue-100 text-blue-600 rounded text-xs font-bold flex items-center justify-center">2</span>
                Classification &amp; Location
              </h2>
            </div>
            <div className="p-6 grid grid-cols-1 md:grid-cols-2 gap-5">
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Category <span className="text-red-500">*</span>
                </label>
                <select
                  name="category"
                  value={form.category}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                >
                  <option value="">-- Select Category --</option>
                  {categories.map((c) => (
                    <option key={c} value={c}>{c}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Storage Location <span className="text-red-500">*</span>
                </label>
                <select
                  name="location"
                  value={form.location}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                >
                  <option value="">-- Select Location --</option>
                  {locations.map((l) => (
                    <option key={l} value={l}>{l}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Current Status
                </label>
                <select
                  name="status"
                  value={form.status}
                  onChange={handleChange}
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                >
                  <option value="Available">Available</option>
                  <option value="In Use">In Use</option>
                  <option value="Under Repair">Under Repair</option>
                  <option value="Inactive">Inactive</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Condition
                </label>
                <div className="flex gap-3">
                  {["Good", "Fair", "Poor"].map((c) => (
                    <label key={c} className="flex-1 cursor-pointer">
                      <input
                        type="radio"
                        name="condition"
                        value={c}
                        checked={form.condition === c}
                        onChange={handleChange}
                        className="sr-only"
                      />
                      <div className={`text-center py-2.5 rounded-lg text-sm font-medium border-2 transition-colors ${
                        form.condition === c
                          ? c === "Good" ? "border-green-500 bg-green-50 text-green-700"
                            : c === "Fair" ? "border-yellow-500 bg-yellow-50 text-yellow-700"
                            : "border-red-500 bg-red-50 text-red-700"
                          : "border-slate-200 text-slate-600 hover:border-slate-300"
                      }`}>
                        {c}
                      </div>
                    </label>
                  ))}
                </div>
              </div>
            </div>
          </div>

          {/* Purchase Information */}
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-6 py-4 border-b border-slate-100">
              <h2 className="font-semibold text-slate-800 flex items-center gap-2">
                <span className="w-6 h-6 bg-blue-100 text-blue-600 rounded text-xs font-bold flex items-center justify-center">3</span>
                Purchase Information
              </h2>
            </div>
            <div className="p-6 grid grid-cols-1 md:grid-cols-2 gap-5">
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Purchase Date
                </label>
                <input
                  type="date"
                  name="purchaseDate"
                  value={form.purchaseDate}
                  onChange={handleChange}
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Purchase Price (THB)
                </label>
                <div className="relative">
                  <span className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-sm">฿</span>
                  <input
                    type="number"
                    name="purchasePrice"
                    value={form.purchasePrice}
                    onChange={handleChange}
                    placeholder="0.00"
                    className="w-full pl-7 pr-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Supplier
                </label>
                <input
                  type="text"
                  name="supplier"
                  value={form.supplier}
                  onChange={handleChange}
                  placeholder="e.g. บริษัท XYZ จำกัด"
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">
                  Warranty Expiry
                </label>
                <input
                  type="date"
                  name="warrantyExpiry"
                  value={form.warrantyExpiry}
                  onChange={handleChange}
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
            </div>
          </div>

          {/* Department & Ownership */}
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-6 py-4 border-b border-slate-100">
              <h2 className="font-semibold text-slate-800 flex items-center gap-2">
                <span className="w-6 h-6 bg-blue-100 text-blue-600 rounded text-xs font-bold flex items-center justify-center">4</span>
                Department &amp; Ownership
              </h2>
            </div>
            <div className="p-6 grid grid-cols-1 md:grid-cols-2 gap-5">
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">Department</label>
                <input
                  type="text"
                  value="วิศวกรรมคอมพิวเตอร์"
                  readOnly
                  className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm bg-slate-50 cursor-not-allowed text-slate-600"
                />
                <p className="text-xs text-slate-400 mt-1">กำหนดโดยอัตโนมัติ</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1.5">Asset Owner</label>
                {session?.role === "lecturer" ? (
                  // Lecturer: auto-filled with their name, read-only
                  <input
                    type="text"
                    value={session.name}
                    readOnly
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm bg-slate-50 cursor-not-allowed text-slate-600"
                  />
                ) : (
                  // DeptHead / Admin: can assign to any lecturer or leave unassigned (depthead-owned)
                  <select
                    name="ownerLecturerId"
                    value={form.ownerLecturerId}
                    onChange={handleChange}
                    className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                  >
                    <option value="">ภาควิชา (ไม่มีเจ้าของเฉพาะ)</option>
                    {lecturers.map((u) => (
                      <option key={u.id} value={u.id}>{u.name}</option>
                    ))}
                  </select>
                )}
                <p className="text-xs text-slate-400 mt-1">
                  {session?.role === "lecturer"
                    ? "Asset นี้จะอยู่ในความรับผิดชอบของคุณ"
                    : "ถ้าไม่เลือก asset จะอยู่ในความดูแลของหัวหน้าภาควิชา"}
                </p>
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
              className="px-6 py-2.5 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 transition-colors flex items-center gap-2"
            >
              <span>+</span>
              Add Asset
            </button>
          </div>
        </form>
      </div>
    </MainLayout>
    </ProtectedRoute>
  );
}
