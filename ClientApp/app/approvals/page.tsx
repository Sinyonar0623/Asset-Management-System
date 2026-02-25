"use client";
import { useState, useEffect, useCallback } from "react";
import MainLayout from "../components/MainLayout";
import Header from "../components/Header";
import { borrowApi, repairApi, BorrowRequest, RepairRequest } from "../lib/api";

const STATUS_COLORS: Record<string, string> = {
  Pending: "bg-yellow-100 text-yellow-700",
  Approved: "bg-blue-100 text-blue-700",
  Returned: "bg-green-100 text-green-700",
  Completed: "bg-green-100 text-green-700",
  Rejected: "bg-red-100 text-red-700",
  "In Repair": "bg-orange-100 text-orange-700",
};

type UnifiedReq =
  | (BorrowRequest & { reqType: "Borrow" })
  | (RepairRequest & { reqType: "Repair" });

export default function ApprovalsPage() {
  const [requests, setRequests] = useState<UnifiedReq[]>([]);
  const [loading, setLoading] = useState(true);
  const [tab, setTab] = useState<"pending" | "all">("pending");
  const [selectedUid, setSelectedUid] = useState<string | null>(null);
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [rejectTarget, setRejectTarget] = useState<{ id: number; type: "Borrow" | "Repair" } | null>(null);
  const [rejectRemark, setRejectRemark] = useState("");
  const [processing, setProcessing] = useState(false);
  const [successMsg, setSuccessMsg] = useState("");

  const loadAll = useCallback(() => {
    setLoading(true);
    Promise.all([
      borrowApi.getAll({ pageSize: 100 }),
      repairApi.getAll({ pageSize: 100 }),
    ])
      .then(([b, rp]) => {
        const borrows = b.items.map((r) => ({ ...r, reqType: "Borrow" as const }));
        const repairs = rp.items.map((r) => ({ ...r, reqType: "Repair" as const }));
        setRequests([...borrows, ...repairs].sort((a, b) => b.id - a.id));
      })
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => { loadAll(); }, [loadAll]);

  const pending = requests.filter((r) => r.status === "Pending");
  const list = tab === "pending" ? pending : requests;
  const selected = requests.find((r) => `${r.reqType}-${r.id}` === selectedUid);

  const handleApprove = async (req: UnifiedReq) => {
    setProcessing(true);
    try {
      if (req.reqType === "Borrow") await borrowApi.approve(req.id);
      else await repairApi.approve(req.id);
      setSuccessMsg(`Approved request #${req.id}`);
      setSelectedUid(null);
      loadAll();
    } catch (err) {
      alert((err as Error).message);
    } finally {
      setProcessing(false);
    }
  };

  const openReject = (req: UnifiedReq) => {
    setRejectTarget({ id: req.id, type: req.reqType });
    setShowRejectModal(true);
  };

  const confirmReject = async () => {
    if (!rejectTarget) return;
    setProcessing(true);
    try {
      if (rejectTarget.type === "Borrow") await borrowApi.reject(rejectTarget.id, rejectRemark);
      else await repairApi.reject(rejectTarget.id, rejectRemark);
      setSuccessMsg(`Rejected request #${rejectTarget.id}`);
      setShowRejectModal(false);
      setRejectRemark("");
      setRejectTarget(null);
      setSelectedUid(null);
      loadAll();
    } catch (err) {
      alert((err as Error).message);
    } finally {
      setProcessing(false);
    }
  };

  const getDesc = (r: UnifiedReq) =>
    r.reqType === "Borrow"
      ? (r as BorrowRequest & { reqType: "Borrow" }).purpose
      : (r as RepairRequest & { reqType: "Repair" }).problemDescription;

  return (
    <MainLayout>
      <Header title="Approvals" subtitle={`${pending.length} requests pending approval`} />

      <div className="flex-1 p-6 flex gap-6 min-h-0 overflow-hidden">
        <div className="flex-1 flex flex-col min-w-0 overflow-hidden">
          {successMsg && (
            <div className="bg-green-50 border border-green-200 rounded-xl p-3 flex items-center gap-2 mb-4 text-sm text-green-800">
              ✅ {successMsg}
              <button onClick={() => setSuccessMsg("")} className="ml-auto text-green-400">✕</button>
            </div>
          )}

          <div className="flex gap-1 bg-slate-100 rounded-lg p-1 mb-4 w-fit shrink-0">
            <button onClick={() => setTab("pending")}
              className={`px-4 py-1.5 rounded-md text-sm font-medium transition-colors ${tab === "pending" ? "bg-white text-slate-800 shadow-sm" : "text-slate-500 hover:text-slate-700"}`}>
              Pending ({pending.length})
            </button>
            <button onClick={() => setTab("all")}
              className={`px-4 py-1.5 rounded-md text-sm font-medium transition-colors ${tab === "all" ? "bg-white text-slate-800 shadow-sm" : "text-slate-500 hover:text-slate-700"}`}>
              All ({requests.length})
            </button>
          </div>

          {loading ? (
            <div className="flex items-center justify-center py-12">
              <div className="w-6 h-6 border-4 border-blue-600 border-t-transparent rounded-full animate-spin" />
            </div>
          ) : (
            <div className="space-y-3 overflow-y-auto flex-1">
              {list.length === 0 && (
                <div className="text-center py-12 text-slate-400">
                  <div className="text-4xl mb-3">✅</div>
                  <div className="font-medium">ไม่มีคำขอที่รอการอนุมัติ</div>
                </div>
              )}
              {list.map((r) => {
                const uid = `${r.reqType}-${r.id}`;
                const isSelected = selectedUid === uid;
                return (
                  <div key={uid} onClick={() => setSelectedUid(isSelected ? null : uid)}
                    className={`bg-white rounded-xl border cursor-pointer transition-all shadow-sm ${isSelected ? "border-blue-500 ring-2 ring-blue-100" : "border-slate-200 hover:border-slate-300"}`}>
                    <div className="p-4">
                      <div className="flex items-start justify-between mb-2">
                        <div className="flex items-center gap-2">
                          <span className={`px-2 py-0.5 rounded text-xs font-medium ${r.reqType === "Borrow" ? "bg-blue-50 text-blue-600" : "bg-orange-50 text-orange-600"}`}>
                            {r.reqType}
                          </span>
                          <span className="font-mono text-xs text-slate-400">#{r.id}</span>
                        </div>
                        <span className={`px-2.5 py-0.5 rounded-full text-xs font-medium ${STATUS_COLORS[r.status] ?? "bg-slate-100 text-slate-600"}`}>
                          {r.status}
                        </span>
                      </div>
                      <div className="font-medium text-slate-800 text-sm">{r.assetName}</div>
                      <div className="text-xs text-slate-500 mt-1">{r.requesterName}</div>
                      <div className="text-xs text-slate-400 mt-1 truncate">{getDesc(r)}</div>
                      {r.status === "Pending" && (
                        <div className="flex gap-2 mt-3">
                          <button onClick={(e) => { e.stopPropagation(); handleApprove(r); }} disabled={processing}
                            className="flex-1 py-1.5 bg-green-500 text-white rounded-lg text-xs font-medium hover:bg-green-600 disabled:opacity-60">
                            ✓ Approve
                          </button>
                          <button onClick={(e) => { e.stopPropagation(); openReject(r); }} disabled={processing}
                            className="flex-1 py-1.5 bg-red-500 text-white rounded-lg text-xs font-medium hover:bg-red-600 disabled:opacity-60">
                            ✕ Reject
                          </button>
                        </div>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>

        {selected && (
          <div className="w-80 shrink-0">
            <div className="bg-white rounded-xl border border-slate-200 shadow-sm sticky top-6">
              <div className="px-5 py-4 border-b border-slate-100 flex items-center justify-between">
                <h3 className="font-semibold text-slate-800">Request Detail</h3>
                <button onClick={() => setSelectedUid(null)} className="text-slate-400 hover:text-slate-600 text-sm">✕</button>
              </div>
              <div className="p-5 space-y-3 text-sm">
                {[
                  ["Type", selected.reqType],
                  ["Asset", selected.assetName],
                  ["Requester", selected.requesterName],
                  ["Status", selected.status],
                  ...(selected.reqType === "Borrow" ? [
                    ["Borrow Date", (selected as BorrowRequest & {reqType:"Borrow"}).borrowDate?.slice(0,10) ?? "—"],
                    ["Return Date", (selected as BorrowRequest & {reqType:"Borrow"}).returnDate?.slice(0,10) ?? "—"],
                  ] : []),
                ].map(([label, value]) => (
                  <div key={label} className="flex justify-between py-1 border-b border-slate-50">
                    <span className="text-slate-500">{label}</span>
                    <span className="font-medium text-right max-w-40">{value}</span>
                  </div>
                ))}
                <div className="pt-1">
                  <div className="text-xs font-semibold text-slate-500 uppercase mb-1">
                    {selected.reqType === "Borrow" ? "Purpose" : "Problem"}
                  </div>
                  <div className="text-sm text-slate-700 bg-slate-50 rounded-lg p-3 leading-relaxed">{getDesc(selected)}</div>
                </div>
                {selected.status === "Pending" && (
                  <div className="flex gap-2 pt-2">
                    <button onClick={() => handleApprove(selected)} disabled={processing}
                      className="flex-1 py-2 bg-green-500 text-white rounded-lg text-sm font-medium hover:bg-green-600 disabled:opacity-60">
                      ✓ Approve
                    </button>
                    <button onClick={() => openReject(selected)} disabled={processing}
                      className="flex-1 py-2 bg-red-500 text-white rounded-lg text-sm font-medium hover:bg-red-600 disabled:opacity-60">
                      ✕ Reject
                    </button>
                  </div>
                )}
              </div>
            </div>
          </div>
        )}
      </div>

      {showRejectModal && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-md">
            <div className="px-6 py-4 border-b border-slate-100">
              <h3 className="font-semibold text-slate-800">Reject Request</h3>
            </div>
            <div className="p-6 space-y-4">
              <p className="text-sm text-slate-600">กรุณาระบุเหตุผลในการปฏิเสธคำขอนี้</p>
              <textarea value={rejectRemark} onChange={(e) => setRejectRemark(e.target.value)} rows={4}
                placeholder="เหตุผลการปฏิเสธ..."
                className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-red-400 resize-none" />
              <div className="flex gap-3">
                <button onClick={() => { setShowRejectModal(false); setRejectRemark(""); }}
                  className="flex-1 py-2 border border-slate-200 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-50">
                  Cancel
                </button>
                <button onClick={confirmReject} disabled={processing}
                  className="flex-1 py-2 bg-red-500 text-white rounded-lg text-sm font-medium hover:bg-red-600 disabled:opacity-60">
                  {processing ? "กำลังดำเนินการ..." : "Confirm Reject"}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </MainLayout>
  );
}
