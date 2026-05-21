"use client"

// API:
// GET /Request/{id}
// Returns: { request: RequestDto }
// PATCH /MarkProcessed
// Request: { requestId, decision, comment, assetIds }
// Returns: { isSuccess: boolean }

import * as React from "react"
import { useParams } from "next/navigation"
import { CheckIcon, Loader2Icon, XIcon } from "lucide-react"

import { ApiNotice } from "@/components/ce-ams/api-notice"
import { ApprovalTimeline } from "@/components/ce-ams/approval-timeline"
import { PageHeader } from "@/components/ce-ams/page-header"
import { RoleBadge } from "@/components/ce-ams/role-badge"
import { StatusBadge } from "@/components/ce-ams/status-badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Textarea } from "@/components/ui/textarea"
import { toast } from "@/components/ui/sonner"
import { useAuth } from "@/context/AuthContext"
import {
  getAssetsByLaboratory,
  formatDate,
  getRequestById,
  markRequestProcessed,
  shortId,
  type AssetDto,
  type RequestDto,
} from "@/lib/ce-ams-api"

function roleToApiRole(role?: string) {
  if (role === "lecturer") return "TEACHER"
  if (role === "depthead") return "HOD"
  if (role === "admin") return "ADMIN"
  return "STUDENT"
}

function getRequestTypeLabel(requestType?: string | null) {
  const normalized = (requestType || "").toUpperCase()
  if (normalized === "ALLOCATE") return "Allocation"
  if (normalized === "BORROW") return "Borrow"
  if (normalized === "REPAIR") return "Repair"
  if (normalized === "RETIRE") return "Retirement"
  return requestType || "Loading"
}

function getWorkflowSubtitle(requestType?: string | null) {
  const normalized = (requestType || "").toUpperCase()
  if (normalized === "ALLOCATE") return "Teacher allocation request ending with HOD approval."
  if (normalized === "REPAIR") return "Asset repair request ending with HOD approval."
  if (normalized === "RETIRE") return "Asset retirement request ending with HOD approval."
  if (normalized === "BORROW") return "Student borrow request through Teacher and HOD approval."
  return "Request approval workflow."
}

function getReasonLabel(requestType?: string | null) {
  const normalized = (requestType || "").toUpperCase()
  if (normalized === "ALLOCATE") return "Allocation Reason"
  if (normalized === "REPAIR") return "Repair Reason"
  if (normalized === "RETIRE") return "Retirement Reason"
  if (normalized === "BORROW") return "Borrow Reason"
  return "Reason"
}

