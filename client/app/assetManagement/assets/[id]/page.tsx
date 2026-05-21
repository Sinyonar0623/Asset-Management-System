"use client"

// API:
// GET /Asset/{id}
// Returns: { asset: AssetDto }
// GET /AssetUnit/asset/{assetId}
// Returns: { assetUnits: AssetUnitDto[] }
// GET /Asset/{id}/history
// Returns: { histories: AssetHistoryDto[] }

import * as React from "react"
import Link from "next/link"
import { useParams } from "next/navigation"
import { BoxIcon, EyeIcon, HistoryIcon, Layers3Icon, RotateCcwIcon } from "lucide-react"

import { DataTableShell, TableEmptyState } from "@/components/ce-ams/data-table-shell"
import { PageHeader } from "@/components/ce-ams/page-header"
import { StatusBadge } from "@/components/ce-ams/status-badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { toast } from "@/components/ui/sonner"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import {
  getAssetHistory,
  getAssetById,
  getAssetUnitsByAssetId,
  getUserLookup,
  formatDate,
  returnAsset,
  shortId,
  type AssetDto,
  type AssetHistoryDto,
  type AssetUnitDto,
} from "@/lib/ce-ams-api"
import { useAuth } from "@/context/AuthContext"
import { PERMISSIONS } from "@/lib/auth"

function collectUserIds(histories: AssetHistoryDto[]) {
  const ids = new Set<string>()

  histories.forEach((history) => {
    if (history.fromResponsibleUserId) ids.add(history.fromResponsibleUserId)
    if (history.toResponsibleUserId) ids.add(history.toResponsibleUserId)
    if (history.performedBy) ids.add(history.performedBy)
    if (history.approvedBy) ids.add(history.approvedBy)
  })

  return Array.from(ids)
}

