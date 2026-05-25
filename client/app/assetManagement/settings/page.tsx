"use client"

import * as React from "react"
import { Loader2Icon, SaveIcon, SearchIcon, ShieldCheckIcon } from "lucide-react"

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
import { toast } from "@/components/ui/sonner"
import {
  assignTeacherToLaboratory,
  getLaboratories,
  getTeachers,
  type LaboratoryDto,
  type TeacherDto,
} from "@/lib/ce-ams-api"
import { useAuth } from "@/context/AuthContext"

const UNASSIGNED_TEACHER = "__unassigned__"

function getLaboratoryLabel(laboratory: LaboratoryDto) {
  const name = laboratory.laboratoryName?.trim()
  const roomNo = laboratory.roomNo?.trim()

  if (name && roomNo) return `${name} (${roomNo})`
  return name || roomNo || laboratory.id || "Unnamed laboratory"
}

export default function SettingsPage() {
  const { session } = useAuth()
  const [laboratories, setLaboratories] = React.useState<LaboratoryDto[]>([])
  const [teachers, setTeachers] = React.useState<TeacherDto[]>([])
  const [selectedTeacherByLab, setSelectedTeacherByLab] = React.useState<Record<string, string>>({})
  const [search, setSearch] = React.useState("")
  const [isLoading, setLoading] = React.useState(true)
  const [savingLabId, setSavingLabId] = React.useState<string | null>(null)

  const isAdmin = session?.role === "admin"

  React.useEffect(() => {
    if (!isAdmin) {
      setLoading(false)
      return
    }

    setLoading(true)
    Promise.all([getLaboratories(), getTeachers()])
      .then(([laboratoryItems, teacherItems]) => {
        setLaboratories(laboratoryItems)
        setTeachers(teacherItems)
        setSelectedTeacherByLab(
          Object.fromEntries(
            laboratoryItems
              .filter((laboratory) => laboratory.id)
              .map((laboratory) => [
                laboratory.id as string,
                laboratory.teacherId ?? UNASSIGNED_TEACHER,
              ])
          )
        )
      })
      .catch(() => {
        toast({
          title: "Settings could not be loaded",
          description: "Please confirm the laboratory and teacher APIs are available.",
          variant: "destructive",
        })
      })
      .finally(() => setLoading(false))
  }, [isAdmin])

  const teacherNameById = React.useMemo(
    () => new Map(teachers.map((teacher) => [teacher.teacherId, teacher.username])),
    [teachers]
  )

  const visibleLaboratories = laboratories.filter((laboratory) => {
    const target = `${laboratory.laboratoryName ?? ""} ${laboratory.roomNo ?? ""} ${
      laboratory.description ?? ""
    } ${teacherNameById.get(laboratory.teacherId ?? "") ?? ""}`.toLowerCase()

    return target.includes(search.toLowerCase())
  })

  function updateSelectedTeacher(laboratoryId: string, teacherId: string) {
    setSelectedTeacherByLab((current) => ({
      ...current,
      [laboratoryId]: teacherId,
    }))
  }

  async function assignTeacher(laboratory: LaboratoryDto) {
    if (!laboratory.id) return

    const teacherId = selectedTeacherByLab[laboratory.id]
    if (!teacherId || teacherId === UNASSIGNED_TEACHER || teacherId === laboratory.teacherId) {
      return
    }

    setSavingLabId(laboratory.id)
    try {
      await assignTeacherToLaboratory(laboratory.id, teacherId)
      setLaboratories((current) =>
        current.map((item) =>
          item.id === laboratory.id
            ? {
                ...item,
                teacherId,
              }
            : item
        )
      )
      toast("Teacher assigned to laboratory")
    } catch {
      toast({
        title: "Teacher could not be assigned",
        description: "Only admins can assign teachers to laboratories.",
        variant: "destructive",
      })
    } finally {
      setSavingLabId(null)
    }
  }

  return (
    <div className="space-y-6">
      <PageHeader
        title="Settings"
        subtitle="Manage laboratory ownership and system controls for CE-AMS."
        actions={
          <Badge
            variant="outline"
            className={
              isAdmin
                ? "border-green-600/20 bg-green-600/10 text-green-700"
                : "border-slate-300 bg-slate-100 text-slate-600"
            }
          >
            <ShieldCheckIcon className="mr-1 size-3.5" />
            {isAdmin ? "Admin" : "Admin only"}
          </Badge>
        }
      />

      <div className="flex flex-wrap items-end justify-between gap-4">
        <div>
          <h2 className="text-xl font-semibold text-foreground">
            Laboratory Teacher Assignment
          </h2>
          <p className="mt-1 text-sm text-muted-foreground">
            Assign the responsible teacher for each laboratory.
          </p>
        </div>
        <div className="relative w-full max-w-md">
          <SearchIcon className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            className="pl-9"
            placeholder="Search laboratories"
            disabled={!isAdmin || isLoading}
          />
        </div>
      </div>

      <DataTableShell>
        {!isAdmin ? (
          <TableEmptyState
            title="Admin access required"
            description="Laboratory teacher assignment is available for admin accounts."
          />
        ) : visibleLaboratories.length === 0 ? (
          <TableEmptyState
            title={isLoading ? "Loading settings" : "No laboratories found"}
            description="Laboratories returned by the API will appear here."
          />
        ) : (
          <div className="divide-y divide-border">
            {visibleLaboratories.map((laboratory) => {
              const laboratoryId = laboratory.id
              const currentTeacherName =
                teacherNameById.get(laboratory.teacherId ?? "") ?? "Unassigned"
              const selectedTeacherId = laboratoryId
                ? selectedTeacherByLab[laboratoryId] ?? laboratory.teacherId ?? UNASSIGNED_TEACHER
                : UNASSIGNED_TEACHER
              const canSave =
                Boolean(laboratoryId) &&
                selectedTeacherId !== UNASSIGNED_TEACHER &&
                selectedTeacherId !== laboratory.teacherId &&
                savingLabId !== laboratoryId

              return (
                <div
                  key={laboratoryId ?? getLaboratoryLabel(laboratory)}
                  className="grid gap-4 p-4 lg:grid-cols-[minmax(220px,1fr)_160px_minmax(180px,220px)_minmax(220px,260px)_auto] lg:items-center"
                >
                  <div className="min-w-0">
                    <p className="truncate font-semibold text-foreground">
                      {laboratory.laboratoryName || "Unnamed laboratory"}
                    </p>
                    <p className="truncate text-sm text-muted-foreground">
                      {laboratory.description || "No description"}
                    </p>
                  </div>
                  <div>
                    <p className="text-xs font-semibold uppercase text-muted-foreground">
                      Room
                    </p>
                    <p className="mt-1 font-medium">{laboratory.roomNo || "-"}</p>
                  </div>
                  <div>
                    <p className="text-xs font-semibold uppercase text-muted-foreground">
                      Current teacher
                    </p>
                    <Badge variant="outline" className="mt-1">
                      {currentTeacherName}
                    </Badge>
                  </div>
                  <Select
                    value={selectedTeacherId}
                    onValueChange={(teacherId) => {
                      if (laboratoryId) updateSelectedTeacher(laboratoryId, teacherId)
                    }}
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Select teacher" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value={UNASSIGNED_TEACHER}>Unassigned</SelectItem>
                      {teachers.map((teacher) => (
                        <SelectItem key={teacher.teacherId} value={teacher.teacherId}>
                          {teacher.username}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <Button
                    type="button"
                    size="sm"
                    disabled={!canSave}
                    onClick={() => assignTeacher(laboratory)}
                  >
                    {savingLabId === laboratoryId ? (
                      <Loader2Icon className="size-4 animate-spin" />
                    ) : (
                      <SaveIcon className="size-4" />
                    )}
                    Assign
                  </Button>
                </div>
              )
            })}
          </div>
        )}
      </DataTableShell>
    </div>
  )
}
