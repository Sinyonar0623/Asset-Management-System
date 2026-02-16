"use client";
import { useState } from "react";
import MainLayout from "../components/MainLayout";
import Header from "../components/Header";
import ProtectedRoute from "../components/ProtectedRoute";
import { useAuth } from "../contexts/AuthContext";
import { PERMISSIONS, MOCK_USERS } from "../lib/auth";

// assetOwnerId: null = belongs to department (depthead approves), "user-002" = belongs to lecturer1
const allRequestsData = [
  { id: "BRW-010", type: "Borrow", asset: "Dell Laptop Latitude 5420", assetId: "AST-001", assetOwnerId: "user-002", requester: "นายสมชาย ใจดี", requesterRole: "Student", date: "2024-01-15", borrowDate: "2024-01-16", returnDate: "2024-01-20", purpose: "ใช้ทำโปรเจกต์วิชา CPE441 ระบบฝังตัว", status: "Pending" },
  { id: "REP-005", type: "Repair", asset: "Projector Epson EB-X51", assetId: "AST-003", assetOwnerId: "user-002", requester: "ผศ.ดร.วิชัย สุขใจ", requesterRole: "Lecturer", date: "2024-01-14", borrowDate: "—", returnDate: "—", purpose: "หลอดไฟเสีย ไม่ติด", status: "Pending" },
  { id: "BRW-011", type: "Borrow", asset: "Arduino Uno Kit", assetId: "AST-004", assetOwnerId: null, requester: "นางสาวสุดา มีทอง", requesterRole: "Student", date: "2024-01-14", borrowDate: "2024-01-15", returnDate: "2024-01-22", purpose: "Lab IoT Assignment", status: "Pending" },
  { id: "BRW-008", type: "Borrow", asset: "Oscilloscope Rigol DS1054Z", assetId: "AST-005", assetOwnerId: null, requester: "นายประสิทธิ์ ดีมาก", requesterRole: "Student", date: "2024-01-13", borrowDate: "2024-01-14", returnDate: "2024-01-15", purpose: "Experiment circuit analysis", status: "Approved" },
  { id: "REP-003", type: "Repair", asset: "Soldering Iron Kit", assetId: "AST-010", assetOwnerId: null, requester: "นางสาวรัตนา ช่วยดี", requesterRole: "Admin", date: "2024-01-12", borrowDate: "—", returnDate: "—", purpose: "หัวแร้งไม่ร้อน", status: "Completed" },
  { id: "BRW-007", type: "Borrow", asset: "Desktop PC #3", assetId: "AST-006", assetOwnerId: null, requester: "นายมานะ", requesterRole: "Student", date: "2024-01-10", borrowDate: "2024-01-11", returnDate: "2024-01-12", purpose: "Compile project", status: "Rejected" },
];

const statusColors: Record<string, string> = {
  Pending:      "bg-yellow-100 text-yellow-700",
  Approved:     "bg-blue-100 text-blue-700",
  "In Progress": "bg-purple-100 text-purple-700",
  Completed:    "bg-green-100 text-green-700",
  Rejected:     "bg-red-100 text-red-700",
};

function getApproverName(assetOwnerId: string | null): string {
  if (!assetOwnerId) return "รศ.ดร.สมศักดิ์ ดีงาม (หัวหน้าภาควิชา)";
  const user = MOCK_USERS.find((u) => u.id === assetOwnerId);
  return user ? `${user.name} (อาจารย์)` : "หัวหน้าภาควิชา";
}

