"use client"

// API:
// GET /Parameters/ASSET_CATEGORY
// Returns: { response: ParameterDto[] }
// GET /AssetUnit/unassigned
// Returns: { assetUnits: AssetUnitDto[] }
// GET /AssetUnit/{id}/detail
// Returns: { assetUnit: AssetUnitDetailDto }
// POST /AssetUnit
// Body: { assetUnits: AssetUnitDto[] }
// Returns: { id: string[] }
// PUT /AssetUnit/{id}
// Body: { assetUnit: AssetUnitDto }
// Returns: { isSuccess: boolean }
// POST /AssetUnit/{assetUnitId}/Images
// Body: multipart/form-data { files: File[], description?: string }
// Returns: { images: AssetUnitImageDto[] }
// POST /Asset
// Body: { asset: { name, description, category }, units: string[] }
// Returns: { id: string }

import * as React from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import {
  ArrowLeftIcon,
  BoxIcon,
  CheckIcon,
  EyeIcon,
  ImageIcon,
  Loader2Icon,
  PackagePlusIcon,
  PencilIcon,
  PlusIcon,
  SaveIcon,
  SearchIcon,
  UploadIcon,
  XIcon,
} from "lucide-react"

import { ApiNotice } from "@/components/ce-ams/api-notice"
import { PageHeader } from "@/components/ce-ams/page-header"
import { StatusBadge } from "@/components/ce-ams/status-badge"
import { Badge } from "@/components/ui/badge"
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
import { Input } from "@/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Textarea } from "@/components/ui/textarea"
import { toast } from "@/components/ui/sonner"
import {
  createAsset,
  createAssetUnits,
  formatDate,
  getAssetUnitDetail,
  getParametersByGroup,
  getUnassignedAssetUnits,
  shortId,
  updateAssetUnit,
  uploadAssetUnitImages,
  type AssetUnitDetailDto,
  type AssetUnitDto,
  type ParameterDto,
} from "@/lib/ce-ams-api"
import { cn } from "@/lib/utils"

const ASSET_CATEGORY_GROUP = "ASSET_CATEGORY"
const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL?.replace(/\/$/, "")

function resolveAssetImageUrl(imageUrl: string) {
  if (/^(https?:|data:|blob:)/i.test(imageUrl)) {
    return imageUrl
  }

  if (imageUrl.startsWith("/") && apiBaseUrl) {
    return `${apiBaseUrl}${imageUrl}`
  }

  return imageUrl
}

function getUnitIdentity(unit: AssetUnitDto) {
  return unit.id ?? unit.assetTag
}

function isUnitSelected(unit: AssetUnitDto, selectedIds: string[]) {
  return Boolean(unit.id && selectedIds.includes(unit.id))
}

function activeParameters(items: ParameterDto[]) {
  return items.filter((item) => item.active)
}