export default function RequestDetailPage() {
  const params = useParams<{ id: string }>()
  const { session } = useAuth()
  const [request, setRequest] = React.useState<RequestDto | null>(null)
  const [assets, setAssets] = React.useState<AssetDto[]>([])
  const [comment, setComment] = React.useState("")
  const [selectedAssetIds, setSelectedAssetIds] = React.useState<string[]>([])
  const [isSubmitting, setSubmitting] = React.useState(false)
  const [isLoadingAssets, setLoadingAssets] = React.useState(false)

  const loadRequest = React.useCallback(() => {
    if (!params.id) return
    getRequestById(params.id).then(setRequest).catch(() => {
      toast({
        title: "Unable to load request",
        description: "Please confirm the backend is running and your session is valid.",
        variant: "destructive",
      })
    })
  }, [params.id])

  React.useEffect(() => {
    loadRequest()
  }, [loadRequest])

  const currentTracking = request?.trackings.find((item) => item.isCurrent)
  const currentUserRole = roleToApiRole(session?.role)
  const requestType = request?.requestType?.toUpperCase()
  const canAct =
    request?.status === "PENDING" &&
    Boolean(currentTracking) &&
    (session?.role === "admin" || currentTracking?.requiredRoleCode === currentUserRole)
  const teacherNeedsAssets =
    requestType === "BORROW" && currentTracking?.requiredRoleCode === "TEACHER"
  const approvableAssets = assets.filter((asset) => asset.isAvailable !== false)

  React.useEffect(() => {
    if (!request?.targetLaboratoryId || !teacherNeedsAssets || !canAct) {
      setAssets([])
      setSelectedAssetIds([])
      return
    }

    setLoadingAssets(true)
    getAssetsByLaboratory(request.targetLaboratoryId, 0, 50)
      .then((result) => setAssets(result.items))
      .catch(() => {
        setAssets([])
        toast({
          title: "Unable to load approvable assets",
          description: "Teacher approval requires assets from the request laboratory.",
          variant: "destructive",
        })
      })
      .finally(() => setLoadingAssets(false))
  }, [canAct, request?.targetLaboratoryId, teacherNeedsAssets])

  function toggleAsset(assetId: string) {
    setSelectedAssetIds((current) =>
      current.includes(assetId)
        ? current.filter((id) => id !== assetId)
        : [...current, assetId]
    )
  }

  async function process(decision: "APPROVE" | "REJECT") {
    if (!request || !currentTracking) return

    if (decision === "APPROVE" && teacherNeedsAssets && selectedAssetIds.length === 0) {
      toast({
        title: "Asset IDs required",
        description: "Select at least one asset before approving this borrow request.",
        variant: "destructive",
      })
      return
    }

    setSubmitting(true)
    try {
      await markRequestProcessed({
        requestId: request.id,
        decision,
        comment,
        assetIds: teacherNeedsAssets ? selectedAssetIds : [],
      })
      toast(decision === "APPROVE" ? "Request approved" : "Request rejected")
      setComment("")
      setSelectedAssetIds([])
      loadRequest()
    } catch {
      toast({
        title: "Action failed",
        description: "The backend rejected this action or your role is not authorized for the current step.",
        variant: "destructive",
      })
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="space-y-6">
      <PageHeader
        title={request ? `Request ${shortId(request.id)}` : "Request Detail"}
        subtitle={getWorkflowSubtitle(request?.requestType)}
      />

      <div className="grid grid-cols-[1.2fr_0.8fr] gap-6">
        <Card>
          <CardHeader>
            <CardTitle>Request Information</CardTitle>
          </CardHeader>
          <CardContent className="grid grid-cols-2 gap-4 text-sm">
            <div className="rounded-[16px] border border-border p-4">
              <p className="text-muted-foreground">Status</p>
              <div className="mt-2">
                <StatusBadge status={request?.status} />
              </div>
            </div>
            <div className="rounded-[16px] border border-border p-4">
              <p className="text-muted-foreground">Request Type</p>
              <p className="mt-2 font-semibold">{getRequestTypeLabel(request?.requestType)}</p>
            </div>
            <div className="rounded-[16px] border border-border p-4">
              <p className="text-muted-foreground">Requester</p>
              <p className="mt-2 font-mono text-xs font-semibold">{shortId(request?.requesterId)}</p>
            </div>
            <div className="rounded-[16px] border border-border p-4">
              <p className="text-muted-foreground">Created</p>
              <p className="mt-2 font-semibold">{formatDate(request?.submittedOn)}</p>
            </div>
            <div className="col-span-2 rounded-[16px] border border-border p-4">
              <p className="text-muted-foreground">{getReasonLabel(request?.requestType)}</p>
              <p className="mt-2 font-medium">{request?.reason || "No reason provided"}</p>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Action Panel</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="flex items-center justify-between gap-4 rounded-[16px] border border-border p-4">
              <div>
                <p className="text-sm font-semibold">Current Step</p>
                <p className="mt-1 text-xs text-muted-foreground">
                  {currentTracking ? `Step ${currentTracking.stepNo}` : "No active approval step"}
                </p>
              </div>
              {currentTracking ? <RoleBadge role={currentTracking.requiredRoleCode} /> : <StatusBadge status="COMPLETED" />}
            </div>

            {!canAct && (
              <ApiNotice title="Action unavailable">
                This panel enables actions only when your role matches the current approval step.
              </ApiNotice>
            )}

            {teacherNeedsAssets && (
              <div className="space-y-3 rounded-[16px] border border-border p-3">
                <div className="flex items-center justify-between gap-3">
                  <span className="text-sm font-medium">Assign assets</span>
                  {isLoadingAssets && (
                    <Loader2Icon className="size-4 animate-spin text-muted-foreground" />
                  )}
                </div>
                {approvableAssets.length === 0 ? (
                  <p className="text-sm text-muted-foreground">
                    {isLoadingAssets ? "Loading assets" : "No available assets found for this laboratory."}
                  </p>
                ) : (
                  <div className="max-h-56 space-y-2 overflow-y-auto">
                    {approvableAssets.map((asset) => {
                      if (!asset.id) return null

                      return (
                        <label
                          key={asset.id}
                          className="flex cursor-pointer items-start gap-3 rounded-[12px] border border-border p-3 text-sm"
                        >
                          <input
                            type="checkbox"
                            className="mt-1"
                            checked={selectedAssetIds.includes(asset.id)}
                            disabled={!canAct}
                            onChange={() => toggleAsset(asset.id as string)}
                          />
                          <span className="min-w-0">
                            <span className="block truncate font-semibold text-foreground">
                              {asset.name || "Unnamed asset"}
                            </span>
                            <span className="block truncate text-xs text-muted-foreground">
                              {asset.category || "Uncategorized"}
                            </span>
                          </span>
                        </label>
                      )
                    })}
                  </div>
                )}
              </div>
            )}

            <label className="space-y-2">
              <span className="text-sm font-medium">Decision note</span>
              <Textarea
                value={comment}
                onChange={(event) => setComment(event.target.value)}
                disabled={!canAct}
                placeholder="Add a note for the approval history"
              />
            </label>

            <div className="flex items-center gap-3">
              <Button
                className="flex-1"
                disabled={!canAct || isSubmitting}
                onClick={() => process("APPROVE")}
              >
                <CheckIcon className="size-4" />
                Approve
              </Button>
              <Button
                className="flex-1"
                variant="destructive"
                disabled={!canAct || isSubmitting}
                onClick={() => process("REJECT")}
              >
                <XIcon className="size-4" />
                Reject
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Approval Timeline</CardTitle>
        </CardHeader>
        <CardContent>
          <ApprovalTimeline trackings={request?.trackings ?? []} requestType={request?.requestType} />
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Notes / History</CardTitle>
        </CardHeader>
        <CardContent className="space-y-3">
          {(request?.trackings ?? []).length === 0 ? (
            <p className="text-sm text-muted-foreground">No tracking history returned by the API yet.</p>
          ) : (
            request?.trackings.map((tracking) => (
              <div
                key={`${tracking.stepNo}-${tracking.status}`}
                className="flex items-start justify-between gap-4 rounded-[16px] border border-border p-4"
              >
                <div className="space-y-2">
                  <div className="flex items-center gap-2">
                    <RoleBadge role={tracking.requiredRoleCode} />
                    <StatusBadge status={tracking.status} />
                  </div>
                  <p className="text-sm text-muted-foreground">
                    {tracking.comment || "No comment recorded"}
                  </p>
                </div>
                <div className="text-right text-xs font-medium text-muted-foreground">
                  <p>Step {tracking.stepNo}</p>
                  <p>{formatDate(tracking.actionOn)}</p>
                </div>
              </div>
            ))
          )}
        </CardContent>
      </Card>
    </div>
  )
}
