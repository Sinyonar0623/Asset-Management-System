"use client"

// API:
// GET /Asset?pageNumber=0&pageSize=20
// Returns: { assets: PaginatedResult<AssetDto> }

import * as React from "react"
import Link from "next/link"
import { FilterIcon, PlusIcon, RotateCcwIcon, SearchIcon } from "lucide-react"

import { DataTableShell, TableEmptyState } from "@/components/ce-ams/data-table-shell"
import { PageHeader } from "@/components/ce-ams/page-header"
import { StatusBadge } from "@/components/ce-ams/status-badge"
import { Button } from "@/components/ui/button"
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
import { formatDate, getAssets, returnAsset, shortId, type AssetDto } from "@/lib/ce-ams-api"
import { useAuth } from "@/context/AuthContext"
import { PERMISSIONS } from "@/lib/auth"

export default function AssetListPage() {
  const { session } = useAuth()
  const [assets, setAssets] = React.useState<AssetDto[]>([])
  const [search, setSearch] = React.useState("")
  const [isLoading, setLoading] = React.useState(true)
  const [returningAssetId, setReturningAssetId] = React.useState<string | null>(null)
  const canCreateAsset = session ? PERMISSIONS.canAddAsset(session.role) : false
  const canReturnAsset = session ? PERMISSIONS.canReturnAsset(session.role) : false

  const loadAssets = React.useCallback(() => {
    setLoading(true)
    getAssets(0, 20)
      .then((result) => setAssets(result.items))
      .finally(() => setLoading(false))
  }, [])

  React.useEffect(() => {
    loadAssets()
  }, [loadAssets])

  async function handleReturnAsset(assetId: string) {
    setReturningAssetId(assetId)
    try {
      await returnAsset(assetId)
      toast("Asset returned")
      loadAssets()
    } catch {
      toast({
        title: "Asset could not be returned",
        description: "Please confirm this asset is assigned to you or your role can return it.",
        variant: "destructive",
      })
    } finally {
      setReturningAssetId(null)
    }
  }

  function getAssetStatus(asset: AssetDto) {
    return asset.availabilityStatus || (asset.isAvailable ? "AVAILABLE" : "UNAVAILABLE")
  }

  const visibleAssets = assets.filter((asset) => {
    const target =
      `${asset.name} ${asset.category} ${asset.description} ${asset.location ?? ""}`.toLowerCase()
    return target.includes(search.toLowerCase())
  })

  return (
    <div className="space-y-6">
      <PageHeader title="Assets" subtitle="Browse and inspect department-managed assets." />

      <div className="flex items-center justify-between gap-4">
        <div className="relative w-[360px]">
          <SearchIcon className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            className="pl-9"
            placeholder="Search assets"
          />
        </div>
        <div className="flex items-center gap-3">
          <Button variant="outline">
            <FilterIcon className="size-4" />
            Filter
          </Button>
          {canCreateAsset && (
            <Button asChild>
              <Link href="/assetManagement/assets/new">
                <PlusIcon className="size-4" />
                Add Asset
              </Link>
            </Button>
          )}
        </div>
      </div>

      <DataTableShell>
        {visibleAssets.length === 0 ? (
          <TableEmptyState
            title={isLoading ? "Loading assets" : "No assets found"}
            description="Assets returned by GET /Asset will appear in this table."
          />
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Asset Name</TableHead>
                <TableHead>Code</TableHead>
                <TableHead>Category</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Location</TableHead>
                <TableHead>Updated</TableHead>
                <TableHead className="text-right">Action</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {visibleAssets.map((asset) => (
                <TableRow key={asset.id ?? asset.name}>
                  <TableCell>
                    <div className="space-y-1">
                      <p className="font-semibold text-foreground">{asset.name}</p>
                      <p className="text-xs text-muted-foreground">{asset.description || "No description"}</p>
                    </div>
                  </TableCell>
                  <TableCell className="font-mono text-xs">{shortId(asset.id)}</TableCell>
                  <TableCell>{asset.category || "Unassigned"}</TableCell>
                  <TableCell>
                    <StatusBadge status={getAssetStatus(asset)} />
                  </TableCell>
                  <TableCell className="text-muted-foreground">
                    {asset.location || "G - 001"}
                  </TableCell>
                  <TableCell className="text-muted-foreground">
                    {asset.updatedAt ? formatDate(asset.updatedAt) : "Not updated"}
                  </TableCell>
                  <TableCell>
                    {asset.id ? (
                      <div className="flex justify-end gap-2">
                        {canReturnAsset && getAssetStatus(asset) === "IN_USE" && (
                          <Button
                            variant="outline"
                            size="sm"
                            disabled={returningAssetId === asset.id}
                            onClick={() => handleReturnAsset(asset.id!)}
                          >
                            <RotateCcwIcon className="size-4" />
                            {returningAssetId === asset.id ? "Returning..." : "Return"}
                          </Button>
                        )}
                        <Button asChild variant="ghost" size="sm">
                          <Link href={`/assetManagement/assets/${asset.id}`}>Open</Link>
                        </Button>
                      </div>
                    ) : (
                      <Button variant="ghost" size="sm" disabled>
                        Open
                      </Button>
                    )}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </DataTableShell>
    </div>
  )
}
