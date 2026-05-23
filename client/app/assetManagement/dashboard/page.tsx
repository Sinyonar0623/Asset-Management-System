"use client"

// API:
// GET /Asset/count
// Returns: { count: number }
// GET /Request?pageNumber=0&pageSize=100
// Returns: { requests: PaginatedResult<RequestDto> }
// GET /Laboratory
// Returns: { laboratories: LaboratoryDto[] }
// GET /Asset/laboratory/{laboratoryId}?pageNumber=0&pageSize=6
// Returns: { assets: PaginatedResult<AssetDto> }

import * as React from "react"
import Link from "next/link"
import {
  CheckCircle2Icon,
  Clock3Icon,
  PackageIcon,
  XCircleIcon,
} from "lucide-react"

import { DataTableShell, TableEmptyState } from "@/components/ce-ams/data-table-shell"
import { KpiCard } from "@/components/ce-ams/kpi-card"
import { PageHeader } from "@/components/ce-ams/page-header"
import { StatusBadge } from "@/components/ce-ams/status-badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import {
  getAssetCount,
  getAssetUnitsByAssetId,
  getAssets,
  getAssetsByLaboratory,
  getLaboratories,
  getRequests,
  getUserLookup,
  shortId,
  type AssetDto,
  type AssetUnitDto,
  type LaboratoryDto,
  type RequestDto,
} from "@/lib/ce-ams-api"
import { useAuth } from "@/context/AuthContext"

type AssetSummary = AssetDto & {
  borrowerNames: string[]
  unitCount: number
}

function getLaboratoryLabel(laboratory: LaboratoryDto) {
  const name = laboratory.laboratoryName?.trim()
  const roomNo = laboratory.roomNo?.trim()

  if (name && roomNo) return `${name} (${roomNo})`
  return name || roomNo || laboratory.id || "Unnamed laboratory"
}

function getAssetStatus(asset: AssetDto) {
  return asset.availabilityStatus || (asset.isAvailable ? "AVAILABLE" : "UNAVAILABLE")
}

function getResponsibleUserIds(units: AssetUnitDto[]) {
  return units
    .filter((unit) => unit.availabilityStatus?.toUpperCase() === "IN_USE")
    .map((unit) => unit.responsibleUserId)
    .filter((id): id is string => Boolean(id))
}