export default function CreateAssetPage() {
  const router = useRouter()
  const [name, setName] = React.useState("")
  const [description, setDescription] = React.useState("")
  const [category, setCategory] = React.useState("")
  const [categories, setCategories] = React.useState<ParameterDto[]>([])
  const [units, setUnits] = React.useState<AssetUnitDto[]>([])
  const [selectedUnitIds, setSelectedUnitIds] = React.useState<string[]>([])
  const [unitSearch, setUnitSearch] = React.useState("")
  const [unitDialogOpen, setUnitDialogOpen] = React.useState(false)
  const [addUnitDialogOpen, setAddUnitDialogOpen] = React.useState(false)
  const [isLoadingLookups, setLoadingLookups] = React.useState(true)
  const [isSubmitting, setSubmitting] = React.useState(false)
  const [detail, setDetail] = React.useState<AssetUnitDetailDto | null>(null)
  const [detailLoadingId, setDetailLoadingId] = React.useState<string | null>(null)

  React.useEffect(() => {
    Promise.allSettled([
      getParametersByGroup(ASSET_CATEGORY_GROUP),
      getUnassignedAssetUnits(),
    ]).then(([categoryResult, unitResult]) => {
      if (categoryResult.status === "fulfilled") {
        const activeCategories = activeParameters(categoryResult.value)
        setCategories(activeCategories)
        setCategory((current) => current || activeCategories[0]?.value || "")
      } else {
        toast({
          title: "Unable to load asset categories",
          description: `GET /Parameters/${ASSET_CATEGORY_GROUP} did not return category values.`,
          variant: "destructive",
        })
      }

      if (unitResult.status === "fulfilled") {
        setUnits(unitResult.value)
      } else {
        toast({
          title: "Unable to load unassigned units",
          description: "GET /AssetUnit/unassigned is required before creating an asset.",
          variant: "destructive",
        })
      }

      setLoadingLookups(false)
    })
  }, [])

  const selectedUnits = React.useMemo(
    () => units.filter((unit) => unit.id && selectedUnitIds.includes(unit.id)),
    [selectedUnitIds, units]
  )

  const visibleUnits = React.useMemo(() => {
    const keyword = unitSearch.trim().toLowerCase()

    if (!keyword) {
      return units
    }

    return units.filter((unit) => {
      const searchable = [
        unit.name,
        unit.assetTag,
        unit.serialNo,
        unit.brand,
        unit.availabilityStatus,
        unit.operationalStatus,
      ]
        .join(" ")
        .toLowerCase()

      return searchable.includes(keyword)
    })
  }, [unitSearch, units])

  const canSubmit = Boolean(name.trim() && category && selectedUnitIds.length > 0)

  function toggleUnit(unit: AssetUnitDto) {
    if (!unit.id) return

    setSelectedUnitIds((current) => {
      if (current.includes(unit.id as string)) {
        return current.filter((id) => id !== unit.id)
      }

      return [...current, unit.id as string]
    })
  }

  function removeSelectedUnit(unitId: string) {
    setSelectedUnitIds((current) => current.filter((id) => id !== unitId))
  }

  async function refreshUnassignedUnits() {
    const nextUnits = await getUnassignedAssetUnits()
    setUnits(nextUnits)
    return nextUnits
  }

  async function handleAssetUnitCreated(createdIds: string[]) {
    const nextUnits = await refreshUnassignedUnits()

    setSelectedUnitIds((current) => {
      const nextIds = new Set(current)
      createdIds.forEach((id) => nextIds.add(id))
      return Array.from(nextIds)
    })

    const firstCreatedUnit = nextUnits.find((unit) => unit.id === createdIds[0])
    if (firstCreatedUnit) {
      await openUnitDetail(firstCreatedUnit)
    }
  }

  async function openUnitDetail(unit: AssetUnitDto) {
    if (!unit.id) return

    setDetailLoadingId(unit.id)
    try {
      const nextDetail = await getAssetUnitDetail(unit.id)
      setDetail(nextDetail)
    } catch {
      toast({
        title: "Unable to load unit detail",
        description: "GET /AssetUnit/{id}/detail is required for images and usage history.",
        variant: "destructive",
      })
    } finally {
      setDetailLoadingId(null)
    }
  }

  async function handleUnitUpdated(nextUnit: AssetUnitDto) {
    if (!nextUnit.id) return

    await updateAssetUnit(nextUnit.id, nextUnit)

    const nextDetail = await getAssetUnitDetail(nextUnit.id)
    setDetail(nextDetail)
    setUnits((current) =>
      current.map((unit) => (unit.id === nextUnit.id ? nextDetail.unit : unit))
    )
  }

  async function handleUnitImagesUploaded(
    assetUnitId: string,
    files: File[],
    imageDescription: string
  ) {
    await uploadAssetUnitImages(assetUnitId, files, imageDescription)

    const nextDetail = await getAssetUnitDetail(assetUnitId)
    setDetail(nextDetail)
    setUnits((current) =>
      current.map((unit) => (unit.id === assetUnitId ? nextDetail.unit : unit))
    )
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!canSubmit) {
      return
    }

    setSubmitting(true)
    try {
      const result = await createAsset({
        asset: {
          name: name.trim(),
          description: description.trim(),
          category,
        },
        units: selectedUnitIds,
      })

      toast("Asset created")
      router.push(result.id ? `/assetManagement/assets/${result.id}` : "/assetManagement/assets")
    } catch {
      toast({
        title: "Asset could not be created",
        description: "Please confirm the backend is running and the selected units are still unassigned.",
        variant: "destructive",
      })
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="space-y-6">
      <PageHeader
        title="Create Asset"
        subtitle="Create the asset record by assigning existing physical units prepared in Asset Unit first."
        actions={
          <Button asChild variant="outline">
            <Link href="/assetManagement/assets">
              <ArrowLeftIcon className="size-4" />
              Back to Assets
            </Link>
          </Button>
        }
      />

      <ApiNotice title="Asset creation flow">
        Create Asset Unit records first via `POST /AssetUnit`, then select those unassigned units here.
        This page only submits real backend data to `POST /Asset`.
      </ApiNotice>

      <form className="grid grid-cols-[minmax(0,1fr)_380px] gap-6" onSubmit={handleSubmit}>
        <Card>
          <CardHeader>
            <CardTitle>Asset Information</CardTitle>
          </CardHeader>
          <CardContent className="space-y-5">
            <div>
              <label className="space-y-2">
                <span className="text-sm font-semibold text-foreground">Asset name</span>
                <Input
                  value={name}
                  onChange={(event) => setName(event.target.value)}
                  placeholder="e.g. Dell OptiPlex Lab Workstation"
                />
              </label>
            </div>

            <div>
              <label className="space-y-2">
              <span className="text-sm font-semibold text-foreground">
                Category
              </span>
                <Select
                  value={category}
                  onValueChange={setCategory}
                  disabled={isLoadingLookups || categories.length === 0}
                >
                  <SelectTrigger>
                    <SelectValue placeholder={isLoadingLookups ? "Loading categories" : "Select category"} />
                  </SelectTrigger>
                  <SelectContent>
                  {categories.map((item) => (
                    <SelectItem key={`${item.group}-${item.value}`} value={item.value}>
                      {item.description || item.value}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {categories.length === 0 && !isLoadingLookups && (
                <p className="text-xs text-destructive">
                  No active {ASSET_CATEGORY_GROUP} parameters were returned by the backend.
                </p>
              )}
              </label>
            </div>

            <div>
              <label className="space-y-2">
                <span className="text-sm font-semibold text-foreground">Description</span>
                <Textarea
                  value={description}
                  onChange={(event) => setDescription(event.target.value)}
                  placeholder="Add a clean internal description for this asset group."
                />
              </label>
            </div>

          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Assigned Units</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="rounded-[18px] border border-dashed border-border bg-muted/35 p-4">
              <div className="flex items-start justify-between gap-4">
                <div>
                  <p className="text-3xl font-semibold">{selectedUnitIds.length}</p>
                  <p className="mt-1 text-sm text-muted-foreground">
                    Selected physical units
                  </p>
                </div>
                <div className="flex size-12 items-center justify-center rounded-[16px] bg-blue-600/10 text-blue-700">
                  <BoxIcon className="size-5" />
                </div>
              </div>
            </div>

            <Button
              type="button"
              className="w-full"
              onClick={() => setUnitDialogOpen(true)}
              disabled={isLoadingLookups}
            >
              <PlusIcon className="size-4" />
              Assign Existing Units
            </Button>

            {selectedUnits.length === 0 ? (
              <div className="rounded-[16px] border border-border p-4 text-sm text-muted-foreground">
                Select at least one unassigned unit before creating the asset.
              </div>
            ) : (
              <div className="space-y-3">
                {selectedUnits.map((unit) => (
                  <div
                    key={getUnitIdentity(unit)}
                    className="flex items-center justify-between gap-3 rounded-[16px] border border-border bg-card p-3"
                  >
                    <div className="min-w-0">
                      <p className="truncate text-sm font-semibold text-foreground">{unit.name}</p>
                      <p className="truncate text-xs text-muted-foreground">
                        {unit.assetTag} · {unit.serialNo || "No serial"}
                      </p>
                    </div>
                    {unit.id && (
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon-sm"
                        onClick={() => removeSelectedUnit(unit.id as string)}
                      >
                        <XIcon className="size-4" />
                        <span className="sr-only">Remove unit</span>
                      </Button>
                    )}
                  </div>
                ))}
              </div>
            )}

            <Button type="submit" size="lg" className="w-full" disabled={!canSubmit || isSubmitting}>
              {isSubmitting ? (
                <Loader2Icon className="size-4 animate-spin" />
              ) : (
                <PackagePlusIcon className="size-4" />
              )}
              Create Asset
            </Button>
          </CardContent>
        </Card>
      </form>

      <Dialog open={unitDialogOpen} onOpenChange={setUnitDialogOpen}>
        <DialogContent className="w-[min(1120px,calc(100vw-64px))] max-w-none">
          <DialogHeader>
            <DialogTitle>Assign Existing Unit IDs</DialogTitle>
            <DialogDescription>
              Choose unassigned Asset Units, then inspect images and usage history before attaching them.
            </DialogDescription>
          </DialogHeader>

          <div className="grid min-h-[560px] grid-cols-[minmax(0,1fr)_420px] gap-5">
            <div className="flex min-h-0 flex-col gap-4">
              <div className="flex items-center gap-3">
                <div className="relative min-w-0 flex-1">
                  <SearchIcon className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
                  <Input
                    value={unitSearch}
                    onChange={(event) => setUnitSearch(event.target.value)}
                    className="pl-9"
                    placeholder="Search by tag, serial, brand, or status"
                  />
                </div>
                <Button type="button" onClick={() => setAddUnitDialogOpen(true)}>
                  <PlusIcon className="size-4" />
                  Add Asset Unit
                </Button>
              </div>

              <div className="min-h-0 flex-1 space-y-3 overflow-y-auto rounded-[18px] border border-border bg-muted/20 p-3">
                {isLoadingLookups ? (
                  <div className="flex h-full items-center justify-center gap-2 text-sm text-muted-foreground">
                    <Loader2Icon className="size-4 animate-spin" />
                    Loading unassigned units
                  </div>
                ) : visibleUnits.length === 0 ? (
                  <div className="flex h-full flex-col items-center justify-center gap-2 text-center">
                    <p className="font-semibold text-foreground">No unassigned units found</p>
                    <p className="max-w-sm text-sm text-muted-foreground">
                      Create Asset Unit records first. Once they are unassigned, they will appear here.
                    </p>
                    <Button
                      type="button"
                      className="mt-2"
                      onClick={() => setAddUnitDialogOpen(true)}
                    >
                      <PlusIcon className="size-4" />
                      Add Asset Unit
                    </Button>
                  </div>
                ) : (
                  visibleUnits.map((unit) => {
                    const selected = isUnitSelected(unit, selectedUnitIds)
                    const loadingDetail = Boolean(unit.id && detailLoadingId === unit.id)

                    return (
                      <button
                        key={getUnitIdentity(unit)}
                        type="button"
                        onClick={() => openUnitDetail(unit)}
                        className={cn(
                          "grid w-full grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-4 rounded-[16px] border bg-card p-4 text-left transition hover:border-blue-300 hover:bg-blue-50/70",
                          selected ? "border-blue-500 ring-2 ring-blue-500/15" : "border-border"
                        )}
                      >
                        <span
                          className={cn(
                            "flex size-6 items-center justify-center rounded-full border",
                            selected
                              ? "border-blue-600 bg-blue-600 text-white"
                              : "border-border bg-card text-transparent"
                          )}
                          onClick={(event) => {
                            event.stopPropagation()
                            toggleUnit(unit)
                          }}
                        >
                          <CheckIcon className="size-4" />
                        </span>

                        <span className="min-w-0 space-y-2">
                          <span className="flex items-center gap-2">
                            <span className="truncate text-sm font-semibold text-foreground">
                              {unit.name || "Unnamed unit"}
                            </span>
                            <Badge variant="outline" className="font-mono">
                              {shortId(unit.id)}
                            </Badge>
                          </span>
                          <span className="flex flex-wrap items-center gap-2 text-xs text-muted-foreground">
                            <span>{unit.assetTag || "No asset tag"}</span>
                            <span>{unit.serialNo || "No serial"}</span>
                            <span>{unit.brand || "No brand"}</span>
                          </span>
                          <span className="flex items-center gap-2">
                            <StatusBadge status={unit.availabilityStatus} />
                            <StatusBadge status={unit.operationalStatus} />
                          </span>
                        </span>

                        <span className="flex items-center gap-2">
                          {loadingDetail ? (
                            <Loader2Icon className="size-4 animate-spin text-muted-foreground" />
                          ) : (
                            <EyeIcon className="size-4 text-muted-foreground" />
                          )}
                        </span>
                      </button>
                    )
                  })
                )}
              </div>
            </div>

            <UnitDetailPanel
              detail={detail}
              isLoading={Boolean(detailLoadingId)}
              onUpdateUnit={handleUnitUpdated}
              onUploadImages={handleUnitImagesUploaded}
            />
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => setUnitDialogOpen(false)}>
              Cancel
            </Button>
            <Button type="button" onClick={() => setUnitDialogOpen(false)}>
              Use {selectedUnitIds.length} Selected Unit{selectedUnitIds.length === 1 ? "" : "s"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <AddAssetUnitDialog
        open={addUnitDialogOpen}
        onOpenChange={setAddUnitDialogOpen}
        onCreated={handleAssetUnitCreated}
      />
    </div>
  )
}

function AddAssetUnitDialog({
  open,
  onOpenChange,
  onCreated,
}: {
  open: boolean
  onOpenChange: (open: boolean) => void
  onCreated: (createdIds: string[]) => Promise<void> | void
}) {
  const [assetTag, setAssetTag] = React.useState("")
  const [serialNo, setSerialNo] = React.useState("")
  const [unitName, setUnitName] = React.useState("")
  const [brand, setBrand] = React.useState("")
  const [remark, setRemark] = React.useState("")
  const [imageFiles, setImageFiles] = React.useState<File[]>([])
  const [imageDescription, setImageDescription] = React.useState("")
  const [fileInputKey, setFileInputKey] = React.useState(0)
  const [isSubmitting, setSubmitting] = React.useState(false)

  const canSubmit = Boolean(assetTag.trim() && serialNo.trim() && unitName.trim() && brand.trim())

  function resetForm() {
    setAssetTag("")
    setSerialNo("")
    setUnitName("")
    setBrand("")
    setRemark("")
    setImageFiles([])
    setImageDescription("")
    setFileInputKey((current) => current + 1)
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!canSubmit) return

    setSubmitting(true)
    try {
      const createdIds = await createAssetUnits({
        assetUnits: [
          {
            assetId: null,
            assetTag: assetTag.trim(),
            serialNo: serialNo.trim(),
            name: unitName.trim(),
            brand: brand.trim(),
            availabilityStatus: "",
            operationalStatus: "",
            remark: remark.trim(),
            responsibleUserId: null,
          },
        ],
      })

      let imagesUploaded = imageFiles.length === 0
      if (createdIds[0] && imageFiles.length > 0) {
        try {
          await uploadAssetUnitImages(createdIds[0], imageFiles, imageDescription)
          imagesUploaded = true
        } catch {
          toast({
            title: "Asset Unit created, but images were not uploaded",
            description: "Only PNG and JPEG files are accepted by the backend.",
            variant: "destructive",
          })
        }
      }

      await onCreated(createdIds)
      if (imageFiles.length === 0) {
        toast("Asset Unit created")
      } else if (imagesUploaded) {
        toast("Asset Unit created with images")
      }
      resetForm()
      onOpenChange(false)
    } catch {
      toast({
        title: "Asset Unit could not be created",
        description: "Please confirm asset tag and serial number are unique, then try again.",
        variant: "destructive",
      })
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="w-[min(760px,calc(100vw-64px))] max-w-none">
        <DialogHeader>
          <DialogTitle>Add Asset Unit</DialogTitle>
          <DialogDescription>
            Create one physical unit first, then it will be selected automatically for this asset.
          </DialogDescription>
        </DialogHeader>

        <form className="space-y-5" onSubmit={handleSubmit}>
          <div className="grid grid-cols-2 gap-4">
            <label className="space-y-2">
              <span className="text-sm font-semibold text-foreground">Asset tag</span>
              <Input
                value={assetTag}
                onChange={(event) => setAssetTag(event.target.value)}
                placeholder="e.g. CPE-PC-001"
              />
            </label>

            <label className="space-y-2">
              <span className="text-sm font-semibold text-foreground">Serial number</span>
              <Input
                value={serialNo}
                onChange={(event) => setSerialNo(event.target.value)}
                placeholder="e.g. SN-9X2A-001"
              />
            </label>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <label className="space-y-2">
              <span className="text-sm font-semibold text-foreground">Unit name</span>
              <Input
                value={unitName}
                onChange={(event) => setUnitName(event.target.value)}
                placeholder="e.g. Lab Workstation 01"
              />
            </label>

            <label className="space-y-2">
              <span className="text-sm font-semibold text-foreground">Brand</span>
              <Input
                value={brand}
                onChange={(event) => setBrand(event.target.value)}
                placeholder="e.g. Dell"
              />
            </label>
          </div>

          <label className="space-y-2">
            <span className="text-sm font-semibold text-foreground">Remark</span>
            <Textarea
              value={remark}
              onChange={(event) => setRemark(event.target.value)}
              placeholder="Optional internal note for this unit."
            />
          </label>

          <div className="space-y-3 rounded-[18px] border border-dashed border-border bg-muted/25 p-4">
            <div>
              <p className="text-sm font-semibold text-foreground">Unit images</p>
              <p className="mt-1 text-xs text-muted-foreground">
                Optional. Upload PNG or JPEG files; they will be attached after the unit is created.
              </p>
            </div>

            <Input
              key={fileInputKey}
              type="file"
              accept="image/png,image/jpeg,image/jpg,.jpg,.jpeg"
              multiple
              onChange={(event) => {
                setImageFiles(Array.from(event.target.files ?? []))
              }}
            />

            {imageFiles.length > 0 && (
              <div className="space-y-2">
                <div className="flex flex-wrap gap-2">
                  {imageFiles.map((file) => (
                    <Badge key={`${file.name}-${file.size}`} variant="secondary">
                      {file.name}
                    </Badge>
                  ))}
                </div>
                <label className="space-y-2">
                  <span className="text-sm font-semibold text-foreground">
                    Image description
                  </span>
                  <Input
                    value={imageDescription}
                    onChange={(event) => setImageDescription(event.target.value)}
                    placeholder="Optional description shared by uploaded images"
                  />
                </label>
              </div>
            )}
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
              disabled={isSubmitting}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={!canSubmit || isSubmitting}>
              {isSubmitting ? (
                <Loader2Icon className="size-4 animate-spin" />
              ) : (
                <PlusIcon className="size-4" />
              )}
              Add Asset Unit
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}

function UnitDetailPanel({
  detail,
  isLoading,
  onUpdateUnit,
  onUploadImages,
}: {
  detail: AssetUnitDetailDto | null
  isLoading: boolean
  onUpdateUnit: (unit: AssetUnitDto) => Promise<void>
  onUploadImages: (
    assetUnitId: string,
    files: File[],
    imageDescription: string
  ) => Promise<void>
}) {
  const currentUnit = detail?.unit ?? null
  const [isEditing, setEditing] = React.useState(false)
  const [draftSerialNo, setDraftSerialNo] = React.useState("")
  const [draftBrand, setDraftBrand] = React.useState("")
  const [isSavingUnit, setSavingUnit] = React.useState(false)
  const [uploadFiles, setUploadFiles] = React.useState<File[]>([])
  const [uploadDescription, setUploadDescription] = React.useState("")
  const [fileInputKey, setFileInputKey] = React.useState(0)
  const [isUploading, setUploading] = React.useState(false)

  React.useEffect(() => {
    setEditing(false)
    setDraftSerialNo(currentUnit?.serialNo ?? "")
    setDraftBrand(currentUnit?.brand ?? "")
    setUploadFiles([])
    setUploadDescription("")
    setFileInputKey((current) => current + 1)
  }, [currentUnit?.id, currentUnit?.serialNo, currentUnit?.brand])

  const unitChanged = Boolean(
    currentUnit &&
      (draftSerialNo.trim() !== currentUnit.serialNo ||
        draftBrand.trim() !== currentUnit.brand)
  )
  const canSaveUnit = Boolean(
    currentUnit?.id && draftSerialNo.trim() && draftBrand.trim() && unitChanged
  )
  const canUploadImages = Boolean(currentUnit?.id && uploadFiles.length > 0)

  function cancelEdit() {
    setDraftSerialNo(currentUnit?.serialNo ?? "")
    setDraftBrand(currentUnit?.brand ?? "")
    setEditing(false)
  }

  async function handleSaveUnit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!currentUnit?.id || !canSaveUnit) return

    setSavingUnit(true)
    try {
      await onUpdateUnit({
        ...currentUnit,
        serialNo: draftSerialNo.trim(),
        brand: draftBrand.trim(),
      })
      setEditing(false)
      toast("Asset Unit updated")
    } catch {
      toast({
        title: "Asset Unit could not be updated",
        description: "Please confirm the serial number is unique, then try again.",
        variant: "destructive",
      })
    } finally {
      setSavingUnit(false)
    }
  }

  async function handleUploadImages() {
    if (!currentUnit?.id || !canUploadImages) return

    setUploading(true)
    try {
      await onUploadImages(currentUnit.id, uploadFiles, uploadDescription)
      setUploadFiles([])
      setUploadDescription("")
      setFileInputKey((current) => current + 1)
      toast("Asset Unit images uploaded")
    } catch {
      toast({
        title: "Images could not be uploaded",
        description: "Only PNG and JPEG files are accepted by the backend.",
        variant: "destructive",
      })
    } finally {
      setUploading(false)
    }
  }

  if (isLoading) {
    return (
      <aside className="flex min-h-0 flex-col items-center justify-center rounded-[18px] border border-border bg-card p-5 text-center">
        <Loader2Icon className="mb-3 size-6 animate-spin text-muted-foreground" />
        <p className="font-semibold text-foreground">Loading unit detail</p>
        <p className="mt-1 text-sm text-muted-foreground">Fetching images and usage history.</p>
      </aside>
    )
  }

  if (!detail) {
    return (
      <aside className="flex min-h-0 flex-col items-center justify-center rounded-[18px] border border-dashed border-border bg-card p-5 text-center">
        <EyeIcon className="mb-3 size-6 text-muted-foreground" />
        <p className="font-semibold text-foreground">Click a unit to inspect it</p>
        <p className="mt-1 max-w-xs text-sm text-muted-foreground">
          The detail panel shows photos, status, notes, and usage history from the backend.
        </p>
      </aside>
    )
  }

  return (
    <aside className="min-h-0 overflow-y-auto rounded-[18px] border border-border bg-card">
      <div className="border-b border-border p-5">
        <div className="flex items-start justify-between gap-4">
          <div className="min-w-0">
            <p className="truncate text-lg font-semibold text-foreground">
              {detail.unit.name || "Unnamed unit"}
            </p>
            <p className="mt-1 font-mono text-xs text-muted-foreground">
              {detail.unit.assetTag || shortId(detail.unit.id)}
            </p>
          </div>
          <div className="flex shrink-0 items-center gap-2">
            <StatusBadge status={detail.unit.availabilityStatus} />
            {!isEditing && (
              <Button
                type="button"
                variant="outline"
                size="icon-sm"
                onClick={() => setEditing(true)}
                title="Edit serial and brand"
              >
                <PencilIcon className="size-4" />
                <span className="sr-only">Edit serial and brand</span>
              </Button>
            )}
          </div>
        </div>
        {isEditing ? (
          <form className="mt-4 space-y-3" onSubmit={handleSaveUnit}>
            <div className="grid grid-cols-2 gap-3">
              <label className="space-y-2">
                <span className="text-xs font-semibold text-muted-foreground">Serial</span>
                <Input
                  value={draftSerialNo}
                  onChange={(event) => setDraftSerialNo(event.target.value)}
                  placeholder="Serial number"
                  disabled={isSavingUnit}
                />
              </label>
              <label className="space-y-2">
                <span className="text-xs font-semibold text-muted-foreground">Brand</span>
                <Input
                  value={draftBrand}
                  onChange={(event) => setDraftBrand(event.target.value)}
                  placeholder="Brand"
                  disabled={isSavingUnit}
                />
              </label>
            </div>
            <div className="flex justify-end gap-2">
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={cancelEdit}
                disabled={isSavingUnit}
              >
                Cancel
              </Button>
              <Button type="submit" size="sm" disabled={!canSaveUnit || isSavingUnit}>
                {isSavingUnit ? (
                  <Loader2Icon className="size-4 animate-spin" />
                ) : (
                  <SaveIcon className="size-4" />
                )}
                Save
              </Button>
            </div>
          </form>
        ) : (
          <div className="mt-4 grid grid-cols-2 gap-3 text-sm">
            <InfoTile label="Serial" value={detail.unit.serialNo || "No serial"} />
            <InfoTile label="Brand" value={detail.unit.brand || "No brand"} />
            <InfoTile label="Operational" value={detail.unit.operationalStatus || "Unknown"} />
            <InfoTile label="Responsible" value={shortId(detail.unit.responsibleUserId)} />
          </div>
        )}
        {detail.unit.remark && (
          <div className="mt-3 rounded-[14px] border border-border bg-muted/30 p-3 text-sm text-muted-foreground">
            {detail.unit.remark}
          </div>
        )}
      </div>

      <div className="space-y-5 p-5">
        <section className="space-y-3">
          <div className="flex items-center justify-between gap-3">
            <h3 className="text-sm font-semibold text-foreground">Images</h3>
            <Badge variant="secondary">{detail.images.length}</Badge>
          </div>
          <div className="space-y-3 rounded-[16px] border border-dashed border-border bg-muted/25 p-3">
            <div className="flex items-center justify-between gap-3">
              <div>
                <p className="text-sm font-semibold text-foreground">Upload images</p>
                <p className="mt-1 text-xs text-muted-foreground">PNG or JPEG files</p>
              </div>
              <Button
                type="button"
                size="sm"
                onClick={handleUploadImages}
                disabled={!canUploadImages || isUploading}
              >
                {isUploading ? (
                  <Loader2Icon className="size-4 animate-spin" />
                ) : (
                  <UploadIcon className="size-4" />
                )}
                Upload
              </Button>
            </div>
            <Input
              key={fileInputKey}
              type="file"
              accept="image/png,image/jpeg,image/jpg,.jpg,.jpeg"
              multiple
              disabled={isUploading}
              onChange={(event) => setUploadFiles(Array.from(event.target.files ?? []))}
            />
            {uploadFiles.length > 0 && (
              <div className="space-y-2">
                <div className="flex flex-wrap gap-2">
                  {uploadFiles.map((file) => (
                    <Badge key={`${file.name}-${file.size}`} variant="secondary">
                      {file.name}
                    </Badge>
                  ))}
                </div>
                <Input
                  value={uploadDescription}
                  onChange={(event) => setUploadDescription(event.target.value)}
                  placeholder="Optional description shared by uploaded images"
                  disabled={isUploading}
                />
              </div>
            )}
          </div>
          {detail.images.length === 0 ? (
            <EmptyPanel
              icon={<ImageIcon className="size-5" />}
              title="No images yet"
              description="Images uploaded to the Asset Unit API will appear here."
            />
          ) : (
            <div className="grid grid-cols-2 gap-3">
              {detail.images.map((image) => (
                <div
                  key={image.id}
                  className="min-h-32 overflow-hidden rounded-[16px] border border-border bg-muted"
                >
                  <div
                    className="h-28 bg-cover bg-center"
                    style={{ backgroundImage: `url("${resolveAssetImageUrl(image.imageUrl)}")` }}
                  />
                  <div className="p-3">
                    <p className="truncate text-xs font-semibold text-foreground">
                      {image.description || image.fileName || "Unit image"}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          )}
        </section>

        <section className="space-y-3">
          <div className="flex items-center justify-between gap-3">
            <h3 className="text-sm font-semibold text-foreground">Usage History</h3>
            <Badge variant="secondary">{detail.histories.length}</Badge>
          </div>
          {detail.histories.length === 0 ? (
            <EmptyPanel
              icon={<BoxIcon className="size-5" />}
              title="No usage history"
              description="History entries will appear after requests, transfers, or status updates."
            />
          ) : (
            <div className="space-y-3">
              {detail.histories.map((history, index) => (
                <div
                  key={`${history.assetUnitId}-${history.performedAt}-${index}`}
                  className="rounded-[16px] border border-border p-3"
                >
                  <div className="flex items-start justify-between gap-3">
                    <div>
                      <p className="text-sm font-semibold text-foreground">{history.actionType}</p>
                      <p className="mt-1 text-xs text-muted-foreground">
                        {formatDate(history.performedAt)} by {shortId(history.performedBy)}
                      </p>
                    </div>
                    {history.referenceNo && (
                      <Badge variant="outline" className="font-mono">
                        {history.referenceNo}
                      </Badge>
                    )}
                  </div>
                  {history.remark && (
                    <p className="mt-2 text-sm text-muted-foreground">{history.remark}</p>
                  )}
                </div>
              ))}
            </div>
          )}
        </section>
      </div>
    </aside>
  )
}

function InfoTile({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-[14px] border border-border bg-muted/30 p-3">
      <p className="text-xs text-muted-foreground">{label}</p>
      <p className="mt-1 truncate text-sm font-semibold text-foreground">{value}</p>
    </div>
  )
}

function EmptyPanel({
  icon,
  title,
  description,
}: {
  icon: React.ReactNode
  title: string
  description: string
}) {
  return (
    <div className="flex items-start gap-3 rounded-[16px] border border-dashed border-border bg-muted/30 p-4">
      <div className="mt-0.5 text-muted-foreground">{icon}</div>
      <div>
        <p className="text-sm font-semibold text-foreground">{title}</p>
        <p className="mt-1 text-sm text-muted-foreground">{description}</p>
      </div>
    </div>
  )
}
