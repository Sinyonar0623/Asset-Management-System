"use client"

import * as React from "react"
import { ImageIcon, Loader2Icon, SearchIcon, SendIcon } from "lucide-react"

import { Button } from "@/components/ui/button"
import { StatusBadge } from "@/components/ce-ams/status-badge"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { toast } from "@/components/ui/sonner"
import { Textarea } from "@/components/ui/textarea"
import { useAuth } from "@/context/AuthContext"
import {
  createRequest,
  getAllocatableAssets,
  getAssetUnitImages,
  getAssetUnitsByAssetId,
  getAssetsByLaboratory,
  getLaboratories,
  shortId,
  type AssetDto,
  type AssetUnitImageDto,
  type AssetUnitDto,
  type CreateRequestPayload,
  type LaboratoryDto,
  type RequestDetailDto,
} from "@/lib/ce-ams-api"

type RequestType = "ALLOCATE" | "BORROW" | "REPAIR" | "RETIRE"

const requestTypeLabels: Record<RequestType, string> = {
  ALLOCATE: "Allocate to Lab",
  BORROW: "Borrow",
  REPAIR: "Repair",
  RETIRE: "Retire",
}

const assetSectionTitle: Record<Exclude<RequestType, "BORROW">, string> = {
  ALLOCATE: "Assets from central storage",
  REPAIR: "Assets in target laboratory",
  RETIRE: "Assets in target laboratory",
}

const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL?.replace(/\/$/, "")

function getLaboratoryLabel(laboratory: LaboratoryDto) {
  const name = laboratory.laboratoryName?.trim()
  const roomNo = laboratory.roomNo?.trim()

  if (name && roomNo) {
    return `${name} (${roomNo})`
  }

  return name || roomNo || laboratory.id || "Unnamed laboratory"
}