export default function DashboardPage() {
  const { session } = useAuth()
  const [assetCount, setAssetCount] = React.useState<number | null>(null)
  const [requests, setRequests] = React.useState<RequestDto[]>([])
  const [assignedLaboratories, setAssignedLaboratories] = React.useState<LaboratoryDto[]>([])
  const [assets, setAssets] = React.useState<AssetSummary[]>([])
  const [isLoading, setLoading] = React.useState(true)
  const [isLoadingAssets, setLoadingAssets] = React.useState(true)

  React.useEffect(() => {
    Promise.allSettled([getAssetCount(), getRequests(0, 100), getLaboratories()]).then(
      (results) => {
        const [assetResult, requestResult, laboratoryResult] = results

        if (assetResult.status === "fulfilled") setAssetCount(assetResult.value)
        if (requestResult.status === "fulfilled") setRequests(requestResult.value.items)

        if (laboratoryResult.status === "fulfilled" && session?.role === "lecturer") {
          const userId = session.userId.toLowerCase()
          setAssignedLaboratories(
            laboratoryResult.value.filter(
              (laboratory) => laboratory.teacherId?.toLowerCase() === userId
            )
          )
        } else {
          setAssignedLaboratories([])
        }

        setLoading(false)
      }
    )
  }, [session?.role, session?.userId])

  React.useEffect(() => {
    let isActive = true

    async function loadAssetSummaries() {
      if (!session) return
      if (isLoading) {
        setLoadingAssets(true)
        return
      }

      setLoadingAssets(true)

      try {
        const assetItems =
          session.role === "lecturer"
            ? (
                await Promise.allSettled(
                  assignedLaboratories
                    .filter((laboratory) => laboratory.id)
                    .map((laboratory) => getAssetsByLaboratory(laboratory.id as string, 0, 6))
                )
              ).flatMap((result) => (result.status === "fulfilled" ? result.value.items : []))
            : (await getAssets(0, 6)).items

        const uniqueAssets = Array.from(
          new Map(assetItems.filter((asset) => asset.id).map((asset) => [asset.id, asset])).values()
        ).slice(0, 6)

        const unitsByAssetId = new Map<string, AssetUnitDto[]>()
        await Promise.allSettled(
          uniqueAssets.map(async (asset) => {
            const units = await getAssetUnitsByAssetId(asset.id as string)
            unitsByAssetId.set(asset.id as string, units)
          })
        )

        const borrowerIds = Array.from(
          new Set(
            Array.from(unitsByAssetId.values()).flatMap((units) => getResponsibleUserIds(units))
          )
        )
        const borrowers = await getUserLookup(borrowerIds).catch(() => [])
        const borrowerNameById = new Map(borrowers.map((user) => [user.userId, user.username]))

        const summaries = uniqueAssets.map((asset) => {
          const units = asset.id ? unitsByAssetId.get(asset.id) ?? [] : []
          const borrowerNames = Array.from(
            new Set(
              getResponsibleUserIds(units).map(
                (userId) => borrowerNameById.get(userId) ?? shortId(userId)
              )
            )
          )

          return {
            ...asset,
            borrowerNames,
            unitCount: units.length,
          }
        })

        if (isActive) setAssets(summaries)
      } catch {
        if (isActive) setAssets([])
      } finally {
        if (isActive) setLoadingAssets(false)
      }
    }

    if (session?.role === "lecturer" && assignedLaboratories.length === 0 && !isLoading) {
      setAssets([])
      setLoadingAssets(false)
      return
    }

    void loadAssetSummaries()

    return () => {
      isActive = false
    }
  }, [assignedLaboratories, isLoading, session])

  const pending = requests.filter((item) => item.status === "PENDING").length
  const approved = requests.filter((item) => item.status === "APPROVED").length
  const rejected = requests.filter((item) => item.status === "REJECTED").length
  const isLecturer = session?.role === "lecturer"
  const hasAssignedLaboratory = assignedLaboratories.length > 0

  return (
    <div className="grid min-h-[calc(100svh-144px)] grid-rows-[auto_auto_auto_minmax(0,1fr)] gap-8">
      <PageHeader
        title="Dashboard"
        subtitle="Overview of department assets and approval workload."
      />

      <div className="grid grid-cols-4 gap-7">
        <KpiCard
          title="Total Assets"
          value={assetCount ?? (isLoading ? "..." : "Requires API")}
          description="From GET /Asset/count"
          icon={PackageIcon}
        />
        <KpiCard
          title="Pending Requests"
          value={isLoading ? "..." : pending}
          description="Current request page"
          icon={Clock3Icon}
          tone="amber"
        />
        <KpiCard
          title="Approved Requests"
          value={isLoading ? "..." : approved}
          description="Current request page"
          icon={CheckCircle2Icon}
          tone="green"
        />
        <KpiCard
          title="Rejected Requests"
          value={isLoading ? "..." : rejected}
          description="Current request page"
          icon={XCircleIcon}
          tone="red"
        />
      </div>

      {isLecturer && (
        <section className="space-y-4">
          <PageHeader
            title="Assigned Laboratory"
            subtitle={
              hasAssignedLaboratory
                ? "Laboratories currently assigned to you."
                : "No laboratory is assigned to this teacher account."
            }
          />
          {hasAssignedLaboratory ? (
            <div className="grid grid-cols-3 gap-5">
              {assignedLaboratories.map((laboratory) => (
                <Card key={laboratory.id ?? getLaboratoryLabel(laboratory)}>
                  <CardContent className="space-y-3 p-6">
                    <div className="space-y-1">
                      <p className="text-lg font-semibold text-foreground">
                        {laboratory.laboratoryName || "Unnamed laboratory"}
                      </p>
                      <p className="text-sm font-medium text-muted-foreground">
                        Room {laboratory.roomNo || "-"}
                      </p>
                    </div>
                    <p className="text-sm leading-6 text-muted-foreground">
                      {laboratory.description || "No description"}
                    </p>
                  </CardContent>
                </Card>
              ))}
            </div>
          ) : (
            <DataTableShell className="min-h-48">
              <TableEmptyState
                title={isLoading ? "Loading laboratory assignment" : "No assigned laboratory"}
                description="Assets will stay empty until an admin assigns this teacher to a laboratory."
              />
            </DataTableShell>
          )}
        </section>
      )}

      <section className="flex min-h-0 flex-col gap-5">
        <PageHeader
          title="Laboratory Assets"
          subtitle={
            isLecturer
              ? "Assets in your assigned laboratory with availability and borrower details."
              : "Recent assets with availability and borrower details."
          }
          actions={
            <Button asChild variant="outline" size="lg">
              <Link href="/assetManagement/assets">View all</Link>
            </Button>
          }
        />
        <DataTableShell className="flex min-h-[420px] flex-1">
          {assets.length === 0 ? (
            <TableEmptyState
              title={isLoadingAssets ? "Loading assets" : "No assets found"}
              description={
                isLecturer && !hasAssignedLaboratory
                  ? "No assets are shown because this teacher is not assigned to a laboratory."
                  : "Assets will appear here when the backend returns laboratory asset records."
              }
            />
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Asset</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Available</TableHead>
                  <TableHead>Borrowed By</TableHead>
                  <TableHead>Location</TableHead>
                  <TableHead className="text-right">Action</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {assets.map((asset) => {
                  const status = getAssetStatus(asset)
                  const isAvailable = status.toUpperCase() === "AVAILABLE"

                  return (
                    <TableRow key={asset.id ?? asset.name}>
                      <TableCell>
                        <div className="space-y-1">
                          <p className="font-semibold text-foreground">{asset.name}</p>
                          <p className="text-xs text-muted-foreground">
                            {asset.category || "Uncategorized"} - {asset.unitCount} units
                          </p>
                        </div>
                      </TableCell>
                      <TableCell>
                        <StatusBadge status={status} />
                      </TableCell>
                      <TableCell className="font-medium">
                        {isAvailable ? "Available" : "Not available"}
                      </TableCell>
                      <TableCell className="text-muted-foreground">
                        {asset.borrowerNames.length > 0 ? asset.borrowerNames.join(", ") : "-"}
                      </TableCell>
                      <TableCell className="text-muted-foreground">
                        {asset.location || "-"}
                      </TableCell>
                      <TableCell className="text-right">
                        <Button asChild variant="ghost" size="sm">
                          <Link href={`/assetManagement/assets/${asset.id}`}>Open</Link>
                        </Button>
                      </TableCell>
                    </TableRow>
                  )
                })}
              </TableBody>
            </Table>
          )}
        </DataTableShell>
      </section>
    </div>
  )
}
