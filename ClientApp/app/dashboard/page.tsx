"use client";

import MainLayout from "../components/MainLayout";
import Header from "../components/Header";
import Link from "next/link";
import { useAuth } from "../contexts/AuthContext";
import { PERMISSIONS } from "../lib/auth";

const stats = [
  { label: "Total Assets", value: "248", change: "+12 this month", color: "blue", icon: "📦" },
  { label: "Available", value: "182", change: "73% of total", color: "green", icon: "✅" },
  { label: "In Use", value: "45", change: "18% of total", color: "yellow", icon: "🔄" },
  { label: "Under Repair", value: "21", change: "9% of total", color: "red", icon: "🔧" },
];

const recentRequests = [
  { id: "REQ-001", type: "Borrow", asset: "Dell Laptop #12", requester: "นายสมชาย ใจดี", date: "2024-01-15", status: "Pending" },
  { id: "REQ-002", type: "Repair", asset: "Projector #3", requester: "ผศ.ดร.วิชัย", date: "2024-01-14", status: "Approved" },
  { id: "REQ-003", type: "Borrow", asset: "Arduino Kit #5", requester: "นางสาวสุดา", date: "2024-01-14", status: "In Progress" },
  { id: "REQ-004", type: "Borrow", asset: "Oscilloscope #2", requester: "นายประสิทธิ์", date: "2024-01-13", status: "Completed" },
  { id: "REQ-005", type: "Repair", asset: "Desktop PC #8", requester: "นางสาวรัตนา", date: "2024-01-12", status: "Rejected" },
];

const statusColors: Record<string, string> = {
  Pending: "bg-yellow-100 text-yellow-700",
  Approved: "bg-blue-100 text-blue-700",
  "In Progress": "bg-purple-100 text-purple-700",
  Completed: "bg-green-100 text-green-700",
  Rejected: "bg-red-100 text-red-700",
};

const assetCategories = [
  { name: "Computers & Laptops", count: 85, percent: 34 },
  { name: "Electronic Equipment", count: 62, percent: 25 },
  { name: "Lab Equipment", count: 48, percent: 19 },
  { name: "Audio/Visual", count: 31, percent: 13 },
  { name: "Furniture & Others", count: 22, percent: 9 },
];

export default function DashboardPage() {
  const { session } = useAuth();
  const role = session?.role;

  return (
    <MainLayout>
      <Header
        title="Dashboard"
        subtitle="ภาพรวมระบบบริหารจัดการทรัพย์สิน ภาควิชาวิศวกรรมคอมพิวเตอร์"
      />

      <div className="flex-1 p-6 space-y-6">
        {/* Stats */}
        <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4">
          {stats.map((s) => (
            <div key={s.label} className="bg-white rounded-xl border border-slate-200 p-5 flex items-start gap-4 shadow-sm">
              <div className={`w-12 h-12 rounded-xl flex items-center justify-center text-2xl
                ${s.color === "blue" ? "bg-blue-50" :
                  s.color === "green" ? "bg-green-50" :
                  s.color === "yellow" ? "bg-yellow-50" : "bg-red-50"}`}>
                {s.icon}
              </div>
              <div>
                <div className="text-2xl font-bold text-slate-800">{s.value}</div>
                <div className="text-sm font-medium text-slate-600">{s.label}</div>
                <div className="text-xs text-slate-400 mt-0.5">{s.change}</div>
              </div>
            </div>
          ))}
        </div>

        <div className="grid grid-cols-1 xl:grid-cols-3 gap-6">
          {/* Recent Requests */}
          <div className="xl:col-span-2 bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-5 py-4 border-b border-slate-100 flex items-center justify-between">
              <h2 className="font-semibold text-slate-800">Recent Requests</h2>
              <Link href="/approvals" className="text-sm text-blue-600 hover:underline">View all</Link>
            </div>
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="text-left text-xs text-slate-500 border-b border-slate-100">
                    <th className="px-5 py-3 font-medium">Request ID</th>
                    <th className="px-5 py-3 font-medium">Type</th>
                    <th className="px-5 py-3 font-medium">Asset</th>
                    <th className="px-5 py-3 font-medium">Requester</th>
                    <th className="px-5 py-3 font-medium">Date</th>
                    <th className="px-5 py-3 font-medium">Status</th>
                  </tr>
                </thead>
                <tbody>
                  {recentRequests.map((r) => (
                    <tr key={r.id} className="border-b border-slate-50 hover:bg-slate-50 transition-colors">
                      <td className="px-5 py-3 font-mono text-xs text-slate-600">{r.id}</td>
                      <td className="px-5 py-3">
                        <span className={`px-2 py-0.5 rounded text-xs font-medium ${r.type === "Borrow" ? "bg-blue-50 text-blue-600" : "bg-orange-50 text-orange-600"}`}>
                          {r.type}
                        </span>
                      </td>
                      <td className="px-5 py-3 text-slate-700">{r.asset}</td>
                      <td className="px-5 py-3 text-slate-600">{r.requester}</td>
                      <td className="px-5 py-3 text-slate-500">{r.date}</td>
                      <td className="px-5 py-3">
                        <span className={`px-2 py-0.5 rounded-full text-xs font-medium ${statusColors[r.status]}`}>
                          {r.status}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>

          {/* Asset Categories */}
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-5 py-4 border-b border-slate-100">
              <h2 className="font-semibold text-slate-800">Asset Categories</h2>
            </div>
            <div className="p-5 space-y-4">
              {assetCategories.map((c) => (
                <div key={c.name}>
                  <div className="flex justify-between text-sm mb-1.5">
                    <span className="text-slate-700 font-medium">{c.name}</span>
                    <span className="text-slate-500">{c.count}</span>
                  </div>
                  <div className="w-full bg-slate-100 rounded-full h-2">
                    <div
                      className="bg-blue-500 h-2 rounded-full transition-all"
                      style={{ width: `${c.percent}%` }}
                    />
                  </div>
                  <div className="text-xs text-slate-400 mt-0.5">{c.percent}%</div>
                </div>
              ))}
            </div>

            {/* Quick Actions — filtered by role */}
            <div className="px-5 pb-5 space-y-2">
              <div className="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-3">Quick Actions</div>
              {role && PERMISSIONS.canAddAsset(role) && (
                <Link href="/assets/new" className="flex items-center gap-2 w-full px-4 py-2.5 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 transition-colors">
                  <span>+</span> Add New Asset
                </Link>
              )}
              <Link href="/borrow" className="flex items-center gap-2 w-full px-4 py-2.5 bg-slate-100 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-200 transition-colors">
                <span>📤</span> New Borrow Request
              </Link>
              {role && PERMISSIONS.canCreateRepair(role) && (
                <Link href="/repair" className="flex items-center gap-2 w-full px-4 py-2.5 bg-slate-100 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-200 transition-colors">
                  <span>🔧</span> New Repair Request
                </Link>
              )}
            </div>
          </div>
        </div>
      </div>
    </MainLayout>
  );
}
