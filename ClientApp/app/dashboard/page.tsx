"use client";
import { useState, useEffect } from "react";
import MainLayout from "../components/MainLayout";
import Header from "../components/Header";
import Link from "next/link";
import { useAuth } from "../contexts/AuthContext";
import { PERMISSIONS } from "../lib/auth";
import { assetApi, borrowApi, repairApi, PagedResult, AssetSummary, BorrowRequest, RepairRequest } from "../lib/api";

const STATUS_COLORS: Record<string, string> = {
  Pending: "bg-yellow-100 text-yellow-700",
  Approved: "bg-blue-100 text-blue-700",
  "In Progress": "bg-purple-100 text-purple-700",
  Completed: "bg-green-100 text-green-700",
  Rejected: "bg-red-100 text-red-700",
  Returned: "bg-green-100 text-green-700",
  "In Repair": "bg-orange-100 text-orange-700",
};

type RecentItem = {
  id: string;
  type: "Borrow" | "Repair";
  asset: string;
  requester: string;
  date: string;
  status: string;
};

export default function DashboardPage() {
  const { session } = useAuth();
  const role = session?.role;

  const [assetStats, setAssetStats] = useState({ total: 0, active: 0, borrowed: 0, underRepair: 0 });
  const [recentItems, setRecentItems] = useState<RecentItem[]>([]);
  const [loadingStats, setLoadingStats] = useState(true);

  useEffect(() => {
    // Load asset stats
    assetApi.getAll({ pageSize: 1 }).then((r) => {
      setAssetStats((s) => ({ ...s, total: r.totalCount }));
    }).catch(console.error);

    assetApi.getAll({ status: "Active", pageSize: 1 }).then((r) => {
      setAssetStats((s) => ({ ...s, active: r.totalCount }));
    }).catch(console.error);

    assetApi.getAll({ status: "Borrowed", pageSize: 1 }).then((r) => {
      setAssetStats((s) => ({ ...s, borrowed: r.totalCount }));
    }).catch(console.error);

    assetApi.getAll({ status: "Under Repair", pageSize: 1 }).then((r) => {
      setAssetStats((s) => ({ ...s, underRepair: r.totalCount }));
      setLoadingStats(false);
    }).catch(() => setLoadingStats(false));

    // Load recent requests
    Promise.all([
      borrowApi.getAll({ pageSize: 5 }),
      repairApi.getAll({ pageSize: 5 }),
    ]).then(([borrows, repairs]) => {
      const b: RecentItem[] = borrows.items.map((r) => ({
        id: `BRW-${r.id}`,
        type: "Borrow",
        asset: r.assetName,
        requester: r.requesterName,
        date: r.createOn?.slice(0, 10) ?? "—",
        status: r.status,
      }));
      const rp: RecentItem[] = repairs.items.map((r) => ({
        id: `REP-${r.id}`,
        type: "Repair",
        asset: r.assetName,
        requester: r.requesterName,
        date: r.createOn?.slice(0, 10) ?? "—",
        status: r.status,
      }));
      setRecentItems([...b, ...rp].sort((a, b) => b.id.localeCompare(a.id)).slice(0, 8));
    }).catch(console.error);
  }, []);

  const stats = [
    { label: "Total Assets", value: assetStats.total, change: "ทรัพย์สินทั้งหมด", color: "blue", icon: "📦" },
    { label: "Active", value: assetStats.active, change: "พร้อมใช้งาน", color: "green", icon: "✅" },
    { label: "Borrowed", value: assetStats.borrowed, change: "กำลังถูกยืม", color: "yellow", icon: "🔄" },
    { label: "Under Repair", value: assetStats.underRepair, change: "กำลังซ่อม", color: "red", icon: "🔧" },
  ];

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
                <div className="text-2xl font-bold text-slate-800">
                  {loadingStats ? <span className="inline-block w-8 h-6 bg-slate-100 rounded animate-pulse" /> : s.value}
                </div>
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
            {recentItems.length === 0 ? (
              <div className="text-center py-10 text-slate-400 text-sm">ยังไม่มีคำขอ</div>
            ) : (
              <div className="overflow-x-auto">
                <table className="w-full text-sm">
                  <thead>
                    <tr className="text-left text-xs text-slate-500 border-b border-slate-100">
                      <th className="px-5 py-3 font-medium">ID</th>
                      <th className="px-5 py-3 font-medium">Type</th>
                      <th className="px-5 py-3 font-medium">Asset</th>
                      <th className="px-5 py-3 font-medium">Requester</th>
                      <th className="px-5 py-3 font-medium">Date</th>
                      <th className="px-5 py-3 font-medium">Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    {recentItems.map((r) => (
                      <tr key={r.id} className="border-b border-slate-50 hover:bg-slate-50 transition-colors">
                        <td className="px-5 py-3 font-mono text-xs text-slate-600">{r.id}</td>
                        <td className="px-5 py-3">
                          <span className={`px-2 py-0.5 rounded text-xs font-medium ${r.type === "Borrow" ? "bg-blue-50 text-blue-600" : "bg-orange-50 text-orange-600"}`}>
                            {r.type}
                          </span>
                        </td>
                        <td className="px-5 py-3 text-slate-700 max-w-32 truncate">{r.asset}</td>
                        <td className="px-5 py-3 text-slate-600 max-w-28 truncate">{r.requester}</td>
                        <td className="px-5 py-3 text-slate-500">{r.date}</td>
                        <td className="px-5 py-3">
                          <span className={`px-2 py-0.5 rounded-full text-xs font-medium ${STATUS_COLORS[r.status] ?? "bg-slate-100 text-slate-600"}`}>
                            {r.status}
                          </span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>

          {/* Quick Actions */}
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-5 py-4 border-b border-slate-100">
              <h2 className="font-semibold text-slate-800">Quick Actions</h2>
            </div>
            <div className="p-5 space-y-2">
              {role && PERMISSIONS.canAddAsset(role) && (
                <Link href="/assets/new"
                  className="flex items-center gap-2 w-full px-4 py-2.5 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 transition-colors">
                  <span>+</span> Add New Asset
                </Link>
              )}
              <Link href="/borrow"
                className="flex items-center gap-2 w-full px-4 py-2.5 bg-slate-100 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-200 transition-colors">
                <span>📤</span> New Borrow Request
              </Link>
              <Link href="/repair"
                className="flex items-center gap-2 w-full px-4 py-2.5 bg-slate-100 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-200 transition-colors">
                <span>🔧</span> New Repair Request
              </Link>
              {role && PERMISSIONS.canApprove(role) && (
                <Link href="/approvals"
                  className="flex items-center gap-2 w-full px-4 py-2.5 bg-slate-100 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-200 transition-colors">
                  <span>✅</span> View Approvals
                </Link>
              )}
              <Link href="/assets"
                className="flex items-center gap-2 w-full px-4 py-2.5 bg-slate-100 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-200 transition-colors">
                <span>📦</span> Browse Assets
              </Link>
            </div>

            <div className="px-5 pb-5">
              <div className="bg-slate-50 rounded-lg p-4 border border-slate-100">
                <div className="text-xs font-semibold text-slate-500 uppercase mb-2">Signed in as</div>
                <div className="font-medium text-slate-800 text-sm">{session?.name || session?.username}</div>
                <div className="text-xs text-slate-500">{session?.email}</div>
                <div className="mt-1.5">
                  <span className="px-2 py-0.5 rounded text-xs font-medium bg-blue-100 text-blue-700">
                    {session?.roleName}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </MainLayout>
  );
}
