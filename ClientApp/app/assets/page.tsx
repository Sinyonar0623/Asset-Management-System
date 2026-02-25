"use client";
import { useState, useEffect, useCallback } from "react";
import MainLayout from "../components/MainLayout";
import Header from "../components/Header";
import Link from "next/link";
import { useAuth } from "../contexts/AuthContext";
import { PERMISSIONS } from "../lib/auth";
import { assetApi } from "../lib/api";
import type { AssetSummary, PagedResult } from "../lib/api";

const STATUS_COLORS: Record<string, string> = {
  available: "bg-green-100 text-green-700",
  in_use: "bg-blue-100 text-blue-700",
  under_repair: "bg-orange-100 text-orange-700",
  inactive: "bg-slate-100 text-slate-600",
};
const STATUS_LABELS: Record<string, string> = {
  available: "Available", in_use: "In Use", under_repair: "Under Repair", inactive: "Inactive",
};

export default function AssetsPage() {
  const { session } = useAuth();
  const role = session?.role;
  const [search, setSearch] = useState("");
  const [filterStatus, setFilterStatus] = useState("");
  const [page, setPage] = useState(1);
  const [data, setData] = useState<PagedResult<AssetSummary> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const fetchAssets = useCallback(async () => {
    setLoading(true); setError("");
    try {
      const result = await assetApi.getAll({ page, pageSize: 20, search: search || undefined, status: filterStatus || undefined });
      setData(result);
    } catch (e) { setError((e as Error).message); }
    finally { setLoading(false); }
  }, [page, search, filterStatus]);

  useEffect(() => { fetchAssets(); }, [fetchAssets]);
  const canAdd = role && PERMISSIONS.canAddAsset(role);

  return (
    <MainLayout>
      <Header title="Asset Management" subtitle="รายการทรัพย์สินทั้งหมด"
        actions={canAdd ? (
          <Link href="/assets/new" className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700">
            <span>+</span> Add New Asset
          </Link>
        ) : undefined} />
      <div className="flex-1 p-6 space-y-4">
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
          {[{ label: "Total", value: data?.totalCount ?? 0 },
            { label: "Available", value: data?.items.filter(a => a.status === "available").length ?? 0 },
            { label: "In Use", value: data?.items.filter(a => a.status === "in_use").length ?? 0 },
            { label: "Under Repair", value: data?.items.filter(a => a.status === "under_repair").length ?? 0 }
          ].map((s) => (
            <div key={s.label} className="bg-white rounded-lg border border-slate-200 p-4 text-center shadow-sm">
              <div className="text-2xl font-bold text-slate-800">{s.value}</div>
              <div className="text-sm text-slate-500">{s.label}</div>
            </div>
          ))}
        </div>
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm p-4 flex flex-wrap gap-3">
          <input type="text" placeholder="Search name, ID, serial..." value={search}
            onChange={(e) => { setSearch(e.target.value); setPage(1); }}
            className="flex-1 min-w-48 px-4 py-2 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" />
          <select value={filterStatus} onChange={(e) => { setFilterStatus(e.target.value); setPage(1); }}
            className="px-3 py-2 text-sm border border-slate-200 rounded-lg bg-white">
            <option value="">All Status</option>
            <option value="available">Available</option>
            <option value="in_use">In Use</option>
            <option value="under_repair">Under Repair</option>
          </select>
        </div>
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
          {error ? <div className="p-8 text-center text-red-500">{error}</div>
          : loading ? <div className="p-8 text-center text-slate-400">Loading...</div>
          : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="bg-slate-50 border-b border-slate-200 text-left">
                    {["Asset ID","Asset Name","Type","Location","Status","Amount","Actions"].map(h => (
                      <th key={h} className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">{h}</th>
                    ))}
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-100">
                  {!data?.items.length ? (
                    <tr><td colSpan={7} className="px-4 py-10 text-center text-slate-400">No assets found</td></tr>
                  ) : data.items.map((asset) => (
                    <tr key={asset.id} className="hover:bg-slate-50 transition-colors">
                      <td className="px-4 py-3 font-mono text-xs text-slate-600">{asset.realWorldId}</td>
                      <td className="px-4 py-3">
                        <div className="font-medium text-slate-800">{asset.name}</div>
                        <div className="text-xs text-slate-400">{asset.serialNo}</div>
                      </td>
                      <td className="px-4 py-3 text-slate-600">{asset.type}</td>
                      <td className="px-4 py-3 text-slate-600 text-xs">{asset.laboratoryName ?? "-"}</td>
                      <td className="px-4 py-3">
                        <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${STATUS_COLORS[asset.status] ?? "bg-slate-100 text-slate-600"}`}>
                          {STATUS_LABELS[asset.status] ?? asset.status}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-slate-600">{asset.amount}</td>
                      <td className="px-4 py-3">
                        <div className="flex items-center gap-2">
                          <Link href={`/assets/${asset.id}`} className="text-xs text-blue-600 hover:underline">View</Link>
                          {canAdd && <Link href={`/assets/${asset.id}/edit`} className="text-xs text-slate-500 hover:underline">Edit</Link>}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
          {data && data.totalPages > 1 && (
            <div className="px-4 py-3 border-t border-slate-100 flex items-center justify-between text-sm text-slate-500">
              <span>Page {data.page} of {data.totalPages} ({data.totalCount} total)</span>
              <div className="flex gap-1">
                <button onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}
                  className="px-3 py-1 rounded border border-slate-200 hover:bg-slate-50 disabled:opacity-40">Prev</button>
                <button onClick={() => setPage(p => Math.min(data.totalPages, p + 1))} disabled={page === data.totalPages}
                  className="px-3 py-1 rounded border border-slate-200 hover:bg-slate-50 disabled:opacity-40">Next</button>
              </div>
            </div>
          )}
        </div>
      </div>
    </MainLayout>
  );
}
