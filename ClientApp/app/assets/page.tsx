"use client";
import { useState } from "react";
import MainLayout from "../components/MainLayout";
import Header from "../components/Header";
import Link from "next/link";
import { useAuth } from "../contexts/AuthContext";
import { PERMISSIONS, MOCK_USERS } from "../lib/auth";

const assets = [
  { id: "AST-001", name: "Dell Laptop Latitude 5420", category: "Computers & Laptops", location: "ห้อง Lab 1", status: "Available", condition: "Good", serial: "DL5420-2023-001", department: "วิศวกรรมคอมพิวเตอร์", ownerLecturerId: "user-002" },
  { id: "AST-002", name: "Dell Laptop Latitude 5420", category: "Computers & Laptops", location: "ห้อง Lab 1", status: "In Use", condition: "Good", serial: "DL5420-2023-002", department: "วิศวกรรมคอมพิวเตอร์", ownerLecturerId: null },
  { id: "AST-003", name: "Epson Projector EB-X51", category: "Audio/Visual", location: "ห้องเรียน 201", status: "Available", condition: "Good", serial: "EPEB-X51-001", department: "วิศวกรรมคอมพิวเตอร์", ownerLecturerId: "user-002" },
  { id: "AST-004", name: "Arduino Uno Kit", category: "Lab Equipment", location: "ห้อง Lab 2", status: "Available", condition: "Good", serial: "ARD-UNO-KIT-005", department: "วิศวกรรมคอมพิวเตอร์", ownerLecturerId: null },
  { id: "AST-005", name: "Oscilloscope Rigol DS1054Z", category: "Electronic Equipment", location: "ห้อง Lab 2", status: "Under Repair", condition: "Fair", serial: "OSC-DS1054-002", department: "วิศวกรรมคอมพิวเตอร์", ownerLecturerId: null },
  { id: "AST-006", name: "Desktop PC Intel Core i7", category: "Computers & Laptops", location: "สำนักงาน", status: "Available", condition: "Good", serial: "PC-I7-2022-008", department: "วิศวกรรมคอมพิวเตอร์", ownerLecturerId: null },
  { id: "AST-007", name: "Raspberry Pi 4 Kit", category: "Lab Equipment", location: "ห้อง Lab 2", status: "In Use", condition: "Good", serial: "RPI4-KIT-003", department: "วิศวกรรมคอมพิวเตอร์", ownerLecturerId: null },
  { id: "AST-008", name: "Network Switch 24-Port", category: "Electronic Equipment", location: "ห้อง Server", status: "Available", condition: "Good", serial: "NSW-24P-001", department: "วิศวกรรมคอมพิวเตอร์", ownerLecturerId: null },
  { id: "AST-009", name: "Whiteboard 120x240cm", category: "Furniture & Others", location: "ห้องเรียน 301", status: "Available", condition: "Good", serial: "WB-120-003", department: "วิศวกรรมคอมพิวเตอร์", ownerLecturerId: null },
  { id: "AST-010", name: "Soldering Iron Kit", category: "Lab Equipment", location: "ห้อง Lab 3", status: "Under Repair", condition: "Poor", serial: "SLD-KIT-007", department: "วิศวกรรมคอมพิวเตอร์", ownerLecturerId: null },
];

const statusColors: Record<string, string> = {
  Available: "bg-green-100 text-green-700",
  "In Use": "bg-blue-100 text-blue-700",
  "Under Repair": "bg-orange-100 text-orange-700",
  Inactive: "bg-slate-100 text-slate-600",
};

const conditionColors: Record<string, string> = {
  Good: "text-green-600",
  Fair: "text-yellow-600",
  Poor: "text-red-600",
};

function getOwnerName(ownerLecturerId: string | null): string {
  if (!ownerLecturerId) return "หัวหน้าภาควิชา";
  const user = MOCK_USERS.find((u) => u.id === ownerLecturerId);
  return user?.name ?? ownerLecturerId;
}