function toApiDate(value: string) {
  return value ? new Date(`${value}T00:00:00`).toISOString() : null
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

export function CreateRequestDialog({ onCreated }: { onCreated?: () => void }) {
  const { session } = useAuth()
  const [open, setOpen] = React.useState(false)
  const [previewAsset, setPreviewAsset] = React.useState<AssetDto | null>(null)
  const [previewAssetUnits, setPreviewAssetUnits] = React.useState<AssetUnitDto[]>([])
  const [previewUnitImages, setPreviewUnitImages] = React.useState<AssetUnitImageDto[]>([])
  const [selectedPreviewUnitId, setSelectedPreviewUnitId] = React.useState("")
  const [laboratories, setLaboratories] = React.useState<LaboratoryDto[]>([])
  const [assets, setAssets] = React.useState<AssetDto[]>([])
  const [requestType, setRequestType] = React.useState<RequestType>("BORROW")
  const [targetLaboratoryId, setTargetLaboratoryId] = React.useState("")
  const [assetSearch, setAssetSearch] = React.useState("")
  const [quantity, setQuantity] = React.useState("1")
  const [purpose, setPurpose] = React.useState("")
  const [borrowFrom, setBorrowFrom] = React.useState("")
  const [borrowTo, setBorrowTo] = React.useState("")
  const [issueDescription, setIssueDescription] = React.useState("")
  const [retireReason, setRetireReason] = React.useState("")
  const [extraNote, setExtraNote] = React.useState("")
  const [selectedAssetIds, setSelectedAssetIds] = React.useState<string[]>([])
  const [reason, setReason] = React.useState("")
  const [isSubmitting, setSubmitting] = React.useState(false)
  const [isLoadingAssets, setLoadingAssets] = React.useState(false)
  const [isLoadingPreviewUnits, setLoadingPreviewUnits] = React.useState(false)
  const [isLoadingPreviewImages, setLoadingPreviewImages] = React.useState(false)

  const canCreateMaintenance = session?.role !== "student"
  const sessionRole = session?.role
  const sessionUserId = session?.userId
  const sessionLookupKey = `${sessionRole ?? ""}:${sessionUserId ?? ""}`
  const requestTypeOptions: Array<{ value: RequestType; label: string }> =
    sessionRole === "admin"
      ? [
        { value: "ALLOCATE", label: requestTypeLabels.ALLOCATE },
        { value: "BORROW", label: requestTypeLabels.BORROW },
        { value: "REPAIR", label: requestTypeLabels.REPAIR },
        { value: "RETIRE", label: requestTypeLabels.RETIRE },
      ]
      : canCreateMaintenance
        ? [
          { value: "ALLOCATE", label: requestTypeLabels.ALLOCATE },
          { value: "REPAIR", label: requestTypeLabels.REPAIR },
          { value: "RETIRE", label: requestTypeLabels.RETIRE },
        ]
        : [{ value: "BORROW", label: requestTypeLabels.BORROW }]

  const requiresAssets = requestType !== "BORROW"

  React.useEffect(() => {
    if (!open) return

    const [currentRole, currentUserId] = sessionLookupKey.split(":")

    setRequestType(currentRole === "admin" || !canCreateMaintenance ? "BORROW" : "ALLOCATE")

    getLaboratories()
      .then((items) => {
        const visibleLaboratories =
          currentRole === "lecturer" && currentUserId
            ? items.filter(
              (laboratory) =>
                laboratory.teacherId?.toLowerCase() === currentUserId.toLowerCase()
            )
            : items

        setLaboratories(visibleLaboratories)
        setTargetLaboratoryId((current) => {
          if (current && visibleLaboratories.some((laboratory) => laboratory.id === current)) {
            return current
          }

          return visibleLaboratories[0]?.id || ""
        })
      })
      .catch(() => {
        toast({
          title: "Unable to load laboratories",
          description: "The request API is available, but laboratory lookup failed.",
          variant: "destructive",
        })
      })
  }, [canCreateMaintenance, open, sessionLookupKey])

  React.useEffect(() => {
    if (!open || !requiresAssets || !targetLaboratoryId) {
      setAssets([])
      setAssetSearch("")
      setSelectedAssetIds([])
      return
    }

    setAssetSearch("")
    setSelectedAssetIds([])
    setLoadingAssets(true)
    const loader =
      requestType === "ALLOCATE"
        ? getAllocatableAssets(0, 50)
        : getAssetsByLaboratory(targetLaboratoryId, 0, 50)

    loader
      .then((result) => setAssets(result.items))
      .catch(() => {
        setAssets([])
        toast({
          title: "Unable to load assets",
          description:
            requestType === "ALLOCATE"
              ? "Allocation requires GET /Asset/allocatable."
              : "Asset selection requires GET /Asset/laboratory/{laboratoryId}.",
          variant: "destructive",
        })
      })
      .finally(() => setLoadingAssets(false))
  }, [open, requestType, requiresAssets, targetLaboratoryId])

  React.useEffect(() => {
    if (!previewAsset?.id) {
      setPreviewAssetUnits([])
      setPreviewUnitImages([])
      setSelectedPreviewUnitId("")
      return
    }

    setLoadingPreviewUnits(true)
    getAssetUnitsByAssetId(previewAsset.id)
      .then((units) => {
        setPreviewAssetUnits(units)
        setSelectedPreviewUnitId(units[0]?.id || "")
      })
      .catch(() => {
        setPreviewAssetUnits([])
        setPreviewUnitImages([])
        setSelectedPreviewUnitId("")
      })
      .finally(() => setLoadingPreviewUnits(false))
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

  const canSubmit = React.useMemo(() => {
    if (!targetLaboratoryId || !reason.trim()) return false

    if (requestType === "BORROW") {
      return Boolean(purpose.trim() && Number(quantity) > 0)
    }

    if (selectedAssetIds.length === 0) return false

    if (requestType === "REPAIR") {
      return Boolean(issueDescription.trim())
    }

    if (requestType === "RETIRE") {
      return Boolean(retireReason.trim())
    }

    return true
  }, [
    issueDescription,
    purpose,
    quantity,
    reason,
    requestType,
    retireReason,
    selectedAssetIds.length,
    targetLaboratoryId,
  ])

  const visibleAssets = React.useMemo(() => {
    const query = assetSearch.trim().toLowerCase()
    if (!query) return assets

    return assets.filter((asset) => {
      const target = [
        asset.name,
        asset.description,
        asset.category,
        asset.location,
        shortId(asset.id),
      ]
        .filter(Boolean)
        .join(" ")
        .toLowerCase()

      return target.includes(query)
    })
  }, [assetSearch, assets])

  function resetTypeSpecificFields() {
    setAssets([])
    setAssetSearch("")
    setSelectedAssetIds([])
    setQuantity("1")
    setPurpose("")
    setBorrowFrom("")
    setBorrowTo("")
    setIssueDescription("")
    setRetireReason("")
    setExtraNote("")
  }

  function handleRequestTypeChange(value: string) {
    setRequestType(value as RequestType)
    resetTypeSpecificFields()
  }

  function toggleAsset(assetId: string) {
    setSelectedAssetIds((current) =>
      current.includes(assetId)
        ? current.filter((id) => id !== assetId)
        : [...current, assetId]
    )
  }

  function buildDetail(): RequestDetailDto {
    if (requestType === "BORROW") {
      return {
        purpose: purpose.trim(),
        borrowFrom: toApiDate(borrowFrom),
        borrowTo: toApiDate(borrowTo),
        extraNote: `Requested quantity: ${quantity}`,
      }
    }

    if (requestType === "ALLOCATE") {
      return {
        purpose: "Long-term allocation to target laboratory",
        extraNote: extraNote.trim() || null,
      }
    }

    if (requestType === "REPAIR") {
      return {
        issueDescription: issueDescription.trim(),
        extraNote: extraNote.trim() || null,
      }
    }

    return {
      retireReason: retireReason.trim(),
      extraNote: extraNote.trim() || null,
    }
  }

  function resetForm() {
    setReason("")
    setPreviewAsset(null)
    setPreviewAssetUnits([])
    setPreviewUnitImages([])
    setSelectedPreviewUnitId("")
    resetTypeSpecificFields()
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!canSubmit) return

    const payload: CreateRequestPayload = {
      requestType,
      targetLaboratoryId,
      reason: reason.trim(),
      detail: buildDetail(),
      items: requiresAssets
        ? selectedAssetIds.map((assetId) => ({ assetId }))
        : null,
    }

    setSubmitting(true)
    try {
      await createRequest(payload)
      toast("Request submitted")
      setOpen(false)
      resetForm()
      onCreated?.()
    } catch {
      toast({
        title: "Request could not be submitted",
        description: "Please confirm the selected request type and assets are valid for your role.",
        variant: "destructive",
      })
    } finally {
      setSubmitting(false)
    }
  }

  function handleOpenChange(nextOpen: boolean) {
    setOpen(nextOpen)
    if (!nextOpen) {
      setPreviewAsset(null)
      setPreviewAssetUnits([])
      setPreviewUnitImages([])
      setSelectedPreviewUnitId("")
    }
  }

  function closePreview() {
    setPreviewAsset(null)
    setPreviewAssetUnits([])
    setPreviewUnitImages([])
    setSelectedPreviewUnitId("")
  }

  const selectedPreviewUnit = previewAssetUnits.find(
    (unit) => unit.id === selectedPreviewUnitId
  )

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <Button>
          <SendIcon className="size-4" />
          New Request
        </Button>
      </DialogTrigger>
      <DialogContent className="max-h-[min(90vh,760px)] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Create {requestTypeLabels[requestType]} Request</DialogTitle>
          <DialogDescription>
            {requestType === "BORROW"
              ? "Request items for short-term use."
              : requestType === "ALLOCATE"
                ? "Request long-term assets for a laboratory."
                : requestType === "REPAIR"
                  ? "Request repair for selected laboratory assets."
                  : "Request retirement for selected laboratory assets."}
          </DialogDescription>
        </DialogHeader>

        <form className="space-y-4" onSubmit={handleSubmit}>
          <div className="grid gap-4 md:grid-cols-2">
            <label className="space-y-2">
              <span className="text-sm font-medium">Request type</span>
              <Select value={requestType} onValueChange={handleRequestTypeChange}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {requestTypeOptions.map((item) => (
                    <SelectItem key={item.value} value={item.value}>
                      {item.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </label>

            <label className="space-y-2">
              <span className="text-sm font-medium">Target laboratory</span>
              <Select value={targetLaboratoryId} onValueChange={setTargetLaboratoryId}>
                <SelectTrigger>
                  <SelectValue placeholder="Select laboratory" />
                </SelectTrigger>
                <SelectContent>
                  {laboratories.filter((lab) => lab.id).map((lab) => (
                    <SelectItem key={lab.id} value={lab.id as string}>
                      {getLaboratoryLabel(lab)}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </label>
          </div>

          {requestType === "BORROW" ? (
            <>
              <label className="space-y-2">
                <span className="text-sm font-medium">Purpose / requested item</span>
                <Input
                  value={purpose}
                  onChange={(event) => setPurpose(event.target.value)}
                  placeholder="e.g. Mouse for programming lab work"
                />
              </label>

              <div className="grid gap-4 md:grid-cols-3">
                <label className="space-y-2">
                  <span className="text-sm font-medium">Quantity</span>
                  <Input
                    type="number"
                    min="1"
                    value={quantity}
                    onChange={(event) => setQuantity(event.target.value)}
                  />
                </label>
                <label className="space-y-2">
                  <span className="text-sm font-medium">Borrow from</span>
                  <Input
                    type="date"
                    value={borrowFrom}
                    onChange={(event) => setBorrowFrom(event.target.value)}
                  />
                </label>
                <label className="space-y-2">
                  <span className="text-sm font-medium">Borrow to</span>
                  <Input
                    type="date"
                    value={borrowTo}
                    onChange={(event) => setBorrowTo(event.target.value)}
                  />
                </label>
              </div>
            </>
          ) : (
            <>
              <div className="space-y-3 rounded-[8px] border border-border p-3">
                <div className="flex items-center justify-between gap-3">
                  <span className="text-sm font-medium">
                    {assetSectionTitle[requestType]}
                  </span>
                  {isLoadingAssets && (
                    <Loader2Icon className="size-4 animate-spin text-muted-foreground" />
                  )}
                </div>
                {assets.length > 0 && (
                  <div className="relative">
                    <SearchIcon className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
                    <Input
                      value={assetSearch}
                      onChange={(event) => setAssetSearch(event.target.value)}
                      className="pl-9"
                      placeholder={
                        requestType === "ALLOCATE"
                          ? "Search central storage assets"
                          : "Search laboratory assets"
                      }
                    />
                  </div>
                )}
                {assets.length === 0 ? (
                  <p className="text-sm text-muted-foreground">
                    {isLoadingAssets
                      ? "Loading assets"
                      : requestType === "ALLOCATE"
                        ? "No allocatable assets found in central storage."
                        : "No assets found for this laboratory."}
                  </p>
                ) : visibleAssets.length === 0 ? (
                  <p className="rounded-[8px] border border-dashed border-border p-4 text-sm text-muted-foreground">
                    No assets match this search.
                  </p>
                ) : (
                  <div className="max-h-44 space-y-2 overflow-y-auto">
                    {visibleAssets.map((asset) => {
                      if (!asset.id) return null
                      const checkboxId = `request-asset-${asset.id}`

                      return (
                        <div
                          key={asset.id}
                          className="flex items-start justify-between gap-3 rounded-[8px] border border-border p-3 text-sm"
                        >
                          <div className="flex min-w-0 items-start gap-3">
                            <input
                              id={checkboxId}
                              type="checkbox"
                              className="mt-1"
                              checked={selectedAssetIds.includes(asset.id)}
                              onChange={() => toggleAsset(asset.id as string)}
                            />
                            <label htmlFor={checkboxId} className="min-w-0 cursor-pointer">
                              <span className="block truncate font-semibold text-foreground">
                                {asset.name || "Unnamed asset"}
                              </span>
                              <span className="block truncate text-xs text-muted-foreground">
                                {asset.category || "Uncategorized"}
                              </span>
                            </label>
                          </div>
                          {requestType === "ALLOCATE" && (
                            <Button
                              type="button"
                              variant="ghost"
                              size="sm"
                              className="shrink-0"
                              onClick={() => setPreviewAsset(asset)}
                            >
                              Open
                            </Button>
                          )}
                        </div>
                      )
                    })}
                  </div>
                )}
              </div>

              {requestType === "REPAIR" && (
                <label className="space-y-2">
                  <span className="text-sm font-medium">Issue description</span>
                  <Textarea
                    value={issueDescription}
                    onChange={(event) => setIssueDescription(event.target.value)}
                    placeholder="Describe the asset issue or repair needed"
                  />
                </label>
              )}

              {requestType === "RETIRE" && (
                <label className="space-y-2">
                  <span className="text-sm font-medium">Retire reason</span>
                  <Textarea
                    value={retireReason}
                    onChange={(event) => setRetireReason(event.target.value)}
                    placeholder="Explain why these assets should be retired"
                  />
                </label>
              )}

              <label className="space-y-2">
                <span className="text-sm font-medium">Optional note</span>
                <Input
                  value={extraNote}
                  onChange={(event) => setExtraNote(event.target.value)}
                  placeholder={
                    requestType === "ALLOCATE"
                      ? "e.g. Place in Lab 2 for long-term use"
                      : "Optional note for selected assets"
                  }
                />
              </label>
            </>
          )}

          <label className="space-y-2">
            <span className="text-sm font-medium">
              {requestType === "BORROW"
                ? "Borrow reason"
                : requestType === "ALLOCATE"
                  ? "Allocation reason"
                  : requestType === "REPAIR"
                    ? "Repair reason"
                    : "Retirement request reason"}
            </span>
            <Textarea
              value={reason}
              onChange={(event) => setReason(event.target.value)}
              placeholder={
                requestType === "BORROW"
                  ? "Explain why this item is needed"
                  : requestType === "ALLOCATE"
                    ? "Explain why this lab needs these assets"
                    : requestType === "REPAIR"
                      ? "Explain the operational impact"
                      : "Explain why retirement approval is needed"
              }
            />
          </label>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => setOpen(false)}>
              Cancel
            </Button>
            <Button type="submit" disabled={!canSubmit || isSubmitting}>
              {isSubmitting ? "Submitting..." : "Submit"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
        <Dialog open={Boolean(previewAsset)} onOpenChange={(nextOpen) => {
          if (!nextOpen) closePreview()
        }}>
          <DialogContent className="max-h-[min(90vh,760px)] max-w-[560px] overflow-y-auto">
            <DialogHeader>
              <DialogTitle>{previewAsset?.name || "Asset preview"}</DialogTitle>
              <DialogDescription>
                Quick asset information from central storage.
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

    </Dialog>
  )
}
