"use client"

import * as React from "react"
import Link from "next/link"
import { useParams } from "next/navigation"
import {
  ArrowLeftIcon,
  BoxIcon,
  CalendarClockIcon,
  FileImageIcon,
  HashIcon,
  Loader2Icon,
  UploadIcon,
  UserIcon,
} from "lucide-react"

import { DataTableShell, TableEmptyState } from "@/components/ce-ams/data-table-shell"
import { PageHeader } from "@/components/ce-ams/page-header"
import { StatusBadge } from "@/components/ce-ams/status-badge"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { toast } from "@/components/ui/sonner"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { useAuth } from "@/context/AuthContext"
import {
  getAssetById,
  getAssetUnitDetail,
  getAssetUnitImages,
  getUserLookup,
  shortId,
  uploadAssetUnitImages,
  type AssetDto,
  type AssetHistoryDto,
  type AssetUnitImageDto,
  type AssetUnitDetailDto,
} from "@/lib/ce-ams-api"

const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL?.replace(/\/$/, "")

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

function formatDateTime(value?: string | null) {
  if (!value) return "-"
  return new Intl.DateTimeFormat("en", {
    year: "numeric",
    month: "short",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  }).format(new Date(value))
}

function renderTransition(from?: string | null, to?: string | null) {
  const safeFrom = from && from !== "-" ? from : null
  const safeTo = to && to !== "-" ? to : null

  if (!safeFrom && !safeTo) return "No change"
  return `${safeFrom || "-"} -> ${safeTo || "-"}`
}

function getReference(history: AssetHistoryDto) {
  return history.referenceNo || shortId(history.requestId)
}

function collectUserIds(detail: AssetUnitDetailDto | null) {
  const ids = new Set<string>()
  if (detail?.unit.responsibleUserId) ids.add(detail.unit.responsibleUserId)

  detail?.histories.forEach((history) => {
    if (history.fromResponsibleUserId) ids.add(history.fromResponsibleUserId)
    if (history.toResponsibleUserId) ids.add(history.toResponsibleUserId)
    if (history.performedBy) ids.add(history.performedBy)
    if (history.approvedBy) ids.add(history.approvedBy)
  })

  return Array.from(ids)
}

