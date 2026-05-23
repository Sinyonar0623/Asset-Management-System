"use client"

// API:
// GET /Request/{id}
// Returns: { request: RequestDto }
// PATCH /MarkProcessed
// Request: { requestId, decision, comment, assetIds }
// Returns: { isSuccess: boolean }

import * as React from "react"
import { useParams } from "next/navigation"
import { CheckIcon, ImageIcon, Loader2Icon, XIcon } from "lucide-react"

import { ApiNotice } from "@/components/ce-ams/api-notice"
import { ApprovalTimeline } from "@/components/ce-ams/approval-timeline"
import { PageHeader } from "@/components/ce-ams/page-header"
import { RoleBadge } from "@/components/ce-ams/role-badge"
import { StatusBadge } from "@/components/ce-ams/status-badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Textarea } from "@/components/ui/textarea"
import { toast } from "@/components/ui/sonner"
import { useAuth } from "@/context/AuthContext"
import {
  getAssetById,
  getAssetUnitImages,
  getAssetUnitsByAssetId,
  getAssetsByLaboratory,
  formatDate,
  getRequestById,
  markRequestProcessed,
  shortId,
  type AssetDto,
  type AssetUnitDto,
  type AssetUnitImageDto,
  type RequestDto,
} from "@/lib/ce-ams-api"

const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL?.replace(/\/$/, "")

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

function getAssetStatus(asset?: AssetDto | null) {
  if (!asset) return "UNKNOWN"
  return asset.availabilityStatus || (asset.isAvailable ? "AVAILABLE" : "UNAVAILABLE")
}

function getAssetUnitLabel(unit: AssetUnitDto) {
  return unit.assetTag || unit.serialNo || unit.name || shortId(unit.id)
}

function resolveAssetImageUrl(imageUrl: string) {
  const normalizedUrl = imageUrl.trim().replaceAll("\\", "/")

  if (/^(data:|blob:)/i.test(normalizedUrl)) {
    return normalizedUrl
  }

  if (/^https?:/i.test(normalizedUrl)) {
    return encodeURI(normalizedUrl)
  }

  if (apiBaseUrl) {
    const path = normalizedUrl.startsWith("/") ? normalizedUrl : `/${normalizedUrl}`
    return encodeURI(`${apiBaseUrl}${path}`)
  }

  return encodeURI(normalizedUrl)
}

function isSameId(left?: string | null, right?: string | null) {
  return Boolean(left && right && left.toLowerCase() === right.toLowerCase())
}