export default function AssetsPage() {
  const { session } = useAuth();
  const role = session?.role;

  const [search, setSearch] = useState("");
  const [filterStatus, setFilterStatus] = useState("All");
  const [filterCategory, setFilterCategory] = useState("All");

  const filtered = assets.filter((a) => {
    const matchSearch = a.name.toLowerCase().includes(search.toLowerCase()) ||
      a.id.toLowerCase().includes(search.toLowerCase()) ||
      a.serial.toLowerCase().includes(search.toLowerCase());
    const matchStatus = filterStatus === "All" || a.status === filterStatus;
    const matchCategory = filterCategory === "All" || a.category === filterCategory;
    return matchSearch && matchStatus && matchCategory;
  });

  const categories = ["All", ...Array.from(new Set(assets.map((a) => a.category)))];
  const canAdd = role && PERMISSIONS.canAddAsset(role);

  return (
    <MainLayout>
      <Header
        title="Asset Management"
        subtitle={`ทั้งหมด ${assets.length} รายการ · ภาควิชาวิศวกรรมคอมพิวเตอร์`}
        actions={
          canAdd ? (
            <Link
              href="/assets/new"
              className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 transition-colors"
            >
              <span className="text-base leading-none">+</span>
              Add New Asset
            </Link>
          ) : undefined
        }
      />

      <div className="flex-1 p-6 space-y-4">
        {/* Summary Cards */}
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
          {[
            { label: "Total", count: assets.length },
            { label: "Available", count: assets.filter(a => a.status === "Available").length },
            { label: "In Use", count: assets.filter(a => a.status === "In Use").length },
            { label: "Under Repair", count: assets.filter(a => a.status === "Under Repair").length },
          ].map((s) => (
            <div key={s.label} className="bg-white rounded-lg border border-slate-200 p-4 text-center shadow-sm">
              <div className="text-2xl font-bold text-slate-800">{s.count}</div>
              <div className="text-sm text-slate-500">{s.label}</div>
            </div>
          ))}
        </div>

        {/* Filters */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm p-4">
          <div className="flex flex-wrap gap-3">
            <div className="flex-1 min-w-48">
              <div className="relative">
                <svg className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
                <input
                  type="text"
                  placeholder="Search by name, ID, serial..."
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                  className="w-full pl-9 pr-4 py-2 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
            </div>
            <select
              value={filterStatus}
              onChange={(e) => setFilterStatus(e.target.value)}
              className="px-3 py-2 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
            >
              <option value="All">All Status</option>
              <option>Available</option>
              <option>In Use</option>
              <option>Under Repair</option>
              <option>Inactive</option>
            </select>
            <select
              value={filterCategory}
              onChange={(e) => setFilterCategory(e.target.value)}
              className="px-3 py-2 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
            >
              {categories.map((c) => (
                <option key={c}>{c}</option>
              ))}
            </select>
          </div>
        </div>

        {/* Table */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="bg-slate-50 border-b border-slate-200 text-left">
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Asset ID</th>
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Asset Name</th>
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Category</th>
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Location</th>
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Status</th>
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Condition</th>
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Department</th>
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Owner</th>
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {filtered.length === 0 ? (
                  <tr>
                    <td colSpan={9} className="px-4 py-10 text-center text-slate-400">
                      No assets found
                    </td>
                  </tr>
                ) : (
                  filtered.map((asset) => (
                    <tr key={asset.id} className="hover:bg-slate-50 transition-colors">
                      <td className="px-4 py-3 font-mono text-xs text-slate-600">{asset.id}</td>
                      <td className="px-4 py-3">
                        <div className="font-medium text-slate-800">{asset.name}</div>
                        <div className="text-xs text-slate-400">{asset.serial}</div>
                      </td>
                      <td className="px-4 py-3 text-slate-600">{asset.category}</td>
                      <td className="px-4 py-3 text-slate-600">{asset.location}</td>
                      <td className="px-4 py-3">
                        <span className={`px-2.5 py-1 rounded-full text-xs font-medium ${statusColors[asset.status]}`}>
                          {asset.status}
                        </span>
                      </td>
                      <td className="px-4 py-3">
                        <span className={`text-xs font-medium ${conditionColors[asset.condition]}`}>
                          {asset.condition}
                        </span>
                      </td>
                      <td className="px-4 py-3">
                        <span className="text-xs px-2 py-0.5 bg-blue-50 text-blue-700 rounded font-medium">
                          {asset.department}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-slate-600 text-xs whitespace-nowrap">
                        {getOwnerName(asset.ownerLecturerId)}
                      </td>
                      <td className="px-4 py-3">
                        <div className="flex items-center gap-2">
                          <Link href={`/assets/${asset.id}`} className="text-xs text-blue-600 hover:underline">View</Link>
                          {canAdd && (
                            <Link href={`/assets/${asset.id}/edit`} className="text-xs text-slate-500 hover:underline">Edit</Link>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
          {/* Pagination */}
          <div className="px-4 py-3 border-t border-slate-100 flex items-center justify-between text-sm text-slate-500">
            <span>Showing {filtered.length} of {assets.length} results</span>
            <div className="flex items-center gap-1">
              <button className="px-3 py-1 rounded border border-slate-200 hover:bg-slate-50 disabled:opacity-40" disabled>←</button>
              <button className="px-3 py-1 rounded border border-blue-500 bg-blue-50 text-blue-600 font-medium">1</button>
              <button className="px-3 py-1 rounded border border-slate-200 hover:bg-slate-50">2</button>
              <button className="px-3 py-1 rounded border border-slate-200 hover:bg-slate-50">→</button>
            </div>
          </div>
        </div>
      </div>
    </MainLayout>
  );
}