export default function AssetUnitDetailPage() {
  const params = useParams<{ id: string; unitId: string }>()
  const { session } = useAuth()
  const [asset, setAsset] = React.useState<AssetDto | null>(null)
  const [detail, setDetail] = React.useState<AssetUnitDetailDto | null>(null)
  const [userNames, setUserNames] = React.useState<Record<string, string>>({})
  const [isLoading, setLoading] = React.useState(true)
  const [selectedImage, setSelectedImage] = React.useState<AssetUnitImageDto | null>(null)
  const [uploadFiles, setUploadFiles] = React.useState<File[]>([])
  const [uploadDescription, setUploadDescription] = React.useState("")
  const [isUploading, setUploading] = React.useState(false)
  const [fileInputKey, setFileInputKey] = React.useState(0)
  const isAdmin = session?.role === "admin"

  const loadDetail = React.useCallback(() => {
    if (!params.id || !params.unitId) return

    setLoading(true)
    Promise.allSettled([
      getAssetById(params.id),
      getAssetUnitDetail(params.unitId),
      getAssetUnitImages(params.unitId),
    ]).then(
      ([assetResult, detailResult, imagesResult]) => {
        if (assetResult.status === "fulfilled") setAsset(assetResult.value)
        if (detailResult.status === "fulfilled") {
          const nextDetail = detailResult.value
          const images = imagesResult.status === "fulfilled" ? imagesResult.value : nextDetail.images
          const detailWithImages = { ...nextDetail, images }
          setDetail(detailWithImages)

          getUserLookup(collectUserIds(detailWithImages))
            .then((users) => {
              setUserNames(
                Object.fromEntries(
                  users.map((user) => [user.userId.toLowerCase(), user.username])
                )
              )
            })
            .catch(() => {
              setUserNames({})
            })
        }
        setLoading(false)
      }
    )
  }, [params.id, params.unitId])

  React.useEffect(() => {
    loadDetail()
  }, [loadDetail])

  const unit = detail?.unit ?? null
  const belongsToAsset =
    !unit?.assetId || unit.assetId.toLowerCase() === params.id.toLowerCase()
  const canUploadImages = Boolean(isAdmin && params.unitId && uploadFiles.length > 0)

  function getUserName(userId?: string | null) {
    if (!userId) return "-"
    return userNames[userId.toLowerCase()] ?? shortId(userId)
  }

  async function handleUploadImages() {
    if (!canUploadImages) return

    setUploading(true)
    try {
      await uploadAssetUnitImages(params.unitId, uploadFiles, uploadDescription)
      setUploadFiles([])
      setUploadDescription("")
      setFileInputKey((current) => current + 1)
      toast("Asset Unit images uploaded")
      loadDetail()
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

  return (
    <div className="space-y-6">
      <PageHeader
        title={unit?.name || "Asset Unit Detail"}
        subtitle={
          unit
            ? `${unit.assetTag || shortId(unit.id)} / ${asset?.name || "Asset"}`
            : "Loading asset unit information"
        }
        actions={
          <Button asChild variant="outline">
            <Link href={`/assetManagement/assets/${params.id}`}>
              <ArrowLeftIcon className="size-4" />
              Back to Asset
            </Link>
          </Button>
        }
      />

      {!belongsToAsset ? (
        <Card className="border-destructive/30 bg-destructive/5">
          <CardContent className="py-4 text-sm text-destructive">
            This unit is not currently attached to the asset in the URL.
          </CardContent>
        </Card>
      ) : null}

      <div className="grid gap-6 lg:grid-cols-[1fr_0.9fr]">
        <Card>
          <CardHeader>
            <CardTitle>Unit Summary</CardTitle>
          </CardHeader>
          <CardContent className="space-y-5">
            <div className="flex flex-wrap items-center gap-3">
              <StatusBadge status={unit?.availabilityStatus || "UNKNOWN"} />
              <Badge variant="outline">{unit?.operationalStatus || "Unknown"}</Badge>
              <Badge variant="secondary">{detail?.images.length ?? 0} images</Badge>
              <Badge variant="secondary">{detail?.histories.length ?? 0} history events</Badge>
            </div>

            <div className="grid gap-3 sm:grid-cols-2">
              <InfoTile
                icon={<HashIcon className="size-4" />}
                label="Asset Tag"
                value={unit?.assetTag || "-"}
              />
              <InfoTile
                icon={<BoxIcon className="size-4" />}
                label="Serial"
                value={unit?.serialNo || "-"}
              />
              <InfoTile
                icon={<BoxIcon className="size-4" />}
                label="Brand"
                value={unit?.brand || "-"}
              />
              <InfoTile
                icon={<UserIcon className="size-4" />}
                label="Responsible User"
                value={getUserName(unit?.responsibleUserId)}
              />
            </div>

            <div className="rounded-[14px] border border-border bg-muted/30 p-4">
              <p className="text-sm font-semibold text-foreground">Remark</p>
              <p className="mt-2 text-sm leading-6 text-muted-foreground">
                {unit?.remark || "No remark"}
              </p>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Parent Asset</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div>
              <p className="text-lg font-semibold">{asset?.name || "Loading asset"}</p>
              <p className="mt-1 text-sm text-muted-foreground">
                {asset?.description || "No description"}
              </p>
            </div>
            <div className="flex flex-wrap gap-2">
              <Badge variant="outline">{asset?.category || "Unassigned"}</Badge>
              <Badge variant="secondary">{shortId(asset?.id)}</Badge>
            </div>
            <Button asChild variant="outline" size="sm">
              <Link href={`/assetManagement/assets/${params.id}`}>Open Asset Detail</Link>
            </Button>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Images</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          {isAdmin ? (
            <div className="space-y-3 rounded-[16px] border border-dashed border-border bg-muted/25 p-4">
              <div className="flex flex-wrap items-center justify-between gap-3">
                <div>
                  <p className="text-sm font-semibold text-foreground">Upload images</p>
                  <p className="mt-1 text-xs text-muted-foreground">Admin only / PNG or JPEG</p>
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
              {uploadFiles.length > 0 ? (
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
              ) : null}
            </div>
          ) : null}

          {isLoading ? (
            <TableEmptyState
              title="Loading images"
              description="Fetching uploaded images from the Asset Unit API."
            />
          ) : !detail || detail.images.length === 0 ? (
            <TableEmptyState
              title="No images yet"
              description="Images uploaded for this Asset Unit will appear here."
            />
          ) : (
            <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
              {detail.images.map((image) => (
                <button
                  type="button"
                  key={image.id}
                  onClick={() => setSelectedImage(image)}
                  className="group overflow-hidden rounded-[16px] border border-border bg-card transition-colors hover:border-primary/50"
                >
                  <div
                    className="h-52 bg-muted bg-cover bg-center"
                    style={{
                      backgroundImage: `url("${resolveAssetImageUrl(image.imageUrl)}")`,
                    }}
                  />
                  <div className="space-y-2 p-4">
                    <div className="flex items-start gap-2">
                      <FileImageIcon className="mt-0.5 size-4 shrink-0 text-muted-foreground" />
                      <div className="min-w-0 text-left">
                        <p className="truncate text-sm font-semibold text-foreground">
                          {image.description || image.fileName || "Asset Unit image"}
                        </p>
                        <p className="mt-1 text-xs text-muted-foreground">
                          {image.contentType || "image"} /{" "}
                          {image.fileSizeBytes ? `${Math.round(image.fileSizeBytes / 1024)} KB` : "-"}
                        </p>
                      </div>
                    </div>
                  </div>
                </button>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      <DataTableShell>
        {!detail || detail.histories.length === 0 ? (
          <TableEmptyState
            title={isLoading ? "Loading history" : "No usage history"}
            description="History entries will appear after requests, transfers, returns, or status updates."
          />
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Action</TableHead>
                <TableHead>Availability</TableHead>
                <TableHead>Operational</TableHead>
                <TableHead>Responsible User</TableHead>
                <TableHead>Performed By</TableHead>
                <TableHead>Performed</TableHead>
                <TableHead>Reference</TableHead>
                <TableHead>Remark</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {detail.histories.map((history, index) => (
                <TableRow key={`${history.assetUnitId}-${history.performedAt}-${index}`}>
                  <TableCell className="font-semibold">{history.actionType}</TableCell>
                  <TableCell>
                    {renderTransition(
                      history.fromAvailabilityStatus,
                      history.toAvailabilityStatus
                    )}
                  </TableCell>
                  <TableCell>
                    {renderTransition(
                      history.fromOperationalStatus,
                      history.toOperationalStatus
                    )}
                  </TableCell>
                  <TableCell>
                    {renderTransition(
                      getUserName(history.fromResponsibleUserId),
                      getUserName(history.toResponsibleUserId)
                    )}
                  </TableCell>
                  <TableCell>{getUserName(history.performedBy)}</TableCell>
                  <TableCell>
                    <div className="flex items-center gap-2">
                      <CalendarClockIcon className="size-4 text-muted-foreground" />
                      {formatDateTime(history.performedAt)}
                    </div>
                  </TableCell>
                  <TableCell>{getReference(history)}</TableCell>
                  <TableCell>{history.remark || "-"}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </DataTableShell>

      <Dialog open={Boolean(selectedImage)} onOpenChange={(open) => !open && setSelectedImage(null)}>
        <DialogContent className="w-[min(960px,calc(100vw-32px))] max-w-none gap-4 p-4">
          <DialogHeader>
            <DialogTitle>
              {selectedImage?.description || selectedImage?.fileName || "Asset Unit image"}
            </DialogTitle>
            <DialogDescription>
              {selectedImage?.contentType || "image"} /{" "}
              {selectedImage?.fileSizeBytes
                ? `${Math.round(selectedImage.fileSizeBytes / 1024)} KB`
                : "-"}
            </DialogDescription>
          </DialogHeader>
          {selectedImage ? (
            <div
              aria-label={selectedImage.description || selectedImage.fileName || "Asset Unit image"}
              className="h-[min(72vh,720px)] rounded-[16px] border border-border bg-muted bg-contain bg-center bg-no-repeat"
              role="img"
              style={{
                backgroundImage: `url("${resolveAssetImageUrl(selectedImage.imageUrl)}")`,
              }}
            />
          ) : null}
        </DialogContent>
      </Dialog>
    </div>
  )
}

function InfoTile({
  icon,
  label,
  value,
}: {
  icon: React.ReactNode
  label: string
  value: string
}) {
  return (
    <div className="rounded-[14px] border border-border bg-muted/30 p-4">
      <div className="flex items-center gap-2 text-xs font-medium text-muted-foreground">
        {icon}
        {label}
      </div>
      <p className="mt-2 truncate text-sm font-semibold text-foreground">{value}</p>
    </div>
  )
}
