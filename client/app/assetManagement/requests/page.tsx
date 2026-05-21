"use client"

// API:
// GET /Request?pageNumber=0&pageSize=20
// Returns: { requests: PaginatedResult<RequestDto> }
// POST /Request
// Returns: { id: string }

import * as React from "react"
import Link from "next/link"
import { SearchIcon } from "lucide-react"

import { ApiNotice } from "@/components/ce-ams/api-notice"
import { CreateRequestDialog } from "@/components/ce-ams/create-request-dialog"
import { DataTableShell, TableEmptyState } from "@/components/ce-ams/data-table-shell"
import { PageHeader } from "@/components/ce-ams/page-header"
import { RoleBadge } from "@/components/ce-ams/role-badge"
import { StatusBadge } from "@/components/ce-ams/status-badge"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { useAuth } from "@/context/AuthContext"
import { formatDate, getRequests, shortId, type RequestDto } from "@/lib/ce-ams-api"

const statuses = ["ALL", "PENDING", "APPROVED", "REJECTED", "CANCELLED", "COMPLETED"]
const roles = ["ALL", "STUDENT", "TEACHER", "HOD"]

export default function RequestListPage() {
  const { session } = useAuth()
  const [requests, setRequests] = React.useState<RequestDto[]>([])
  const [search, setSearch] = React.useState("")
  const [status, setStatus] = React.useState("PENDING")
  const [role, setRole] = React.useState("ALL")
  const [isLoading, setLoading] = React.useState(true)
  const canCreateRequest = session?.role !== "admin"

  const loadRequests = React.useCallback(() => {
    setLoading(true)
    getRequests(0, 20)
      .then((result) => setRequests(result.items))
      .finally(() => setLoading(false))
  }, [])

  React.useEffect(() => {
    loadRequests()
  }, [loadRequests])

  const visibleRequests = requests.filter((request) => {
    const currentTracking = request.trackings.find((item) => item.isCurrent)
    const text = `${request.id} ${request.requestType} ${request.reason} ${request.requesterId}`.toLowerCase()
    const matchesSearch = text.includes(search.toLowerCase())
    const matchesStatus = status === "ALL" || request.status === status
    const matchesRole = role === "ALL" || currentTracking?.requiredRoleCode === role
    return matchesSearch && matchesStatus && matchesRole
  })

  return (
    <div className="space-y-6">
      <PageHeader
        title="Requests"
        subtitle="Track student submissions through Teacher and HOD approval."
        actions={canCreateRequest ? <CreateRequestDialog onCreated={loadRequests} /> : null}
      />

      <div className="flex items-center justify-between gap-4">
        <div className="relative w-[360px]">
          <SearchIcon className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            className="pl-9"
            placeholder="Search requests"
          />
        </div>
        <div className="flex items-center gap-3">
          <Select value={status} onValueChange={setStatus}>
            <SelectTrigger className="w-44">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              {statuses.map((item) => (
                <SelectItem key={item} value={item}>
                  {item === "ALL" ? "All statuses" : item}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
          <Select value={role} onValueChange={setRole}>
            <SelectTrigger className="w-40">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              {roles.map((item) => (
                <SelectItem key={item} value={item}>
                  {item === "ALL" ? "All roles" : item}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      <ApiNotice title="Request display API gap">
        The request list API returns requester IDs and asset IDs, but not display names or asset titles. A ticket has been created for the missing table fields.
      </ApiNotice>

      <DataTableShell>
        {visibleRequests.length === 0 ? (
          <TableEmptyState
            title={isLoading ? "Loading requests" : "No requests found"}
            description="Requests returned by GET /Request will appear in this table."
          />
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Request ID</TableHead>
                <TableHead>Requester</TableHead>
                <TableHead>Asset/Item</TableHead>
                <TableHead>Quantity</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Current Step</TableHead>
                <TableHead>Created</TableHead>
                <TableHead className="text-right">Action</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {visibleRequests.map((request) => {
                const currentTracking = request.trackings.find((item) => item.isCurrent)

                return (
                  <TableRow key={request.id}>
                    <TableCell className="font-semibold">{shortId(request.id)}</TableCell>
                    <TableCell className="font-mono text-xs">{shortId(request.requesterId)}</TableCell>
                    <TableCell>
                      {request.items.length > 0
                        ? request.items.map((item) => shortId(item.assetId)).join(", ")
                        : "Requires API"}
                    </TableCell>
                    <TableCell>{request.items.length || "Requires API"}</TableCell>
                    <TableCell>
                      <StatusBadge status={request.status} />
                    </TableCell>
                    <TableCell>
                      {currentTracking ? (
                        <div className="flex items-center gap-2">
                          <RoleBadge role={currentTracking.requiredRoleCode} />
                          <span className="text-xs text-muted-foreground">Step {currentTracking.stepNo}</span>
                        </div>
                      ) : (
                        <span className="text-muted-foreground">Finalized</span>
                      )}
                    </TableCell>
                    <TableCell>{formatDate(request.submittedOn)}</TableCell>
                    <TableCell className="text-right">
                      <Button asChild variant="ghost" size="sm">
                        <Link href={`/assetManagement/requests/${request.id}`}>Open</Link>
                      </Button>
                    </TableCell>
                  </TableRow>
                )
              })}
            </TableBody>
          </Table>
        )}
      </DataTableShell>
    </div>
  )
}
