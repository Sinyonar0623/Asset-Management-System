"use client"

import * as React from "react"
import { Loader2Icon, SendIcon } from "lucide-react"

import { Button } from "@/components/ui/button"
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
  getAssetsByLaboratory,
  getLaboratories,
  type AssetDto,
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

export function CreateRequestDialog({ onCreated }: { onCreated?: () => void }) {
  const { session } = useAuth()
  const [open, setOpen] = React.useState(false)
  const [laboratories, setLaboratories] = React.useState<LaboratoryDto[]>([])
  const [assets, setAssets] = React.useState<AssetDto[]>([])
  const [requestType, setRequestType] = React.useState<RequestType>("BORROW")
  const [targetLaboratoryId, setTargetLaboratoryId] = React.useState("")
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

  const canCreateMaintenance = session?.role !== "student"
  const requestTypeOptions: Array<{ value: RequestType; label: string }> =
    session?.role === "admin"
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

    setRequestType(session?.role === "admin" || !canCreateMaintenance ? "BORROW" : "ALLOCATE")

    getLaboratories()
      .then((items) => {
        setLaboratories(items)
        setTargetLaboratoryId((current) => current || items[0]?.id || "")
      })
      .catch(() => {
        toast({
          title: "Unable to load laboratories",
          description: "The request API is available, but laboratory lookup failed.",
          variant: "destructive",
        })
      })
  }, [canCreateMaintenance, open, session?.role])

  React.useEffect(() => {
    if (!open || !requiresAssets || !targetLaboratoryId) {
      setAssets([])
      setSelectedAssetIds([])
      return
    }

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

  function resetTypeSpecificFields() {
    setAssets([])
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

  return (
    <Dialog open={open} onOpenChange={setOpen}>
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
                {assets.length === 0 ? (
                  <p className="text-sm text-muted-foreground">
                    {isLoadingAssets
                      ? "Loading assets"
                      : requestType === "ALLOCATE"
                        ? "No allocatable assets found in central storage."
                        : "No assets found for this laboratory."}
                  </p>
                ) : (
                  <div className="max-h-44 space-y-2 overflow-y-auto">
                    {assets.map((asset) => {
                      if (!asset.id) return null

                      return (
                        <label
                          key={asset.id}
                          className="flex cursor-pointer items-start gap-3 rounded-[8px] border border-border p-3 text-sm"
                        >
                          <input
                            type="checkbox"
                            className="mt-1"
                            checked={selectedAssetIds.includes(asset.id)}
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
    </Dialog>
  )
}