export default function RequestDetailPage() {
  const params = useParams<{ id: string }>()
  const { session } = useAuth()
  const [request, setRequest] = React.useState<RequestDto | null>(null)
  const [assets, setAssets] = React.useState<AssetDto[]>([])
  const [attachedAssets, setAttachedAssets] = React.useState<AssetDto[]>([])
  const [previewAsset, setPreviewAsset] = React.useState<AssetDto | null>(null)
  const [previewAssetUnits, setPreviewAssetUnits] = React.useState<AssetUnitDto[]>([])
  const [previewUnitImages, setPreviewUnitImages] = React.useState<AssetUnitImageDto[]>([])
  const [selectedPreviewUnitId, setSelectedPreviewUnitId] = React.useState("")
  const [comment, setComment] = React.useState("")
  const [selectedAssetIds, setSelectedAssetIds] = React.useState<string[]>([])
  const [isSubmitting, setSubmitting] = React.useState(false)
  const [isLoadingAssets, setLoadingAssets] = React.useState(false)
  const [isLoadingAttachedAssets, setLoadingAttachedAssets] = React.useState(false)
  const [isLoadingPreviewUnits, setLoadingPreviewUnits] = React.useState(false)
  const [isLoadingPreviewImages, setLoadingPreviewImages] = React.useState(false)

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

  React.useEffect(() => {
    let isActive = true

    if (!previewAsset?.id) {
      setPreviewAssetUnits([])
      setPreviewUnitImages([])
      setSelectedPreviewUnitId("")
      return
    }

    setLoadingPreviewUnits(true)
    setLoadingPreviewImages(true)
    getAssetUnitsByAssetId(previewAsset.id)
      .then(async (units) => {
        if (!isActive) return

        setPreviewAssetUnits(units)

        if (units.length === 0) {
          setSelectedPreviewUnitId("")
          setPreviewUnitImages([])
          return
        }

        const detailResults = await Promise.allSettled(
          units
            .filter((unit) => Boolean(unit.id))
            .map(async (unit) => ({
              unitId: unit.id as string,
              images: await getAssetUnitImages(unit.id as string),
            }))
        )

        if (!isActive) return

        const firstUnitWithImages = detailResults.find(
          (result) =>
            result.status === "fulfilled" && result.value.images.length > 0
        )

        if (firstUnitWithImages?.status === "fulfilled") {
          setSelectedPreviewUnitId(firstUnitWithImages.value.unitId)
          setPreviewUnitImages(firstUnitWithImages.value.images)
          return
        }

        setSelectedPreviewUnitId(units[0]?.id || "")
        setPreviewUnitImages([])
      })
      .catch(() => {
        if (!isActive) return

        setPreviewAssetUnits([])
        setPreviewUnitImages([])
        setSelectedPreviewUnitId("")
      })
      .finally(() => {
        if (!isActive) return

        setLoadingPreviewUnits(false)
        setLoadingPreviewImages(false)
      })

    return () => {
      isActive = false
    }
  }, [previewAsset?.id])

  React.useEffect(() => {
    if (!selectedPreviewUnitId) {
      setPreviewUnitImages([])
      return
    }

    setLoadingPreviewImages(true)
    getAssetUnitImages(selectedPreviewUnitId)
      .then(setPreviewUnitImages)
      .catch(() => setPreviewUnitImages([]))
      .finally(() => setLoadingPreviewImages(false))
  }, [selectedPreviewUnitId])

  React.useEffect(() => {
    const assetIds = Array.from(
      new Set((request?.items ?? []).map((item) => item.assetId).filter(Boolean))
    )

    if (assetIds.length === 0) {
      setAttachedAssets([])
      return
    }

    let isActive = true
    setLoadingAttachedAssets(true)

    Promise.allSettled(assetIds.map((assetId) => getAssetById(assetId)))
      .then((results) => {
        if (!isActive) return

        setAttachedAssets(
          results.flatMap((result) => (result.status === "fulfilled" ? [result.value] : []))
        )
      })
      .finally(() => {
        if (isActive) setLoadingAttachedAssets(false)
      })

    return () => {
      isActive = false
    }
  }, [request?.items])

  const currentTracking = request?.trackings.find((item) => item.isCurrent)
  const currentUserRole = roleToApiRole(session?.role)
  const requestType = request?.requestType?.toUpperCase()
  const isOwnPendingRequest =
    request?.status === "PENDING" && isSameId(request.requesterId, session?.userId)
  const showActionPanel = !isOwnPendingRequest
  const canAct =
    showActionPanel &&
    request?.status === "PENDING" &&
    Boolean(currentTracking) &&
    (session?.role === "admin" || currentTracking?.requiredRoleCode === currentUserRole)
  const teacherNeedsAssets =
    requestType === "BORROW" && currentTracking?.requiredRoleCode === "TEACHER"
  const approvableAssets = assets.filter((asset) => asset.isAvailable !== false)
  const attachedAssetIds = new Set(attachedAssets.map((asset) => asset.id).filter(Boolean))
  const unresolvedAttachedAssetIds = (request?.items ?? [])
    .map((item) => item.assetId)
    .filter((assetId) => !attachedAssetIds.has(assetId))
  const selectedPreviewUnit = previewAssetUnits.find(
    (unit) => unit.id === selectedPreviewUnitId
  )

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

  function closePreview() {
    setPreviewAsset(null)
    setPreviewAssetUnits([])
    setPreviewUnitImages([])
    setSelectedPreviewUnitId("")
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

      <div
        className={
          showActionPanel ? "grid grid-cols-[1.2fr_0.8fr] gap-6" : "grid gap-6"
        }
      >
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

        {showActionPanel && (
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
        )}
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
          <CardTitle>Attached Assets</CardTitle>
        </CardHeader>
        <CardContent>
          {(request?.items ?? []).length === 0 ? (
            <p className="text-sm text-muted-foreground">No assets are attached to this request.</p>
          ) : isLoadingAttachedAssets ? (
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <Loader2Icon className="size-4 animate-spin" />
              Loading attached assets
            </div>
          ) : (
            <div className="space-y-3">
              {attachedAssets.map((asset) => (
                <div
                  key={asset.id ?? asset.name}
                  className="flex items-start justify-between gap-4 rounded-[16px] border border-border p-4"
                >
                  <div className="min-w-0 space-y-1">
                    <p className="truncate font-semibold text-foreground">
                      {asset.name || "Unnamed asset"}
                    </p>
                    <p className="truncate text-sm text-muted-foreground">
                      {asset.category || "Uncategorized"} · {asset.description || "No description"}
                    </p>
                    <p className="font-mono text-xs font-medium text-muted-foreground">
                      {shortId(asset.id)}
                    </p>
                  </div>
                  <div className="flex shrink-0 flex-col items-end gap-2">
                    <StatusBadge
                      status={getAssetStatus(asset)}
                    />
                    <span className="text-xs font-medium text-muted-foreground">
                      {asset.location || "Central storage"}
                    </span>
                    <Button
                      type="button"
                      variant="outline"
                      size="sm"
                      onClick={() => setPreviewAsset(asset)}
                    >
                      Open
                    </Button>
                  </div>
                </div>
              ))}

              {unresolvedAttachedAssetIds.map((assetId) => (
                <div
                  key={assetId}
                  className="flex items-center justify-between gap-4 rounded-[16px] border border-dashed border-border p-4"
                >
                  <div>
                    <p className="font-semibold text-foreground">Asset details unavailable</p>
                    <p className="mt-1 font-mono text-xs text-muted-foreground">
                      {shortId(assetId)}
                    </p>
                  </div>
                  <StatusBadge status="UNKNOWN" />
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      <Dialog open={Boolean(previewAsset)} onOpenChange={(nextOpen) => {
        if (!nextOpen) closePreview()
      }}>
        <DialogContent className="max-h-[min(90vh,760px)] max-w-[560px] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>{previewAsset?.name || "Asset preview"}</DialogTitle>
            <DialogDescription>
              Attached asset information for this request.
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4">
            <div className="flex items-center justify-between gap-4 rounded-[8px] border border-border p-4">
              <div className="min-w-0">
                <p className="truncate text-sm font-medium text-muted-foreground">
                  Asset ID
                </p>
                <p className="mt-1 font-mono text-sm font-semibold">
                  {shortId(previewAsset?.id)}
                </p>
              </div>
              <StatusBadge status={getAssetStatus(previewAsset)} />
            </div>

            <div className="grid gap-3 md:grid-cols-2">
              <div className="rounded-[8px] border border-border p-4">
                <p className="text-sm text-muted-foreground">Category</p>
                <p className="mt-1 font-semibold">
                  {previewAsset?.category || "Uncategorized"}
                </p>
              </div>
              <div className="rounded-[8px] border border-border p-4">
                <p className="text-sm text-muted-foreground">Location</p>
                <p className="mt-1 font-semibold">
                  {previewAsset?.location || "Central storage"}
                </p>
              </div>
            </div>

            <div className="rounded-[8px] border border-border p-4">
              <p className="text-sm text-muted-foreground">Description</p>
              <p className="mt-2 text-sm leading-6">
                {previewAsset?.description || "No description"}
              </p>
            </div>

            <div className="space-y-3 rounded-[8px] border border-border p-4">
              <div className="flex items-center justify-between gap-3">
                <p className="text-sm font-medium text-muted-foreground">Asset Units</p>
                {isLoadingPreviewUnits && (
                  <Loader2Icon className="size-4 animate-spin text-muted-foreground" />
                )}
              </div>

              {previewAssetUnits.length === 0 ? (
                <p className="text-sm text-muted-foreground">
                  {isLoadingPreviewUnits ? "Loading units" : "No asset units found"}
                </p>
              ) : (
                <>
                  <Select
                    value={selectedPreviewUnitId}
                    onValueChange={setSelectedPreviewUnitId}
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Select asset unit" />
                    </SelectTrigger>
                    <SelectContent>
                      {previewAssetUnits.map((unit) => (
                        <SelectItem key={unit.id ?? unit.assetTag} value={unit.id as string}>
                          {getAssetUnitLabel(unit)}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>

                  {selectedPreviewUnit && (
                    <div className="space-y-3">
                      <div className="grid gap-3 md:grid-cols-2">
                        <div className="rounded-[8px] bg-muted/40 p-3">
                          <p className="text-xs text-muted-foreground">Serial No</p>
                          <p className="mt-1 truncate text-sm font-semibold">
                            {selectedPreviewUnit.serialNo || "-"}
                          </p>
                        </div>
                        <div className="rounded-[8px] bg-muted/40 p-3">
                          <p className="text-xs text-muted-foreground">Brand</p>
                          <p className="mt-1 truncate text-sm font-semibold">
                            {selectedPreviewUnit.brand || "-"}
                          </p>
                        </div>
                        <div className="rounded-[8px] bg-muted/40 p-3">
                          <p className="text-xs text-muted-foreground">Availability</p>
                          <div className="mt-2">
                            <StatusBadge status={selectedPreviewUnit.availabilityStatus} />
                          </div>
                        </div>
                        <div className="rounded-[8px] bg-muted/40 p-3">
                          <p className="text-xs text-muted-foreground">Operational</p>
                          <div className="mt-2">
                            <StatusBadge status={selectedPreviewUnit.operationalStatus} />
                          </div>
                        </div>
                      </div>

                      <div className="overflow-hidden rounded-[8px] border border-border bg-muted">
                        <div className="border-b border-border bg-background px-4 py-3">
                          <p className="text-xs font-semibold text-muted-foreground">
                            Image for {getAssetUnitLabel(selectedPreviewUnit)}
                          </p>
                        </div>
                        {isLoadingPreviewImages ? (
                          <div className="flex h-56 items-center justify-center">
                            <Loader2Icon className="size-5 animate-spin text-muted-foreground" />
                          </div>
                        ) : previewUnitImages.length > 0 ? (
                          <div className="grid gap-3 p-3 sm:grid-cols-2">
                            {previewUnitImages.map((image) => (
                              <a
                                key={image.id}
                                href={resolveAssetImageUrl(image.imageUrl)}
                                target="_blank"
                                rel="noreferrer"
                                className="group overflow-hidden rounded-[8px] border border-border bg-card"
                              >
                                <div
                                  aria-label={
                                    image.description ||
                                    image.fileName ||
                                    getAssetUnitLabel(selectedPreviewUnit)
                                  }
                                  className="h-44 bg-muted bg-cover bg-center"
                                  role="img"
                                  style={{
                                    backgroundImage: `url("${resolveAssetImageUrl(image.imageUrl)}")`,
                                  }}
                                />
                                <div className="border-t border-border bg-background px-3 py-2">
                                  <p className="truncate text-xs font-semibold text-foreground">
                                    {image.description || image.fileName || "Unit image"}
                                  </p>
                                </div>
                              </a>
                            ))}
                          </div>
                        ) : (
                          <div className="flex h-56 flex-col items-center justify-center gap-3 text-muted-foreground">
                            <ImageIcon className="size-8" />
                            <p className="text-sm font-medium">No image for this unit</p>
                          </div>
                        )}
                      </div>
                    </div>
                  )}
                </>
              )}
            </div>
          </div>

          <DialogFooter>
            <Button type="button" onClick={closePreview}>
              Close
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

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