export default function AssetDetailPage() {
  const params = useParams<{ id: string }>()
  const { session } = useAuth()
  const [asset, setAsset] = React.useState<AssetDto | null>(null)
  const [units, setUnits] = React.useState<AssetUnitDto[]>([])
  const [histories, setHistories] = React.useState<AssetHistoryDto[]>([])
  const [userNames, setUserNames] = React.useState<Record<string, string>>({})
  const [isLoading, setLoading] = React.useState(true)
  const [isReturning, setReturning] = React.useState(false)
  const canReturnAsset = session ? PERMISSIONS.canReturnAsset(session.role) : false
  const historyGroups = React.useMemo(() => {
    const unitById = new Map(
      units
        .filter((unit) => unit.id)
        .map((unit, index) => [
          unit.id!.toLowerCase(),
          {
            unit,
            order: index,
          },
        ])
    )
    const groups = new Map<
      string,
      {
        assetUnitId: string
        unit?: AssetUnitDto
        order: number
        histories: AssetHistoryDto[]
      }
    >()

    histories.forEach((history) => {
      const assetUnitId = history.assetUnitId
      const groupKey = assetUnitId.toLowerCase()
      const match = unitById.get(groupKey)

      if (!groups.has(groupKey)) {
        groups.set(groupKey, {
          assetUnitId,
          unit: match?.unit,
          order: match?.order ?? Number.MAX_SAFE_INTEGER,
          histories: [],
        })
      }

      groups.get(groupKey)?.histories.push(history)
    })

    return Array.from(groups.values()).sort((left, right) => {
      if (left.order !== right.order) return left.order - right.order
      return getHistoryLatestTime(right.histories) - getHistoryLatestTime(left.histories)
    })
  }, [histories, units])

  const loadAsset = React.useCallback(() => {
    if (!params.id) return

    setLoading(true)
    Promise.allSettled([
      getAssetById(params.id),
      getAssetUnitsByAssetId(params.id),
      getAssetHistory(params.id),
    ]).then(
      ([assetResult, unitResult, historyResult]) => {
        if (assetResult.status === "fulfilled") setAsset(assetResult.value)
        if (unitResult.status === "fulfilled") setUnits(unitResult.value)
        if (historyResult.status === "fulfilled") {
          const nextHistories = historyResult.value
          setHistories(nextHistories)

          getUserLookup(collectUserIds(nextHistories))
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
  }, [params.id])

  React.useEffect(() => {
    loadAsset()
  }, [loadAsset])

  async function handleReturnAsset() {
    if (!params.id) return

    setReturning(true)
    try {
      await returnAsset(params.id)
      toast("Asset returned")
      loadAsset()
    } catch {
      toast({
        title: "Asset could not be returned",
        description: "Please confirm this asset is assigned to you or your role can return it.",
        variant: "destructive",
      })
    } finally {
      setReturning(false)
    }
  }

  function getAssetStatus(currentAsset: AssetDto | null) {
    return currentAsset?.availabilityStatus || (currentAsset?.isAvailable ? "AVAILABLE" : "UNAVAILABLE")
  }

  function renderTransition(from?: string | null, to?: string | null) {
    const safeFrom = from && from !== "-" ? from : null
    const safeTo = to && to !== "-" ? to : null

    if (!safeFrom && !safeTo) return "No change"
    return `${safeFrom || "-"} -> ${safeTo || "-"}`
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

  function getHistoryLatestTime(items: AssetHistoryDto[]) {
    return items.reduce((latest, item) => {
      const time = new Date(item.performedAt).getTime()
      return Number.isNaN(time) ? latest : Math.max(latest, time)
    }, 0)
  }

  function getUnitTitle(assetUnitId: string, unit?: AssetUnitDto) {
    return unit?.name || `Unit ${shortId(assetUnitId)}`
  }

  function getUnitSubtitle(assetUnitId: string, unit?: AssetUnitDto) {
    if (!unit) return shortId(assetUnitId)
    return [unit.assetTag, unit.serialNo, unit.brand].filter(Boolean).join(" · ")
  }

  function getUserName(userId?: string | null) {
    if (!userId) return "-"
    return userNames[userId.toLowerCase()] ?? shortId(userId)
  }

  function renderHistoryTable(items: AssetHistoryDto[], showUnit: boolean) {
    return (
      <div className="overflow-x-auto">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Action</TableHead>
              {showUnit ? <TableHead>Unit</TableHead> : null}
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
            {items.map((history, index) => {
              const unit = units.find(
                (item) => item.id?.toLowerCase() === history.assetUnitId.toLowerCase()
              )

              return (
                <TableRow
                  key={`${history.assetUnitId}-${history.actionType}-${history.performedAt}-${index}`}
                >
                  <TableCell className="font-semibold">{history.actionType}</TableCell>
                  {showUnit ? (
                    <TableCell>
                      <div>
                        <p className="font-medium">{getUnitTitle(history.assetUnitId, unit)}</p>
                        <p className="text-xs text-muted-foreground">
                          {getUnitSubtitle(history.assetUnitId, unit)}
                        </p>
                      </div>
                    </TableCell>
                  ) : null}
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
                  <TableCell>{formatDateTime(history.performedAt)}</TableCell>
                  <TableCell>{history.referenceNo || shortId(history.requestId)}</TableCell>
                  <TableCell>{history.remark || "-"}</TableCell>
                </TableRow>
              )
            })}
          </TableBody>
        </Table>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <PageHeader
        title={asset?.name || "Asset Detail"}
        subtitle={asset ? `${asset.category || "Unassigned"} · ${shortId(asset.id)}` : "Loading asset information"}
        actions={
          canReturnAsset && asset?.id && getAssetStatus(asset) === "IN_USE" ? (
            <Button variant="outline" onClick={handleReturnAsset} disabled={isReturning}>
              <RotateCcwIcon className="size-4" />
              {isReturning ? "Returning..." : "Return"}
            </Button>
          ) : null
        }
      />

      <div className="grid grid-cols-[1.4fr_1fr_1fr] gap-6">
        <Card>
          <CardHeader>
            <CardTitle>Summary</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="flex items-center gap-4">
              <div className="flex size-14 items-center justify-center rounded-[20px] bg-blue-600/10 text-blue-700">
                <BoxIcon className="size-6" />
              </div>
              <div>
                <p className="text-lg font-semibold">{asset?.name || "Requires API"}</p>
                <p className="text-sm text-muted-foreground">{asset?.description || "No description"}</p>
              </div>
            </div>
            <StatusBadge status={getAssetStatus(asset)} />
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Units</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-3xl font-semibold">{isLoading ? "..." : units.length}</p>
            <p className="mt-2 text-sm text-muted-foreground">Registered physical units</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Metadata</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2 text-sm">
            <p className="flex justify-between gap-4">
              <span className="text-muted-foreground">Location</span>
              <span>{asset?.location || "กองกลางห้องเก็บของ"}</span>
            </p>
            <p className="flex justify-between gap-4">
              <span className="text-muted-foreground">Updated</span>
              <span>{asset?.updatedAt ? formatDate(asset.updatedAt) : "Not updated"}</span>
            </p>
          </CardContent>
        </Card>
      </div>

      <Tabs defaultValue="overview">
        <TabsList>
          <TabsTrigger value="overview">
            <Layers3Icon className="mr-2 size-4" />
            Overview
          </TabsTrigger>
          <TabsTrigger value="units">
            <BoxIcon className="mr-2 size-4" />
            Units
          </TabsTrigger>
          <TabsTrigger value="history">
            <HistoryIcon className="mr-2 size-4" />
            History
          </TabsTrigger>
        </TabsList>

        <TabsContent value="overview">
          <Card>
            <CardHeader>
              <CardTitle>Overview</CardTitle>
            </CardHeader>
            <CardContent className="grid grid-cols-3 gap-4 text-sm">
              <div className="rounded-[16px] border border-border p-4">
                <p className="text-muted-foreground">Category</p>
                <p className="mt-2 font-semibold">{asset?.category || "Unassigned"}</p>
              </div>
              <div className="rounded-[16px] border border-border p-4">
                <p className="text-muted-foreground">Code</p>
                <p className="mt-2 font-semibold">{shortId(asset?.id)}</p>
              </div>
              <div className="rounded-[16px] border border-border p-4">
                <p className="text-muted-foreground">Availability</p>
                <p className="mt-2 font-semibold">{getAssetStatus(asset)}</p>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="units">
          <DataTableShell>
            {units.length === 0 ? (
              <TableEmptyState
                title={isLoading ? "Loading units" : "No units found"}
                description="Asset unit records returned by GET /AssetUnit/asset/{assetId} will appear here."
              />
            ) : (
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Unit</TableHead>
                    <TableHead>Asset Tag</TableHead>
                    <TableHead>Serial</TableHead>
                    <TableHead>Brand</TableHead>
                    <TableHead>Availability</TableHead>
                    <TableHead>Operational</TableHead>
                    <TableHead className="text-right">Action</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {units.map((unit) => (
                    <TableRow key={unit.id ?? unit.assetTag}>
                      <TableCell className="font-semibold">{unit.name}</TableCell>
                      <TableCell>{unit.assetTag}</TableCell>
                      <TableCell>{unit.serialNo}</TableCell>
                      <TableCell>{unit.brand}</TableCell>
                      <TableCell>
                        <StatusBadge status={unit.availabilityStatus} />
                      </TableCell>
                      <TableCell>{unit.operationalStatus}</TableCell>
                      <TableCell className="text-right">
                        {unit.id ? (
                          <Button asChild variant="outline" size="sm">
                            <Link href={`/assetManagement/assets/${params.id}/units/${unit.id}`}>
                              <EyeIcon className="size-4" />
                              Detail
                            </Link>
                          </Button>
                        ) : (
                          "-"
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            )}
          </DataTableShell>
        </TabsContent>

        <TabsContent value="history">
          {histories.length === 0 ? (
            <DataTableShell>
              <TableEmptyState
                title={isLoading ? "Loading history" : "No history found"}
                description="Asset unit history events will appear here when this asset is assigned, reserved, returned, or otherwise updated."
              />
            </DataTableShell>
          ) : (
            <Tabs defaultValue="by-unit">
              <TabsList>
                <TabsTrigger value="by-unit">By Unit</TabsTrigger>
                <TabsTrigger value="timeline">All Timeline</TabsTrigger>
              </TabsList>

              <TabsContent value="by-unit">
                <div className="space-y-4">
                  {historyGroups.map((group) => (
                    <DataTableShell key={group.assetUnitId}>
                      <div className="flex flex-wrap items-center justify-between gap-3 border-b border-border px-4 py-3">
                        <div>
                          <p className="font-semibold">
                            {getUnitTitle(group.assetUnitId, group.unit)}
                          </p>
                          <p className="text-sm text-muted-foreground">
                            {getUnitSubtitle(group.assetUnitId, group.unit)}
                          </p>
                        </div>
                        <span className="rounded-full border border-border px-3 py-1 text-xs font-medium text-muted-foreground">
                          {group.histories.length} events
                        </span>
                      </div>
                      {renderHistoryTable(group.histories, false)}
                    </DataTableShell>
                  ))}
                </div>
              </TabsContent>

              <TabsContent value="timeline">
                <DataTableShell>{renderHistoryTable(histories, true)}</DataTableShell>
              </TabsContent>
            </Tabs>
          )}
        </TabsContent>
      </Tabs>
    </div>
  )
}