export default function ApprovalsPage() {
  const { session } = useAuth();
  const [tab, setTab] = useState<"pending" | "all">("pending");
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [rejectReason, setRejectReason] = useState("");
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [rejectTarget, setRejectTarget] = useState<string | null>(null);
  const [processedIds, setProcessedIds] = useState<Record<string, string>>({});

  // Filter requests by what this user is allowed to approve
  const scopedRequests = allRequestsData.filter((r) => {
    if (!session) return false;
    return PERMISSIONS.canApproveForAsset(session.role, r.assetOwnerId, session.userId);
  });

  const pendingScoped = scopedRequests.filter((r) => (processedIds[r.id] || r.status) === "Pending");
  const list = tab === "pending" ? scopedRequests.filter((r) => r.status === "Pending" && !processedIds[r.id]) : scopedRequests;

  const selected = scopedRequests.find((r) => r.id === selectedId);

  const handleApprove = (id: string) => {
    setProcessedIds((p) => ({ ...p, [id]: "Approved" }));
    setSelectedId(null);
  };

  const handleReject = (id: string) => {
    setRejectTarget(id);
    setShowRejectModal(true);
  };

  const confirmReject = () => {
    if (rejectTarget) {
      setProcessedIds((p) => ({ ...p, [rejectTarget]: "Rejected" }));
    }
    setShowRejectModal(false);
    setRejectReason("");
    setRejectTarget(null);
    setSelectedId(null);
  };

  const getStatus = (r: typeof allRequestsData[0]) => processedIds[r.id] || r.status;

  return (
    <ProtectedRoute requiredPermission="canApprove">
    <MainLayout>
      <Header
        title="Approvals"
        subtitle={`${pendingScoped.length} requests pending your approval`}
      />

      <div className="flex-1 p-6 flex gap-6 min-h-0">
        {/* Left: List */}
        <div className="flex-1 flex flex-col min-w-0">
          {/* Tabs */}
          <div className="flex gap-1 bg-slate-100 rounded-lg p-1 mb-4 w-fit">
            <button
              onClick={() => setTab("pending")}
              className={`px-4 py-1.5 rounded-md text-sm font-medium transition-colors ${tab === "pending" ? "bg-white text-slate-800 shadow-sm" : "text-slate-500 hover:text-slate-700"}`}
            >
              Pending ({pendingScoped.length})
            </button>
            <button
              onClick={() => setTab("all")}
              className={`px-4 py-1.5 rounded-md text-sm font-medium transition-colors ${tab === "all" ? "bg-white text-slate-800 shadow-sm" : "text-slate-500 hover:text-slate-700"}`}
            >
              All Requests ({scopedRequests.length})
            </button>
          </div>

          {/* Request Cards */}
          <div className="space-y-3 overflow-y-auto flex-1">
            {list.length === 0 && (
              <div className="text-center py-12 text-slate-400">
                <div className="text-4xl mb-3">✅</div>
                <div className="font-medium">ไม่มีคำขอที่รอการอนุมัติ</div>
              </div>
            )}
            {list.map((r) => {
              const status = getStatus(r);
              const isSelected = selectedId === r.id;
              return (
                <div
                  key={r.id}
                  onClick={() => setSelectedId(isSelected ? null : r.id)}
                  className={`bg-white rounded-xl border cursor-pointer transition-all shadow-sm ${isSelected ? "border-blue-500 ring-2 ring-blue-100" : "border-slate-200 hover:border-slate-300"}`}
                >
                  <div className="p-4">
                    <div className="flex items-start justify-between mb-2">
                      <div className="flex items-center gap-2">
                        <span className={`px-2 py-0.5 rounded text-xs font-medium ${r.type === "Borrow" ? "bg-blue-50 text-blue-600" : "bg-orange-50 text-orange-600"}`}>
                          {r.type}
                        </span>
                        <span className="font-mono text-xs text-slate-400">{r.id}</span>
                      </div>
                      <span className={`px-2.5 py-0.5 rounded-full text-xs font-medium ${statusColors[status]}`}>
                        {status}
                      </span>
                    </div>
                    <div className="font-medium text-slate-800 text-sm">{r.asset}</div>
                    <div className="text-xs text-slate-500 mt-1">{r.requester} · {r.requesterRole} · {r.date}</div>
                    <div className="text-xs text-slate-400 mt-1 truncate">{r.purpose}</div>
                    {status === "Pending" && !processedIds[r.id] && (
                      <div className="flex gap-2 mt-3">
                        <button
                          onClick={(e) => { e.stopPropagation(); handleApprove(r.id); }}
                          className="flex-1 py-1.5 bg-green-500 text-white rounded-lg text-xs font-medium hover:bg-green-600"
                        >
                          ✓ Approve
                        </button>
                        <button
                          onClick={(e) => { e.stopPropagation(); handleReject(r.id); }}
                          className="flex-1 py-1.5 bg-red-500 text-white rounded-lg text-xs font-medium hover:bg-red-600"
                        >
                          ✕ Reject
                        </button>
                      </div>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        </div>

        {/* Right: Detail Panel */}
        {selected && (
          <div className="w-80 shrink-0">
            <div className="bg-white rounded-xl border border-slate-200 shadow-sm sticky top-6">
              <div className="px-5 py-4 border-b border-slate-100 flex items-center justify-between">
                <h3 className="font-semibold text-slate-800">Request Detail</h3>
                <button onClick={() => setSelectedId(null)} className="text-slate-400 hover:text-slate-600 text-sm">✕</button>
              </div>
              <div className="p-5 space-y-4">
                <div className="space-y-0 text-sm">
                  {([
                    ["Request ID", selected.id],
                    ["Type", selected.type],
                    ["Asset", selected.asset],
                    ["Asset ID", selected.assetId],
                    ["Requester", selected.requester],
                    ["Role", selected.requesterRole],
                    ["Request Date", selected.date],
                    ...(selected.type === "Borrow" ? [
                      ["Borrow Date", selected.borrowDate],
                      ["Return Date", selected.returnDate],
                    ] : []),
                    ["Approver", getApproverName(selected.assetOwnerId)],
                  ] as [string, string][]).map(([label, value]) => (
                    <div key={label} className="flex justify-between py-1.5 border-b border-slate-50">
                      <span className="text-slate-500 shrink-0 mr-2">{label}</span>
                      <span className="font-medium text-slate-800 text-right text-xs max-w-40 break-words">{value}</span>
                    </div>
                  ))}
                </div>

                <div>
                  <div className="text-xs font-semibold text-slate-500 uppercase tracking-wide mb-1">Purpose / Issue</div>
                  <div className="text-sm text-slate-700 bg-slate-50 rounded-lg p-3 leading-relaxed">{selected.purpose}</div>
                </div>

                <div>
                  <div className="text-xs font-semibold text-slate-500 uppercase tracking-wide mb-1">Status</div>
                  <span className={`px-3 py-1 rounded-full text-xs font-medium ${statusColors[getStatus(selected)]}`}>
                    {getStatus(selected)}
                  </span>
                </div>

                {getStatus(selected) === "Pending" && !processedIds[selected.id] && (
                  <div className="flex gap-2 pt-2">
                    <button
                      onClick={() => handleApprove(selected.id)}
                      className="flex-1 py-2 bg-green-500 text-white rounded-lg text-sm font-medium hover:bg-green-600"
                    >
                      ✓ Approve
                    </button>
                    <button
                      onClick={() => handleReject(selected.id)}
                      className="flex-1 py-2 bg-red-500 text-white rounded-lg text-sm font-medium hover:bg-red-600"
                    >
                      ✕ Reject
                    </button>
                  </div>
                )}
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Reject Modal */}
      {showRejectModal && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-md">
            <div className="px-6 py-4 border-b border-slate-100">
              <h3 className="font-semibold text-slate-800">Reject Request</h3>
            </div>
            <div className="p-6 space-y-4">
              <p className="text-sm text-slate-600">กรุณาระบุเหตุผลในการปฏิเสธคำขอนี้</p>
              <textarea
                value={rejectReason}
                onChange={(e) => setRejectReason(e.target.value)}
                rows={4}
                placeholder="เหตุผลการปฏิเสธ..."
                className="w-full px-4 py-2.5 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-red-400 resize-none"
              />
              <div className="flex gap-3">
                <button
                  onClick={() => { setShowRejectModal(false); setRejectReason(""); }}
                  className="flex-1 py-2 border border-slate-200 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-50"
                >
                  Cancel
                </button>
                <button
                  onClick={confirmReject}
                  className="flex-1 py-2 bg-red-500 text-white rounded-lg text-sm font-medium hover:bg-red-600"
                >
                  Confirm Reject
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </MainLayout>
    </ProtectedRoute>
  );
}
