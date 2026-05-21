"use client"

// API:
// GET /Asset/count
// Returns: { count: number }
// GET /Request?pageNumber=0&pageSize=100
// Returns: { requests: PaginatedResult<RequestDto> }

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
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import {
  formatDate,
  getAssetCount,
  getRequests,
  shortId,
  type RequestDto,
} from "@/lib/ce-ams-api"

export default function DashboardPage() {
  const [assetCount, setAssetCount] = React.useState<number | null>(null)
  const [requests, setRequests] = React.useState<RequestDto[]>([])
  const [isLoading, setLoading] = React.useState(true)

  React.useEffect(() => {
    Promise.allSettled([getAssetCount(), getRequests(0, 100)]).then((results) => {
      const [assetResult, requestResult] = results
      if (assetResult.status === "fulfilled") setAssetCount(assetResult.value)
      if (requestResult.status === "fulfilled") setRequests(requestResult.value.items)
      setLoading(false)
    })
  }, [])

  const pending = requests.filter((item) => item.status === "PENDING").length
  const approved = requests.filter((item) => item.status === "APPROVED").length
  const rejected = requests.filter((item) => item.status === "REJECTED").length
  const recentRequests = requests.slice(0, 6)

  return (
    <div className="grid min-h-[calc(100svh-144px)] grid-rows-[auto_auto_minmax(0,1fr)] gap-8">
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

      <section className="flex min-h-0 flex-col gap-5">
        <PageHeader
          title="Recent Requests"
          subtitle="Latest approval items from the request API."
          actions={
            <Button asChild variant="outline" size="lg">
              <Link href="/assetManagement/requests">View all</Link>
            </Button>
          }
        />
        <DataTableShell className="flex min-h-[420px] flex-1">
          {recentRequests.length === 0 ? (
            <TableEmptyState
              title={isLoading ? "Loading requests" : "No requests found"}
              description="Recent requests will appear here when the backend returns request records."
            />
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Request ID</TableHead>
                  <TableHead>Type</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Current Step</TableHead>
                  <TableHead>Created</TableHead>
                  <TableHead className="text-right">Action</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {recentRequests.map((request) => (
                  <TableRow key={request.id}>
                    <TableCell className="font-semibold">{shortId(request.id)}</TableCell>
                    <TableCell>{request.requestType}</TableCell>
                    <TableCell>
                      <StatusBadge status={request.status} />
                    </TableCell>
                    <TableCell>{request.currentStepNo ?? "Finalized"}</TableCell>
                    <TableCell>{formatDate(request.submittedOn)}</TableCell>
                    <TableCell className="text-right">
                      <Button asChild variant="ghost" size="sm">
                        <Link href={`/assetManagement/requests/${request.id}`}>Open</Link>
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          )}
        </DataTableShell>
      </section>
    </div>
  )
}
