"use client"

// API:
// GET /Parameters?pageNumber=0&pageSize=50
// Returns: { response: PaginatedResult<ParameterDto> }

import * as React from "react"
import { PlusIcon, SearchIcon } from "lucide-react"

import { ApiNotice } from "@/components/ce-ams/api-notice"
import { DataTableShell, TableEmptyState } from "@/components/ce-ams/data-table-shell"
import { PageHeader } from "@/components/ce-ams/page-header"
import { Badge } from "@/components/ui/badge"
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
import { getParameters, type ParameterDto } from "@/lib/ce-ams-api"

export default function ParameterListPage() {
  const [parameters, setParameters] = React.useState<ParameterDto[]>([])
  const [group, setGroup] = React.useState("ALL")
  const [search, setSearch] = React.useState("")
  const [isLoading, setLoading] = React.useState(true)

  React.useEffect(() => {
    getParameters(0, 50)
      .then((result) => setParameters(result.items))
      .finally(() => setLoading(false))
  }, [])

  const groups = ["ALL", ...Array.from(new Set(parameters.map((item) => item.group)))]
  const visibleParameters = parameters.filter((parameter) => {
    const matchesGroup = group === "ALL" || parameter.group === group
    const target = `${parameter.group} ${parameter.value} ${parameter.description}`.toLowerCase()
    return matchesGroup && target.includes(search.toLowerCase())
  })

  return (
    <div className="space-y-6">
      <PageHeader
        title="Parameters"
        subtitle="Manage reusable codes and values used across CE-AMS."
        actions={
          <Button disabled title="Parameter creation requires an edit dialog implementation.">
            <PlusIcon className="size-4" />
            Add Parameter
          </Button>
        }
      />

      <div className="flex items-center gap-3">
        <Select value={group} onValueChange={setGroup}>
          <SelectTrigger className="w-56">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            {groups.map((item) => (
              <SelectItem key={item} value={item}>
                {item === "ALL" ? "All groups" : item}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
        <div className="relative w-[360px]">
          <SearchIcon className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            className="pl-9"
            placeholder="Search parameters"
          />
        </div>
      </div>

      <ApiNotice title="Parameter table API gap">
        `ParameterDto` does not include an ID or updated timestamp, so row actions and updated values are intentionally disabled.
      </ApiNotice>

      <DataTableShell>
        {visibleParameters.length === 0 ? (
          <TableEmptyState
            title={isLoading ? "Loading parameters" : "No parameters found"}
            description="Parameters returned by GET /Parameters will appear in this table."
          />
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Group</TableHead>
                <TableHead>Value</TableHead>
                <TableHead>Description</TableHead>
                <TableHead>Active</TableHead>
                <TableHead>Updated</TableHead>
                <TableHead className="text-right">Action</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {visibleParameters.map((parameter) => (
                <TableRow key={`${parameter.group}-${parameter.value}`}>
                  <TableCell className="font-semibold">{parameter.group}</TableCell>
                  <TableCell>{parameter.value}</TableCell>
                  <TableCell>{parameter.description}</TableCell>
                  <TableCell>
                    <Badge
                      variant="outline"
                      className={parameter.active ? "border-green-600/20 bg-green-600/10 text-green-700" : "border-slate-300 bg-slate-100 text-slate-600"}
                    >
                      {parameter.active ? "Active" : "Inactive"}
                    </Badge>
                  </TableCell>
                  <TableCell className="text-muted-foreground">Requires API</TableCell>
                  <TableCell className="text-right">
                    <Button variant="ghost" size="sm" disabled>
                      Edit
                    </Button>
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
