"use client";
import { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import MainLayout from "../../components/MainLayout";
import Header from "../../components/Header";
import Link from "next/link";
import { useAuth } from "../../contexts/AuthContext";
import { assetApi, AssetDetail } from "../../lib/api";
import { PERMISSIONS } from "../../lib/auth";

const STATUS_COLORS: Record<string, string> = {
  Active: "bg-green-100 text-green-700",
  Inactive: "bg-slate-100 text-slate-500",
  Borrowed: "bg-blue-100 text-blue-700",
  "Under Repair": "bg-orange-100 text-orange-700",
};

export default function AssetDetailPage() {
  const params = useParams();
  const router = useRouter();
  const { session } = useAuth();
  const id = Number(params.id);

  const [asset, setAsset] = useState<AssetDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [deleting, setDeleting] = useState(false);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);

  useEffect(() => {
    if (!id) return;
    setLoading(true);
    assetApi
      .getById(id)
      .then(setAsset)
      .catch((e) => setError(e.message || "ไม่พบทรัพย์สิน"))
      .finally(() => setLoading(false));
  }, [id]);

  const handleDelete = async () => {
    setDeleting(true);
    try {
      await assetApi.delete(id);
      router.push("/assets");
    } catch (e) {
      setError((e as Error).message || "ลบไม่สำเร็จ");
      setDeleting(false);
      setShowDeleteConfirm(false);
    }
  };

  const canEdit =
    session &&
    (PERMISSIONS.canAddAsset(session.role) ||
      session.userId === asset?.ownerId);

  if (loading) {
    return (
      <MainLayout>
        <div className="flex-1 flex items-center justify-center">
          <div className="w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full animate-spin" />
        </div>
      </MainLayout>
    );
  }

  if (error || !asset) {
    return (
      <MainLayout>
        <div className="flex-1 flex items-center justify-center p-6">
          <div className="text-center">
            <div className="text-4xl mb-3">⚠️</div>
            <div className="font-semibold text-slate-700 mb-1">{error || "ไม่พบทรัพย์สิน"}</div>
            <Link href="/assets" className="text-sm text-blue-600 hover:underline">
              กลับหน้ารายการ
            </Link>
          </div>
        </div>
      </MainLayout>
    );
  }

  return (
    <MainLayout>
      <Header
        title={asset.name}
        subtitle={`${asset.realWorldId} · ${asset.type}`}
        actions={
          <div className="flex items-center gap-2">
            <Link
              href="/assets"
              className="px-4 py-2 border border-slate-200 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-50"
            >
              Back
            </Link>
            {canEdit && (
              <>
                <Link
                  href={`/assets/${id}/edit`}
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700"
                >
                  Edit
                </Link>
                <button
                  onClick={() => setShowDeleteConfirm(true)}
                  className="px-4 py-2 bg-red-500 text-white rounded-lg text-sm font-medium hover:bg-red-600"
                >
                  Delete
                </button>
              </>
            )}
          </div>
        }
      />

      <div className="flex-1 p-6 space-y-6 max-w-4xl mx-auto w-full">
        {error && (
          <div className="px-4 py-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">
            {error}
          </div>
        )}

        {/* Main info */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
          <div className="px-6 py-4 border-b border-slate-100 flex items-center justify-between">
            <h2 className="font-semibold text-slate-800">Asset Information</h2>
            <span
              className={`px-3 py-1 rounded-full text-xs font-medium ${STATUS_COLORS[asset.status] || "bg-slate-100 text-slate-600"}`}
            >
              {asset.status}
            </span>
          </div>
          <div className="p-6 grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-4 text-sm">
            {[
              ["Asset Name", asset.name],
              ["Real World ID", asset.realWorldId],
              ["Brand", asset.brand],
              ["Serial No.", asset.serialNo],
              ["Type", asset.type],
              ["Amount", String(asset.amount)],
              ["Status", asset.status],
              ["Owner ID", asset.ownerId],
            ].map(([label, value]) => (
              <div key={label} className="flex justify-between border-b border-slate-50 pb-2">
                <span className="text-slate-500 font-medium">{label}</span>
                <span className="text-slate-800 text-right">{value || "—"}</span>
              </div>
            ))}
            {asset.description && (
              <div className="md:col-span-2 border-b border-slate-50 pb-2">
                <span className="text-slate-500 font-medium block mb-1">Description</span>
                <span className="text-slate-700">{asset.description}</span>
              </div>
            )}
            {asset.remark && (
              <div className="md:col-span-2">
                <span className="text-slate-500 font-medium block mb-1">Remark</span>
                <span className="text-slate-700">{asset.remark}</span>
              </div>
            )}
          </div>
        </div>

        {/* Laboratory */}
        {asset.laboratory && (
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-6 py-4 border-b border-slate-100">
              <h2 className="font-semibold text-slate-800">Laboratory</h2>
            </div>
            <div className="p-6 grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-4 text-sm">
              {[
                ["Lab Name", asset.laboratory.laboratoryName],
                ["Room No.", asset.laboratory.roomNo],
                ["Teacher ID", asset.laboratory.teacherId],
                ["Description", asset.laboratory.description],
              ].map(([label, value]) => (
                <div key={label} className="flex justify-between border-b border-slate-50 pb-2">
                  <span className="text-slate-500 font-medium">{label}</span>
                  <span className="text-slate-800 text-right">{value || "—"}</span>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Components */}
        {asset.components && asset.components.length > 0 && (
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-6 py-4 border-b border-slate-100">
              <h2 className="font-semibold text-slate-800">
                Components ({asset.components.length})
              </h2>
            </div>
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="bg-slate-50 border-b border-slate-100 text-left">
                    <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase">Name</th>
                    <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase">Brand</th>
                    <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase">Serial</th>
                    <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase">Type</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-50">
                  {asset.components.map((c) => (
                    <tr key={c.id} className="hover:bg-slate-50">
                      <td className="px-5 py-3 font-medium text-slate-800">{c.name}</td>
                      <td className="px-5 py-3 text-slate-600">{c.brand || "—"}</td>
                      <td className="px-5 py-3 text-slate-500 font-mono text-xs">{c.serialNo || "—"}</td>
                      <td className="px-5 py-3 text-slate-600">{c.type || "—"}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>

      {/* Delete Confirm Modal */}
      {showDeleteConfirm && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-sm">
            <div className="px-6 py-4 border-b border-slate-100">
              <h3 className="font-semibold text-slate-800">Confirm Delete</h3>
            </div>
            <div className="p-6 space-y-4">
              <p className="text-sm text-slate-600">
                คุณแน่ใจหรือไม่ว่าต้องการลบทรัพย์สิน <strong>{asset.name}</strong>?
                การกระทำนี้ไม่สามารถย้อนกลับได้
              </p>
              <div className="flex gap-3">
                <button
                  onClick={() => setShowDeleteConfirm(false)}
                  className="flex-1 py-2 border border-slate-200 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-50"
                >
                  Cancel
                </button>
                <button
                  onClick={handleDelete}
                  disabled={deleting}
                  className="flex-1 py-2 bg-red-500 text-white rounded-lg text-sm font-medium hover:bg-red-600 disabled:opacity-60"
                >
                  {deleting ? "กำลังลบ..." : "Delete"}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </MainLayout>
  );
}
