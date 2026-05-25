import { CheckIcon, ClockIcon, XIcon } from "lucide-react"

import { StatusBadge } from "@/components/ce-ams/status-badge"
import { RoleBadge } from "@/components/ce-ams/role-badge"
import type { RequestTrackingDto } from "@/lib/ce-ams-api"
import { formatDate } from "@/lib/ce-ams-api"
import { cn } from "@/lib/utils"

function StepIcon({ status }: { status?: string }) {
  const normalized = (status || "WAITING").toUpperCase()
  if (normalized === "APPROVED") return <CheckIcon className="size-4" />
  if (normalized === "REJECTED") return <XIcon className="size-4" />
  return <ClockIcon className="size-4" />
}

function getStepTitle(tracking: RequestTrackingDto, requestType?: string | null) {
  const role = tracking.requiredRoleCode.toUpperCase()
  const type = (requestType || "").toUpperCase()

  if (role === "TEACHER") {
    return type === "BORROW" ? "Teacher Asset Review" : "Teacher Review"
  }

  if (role === "HOD") {
    if (type === "ALLOCATE") return "HOD Allocation Approval"
    if (type === "REPAIR") return "HOD Repair Approval"
    if (type === "RETIRE") return "HOD Retirement Approval"
    return "HOD Decision"
  }

  if (role === "STUDENT") return "Student Submission"
  return `${role} Review`
}

export function ApprovalTimeline({
  trackings,
  requestType,
  actorNameById,
}: {
  trackings: RequestTrackingDto[]
  requestType?: string | null
  actorNameById?: Map<string, string>
}) {
  if (trackings.length === 0) {
    return (
      <div className="flex min-h-40 items-center justify-center rounded-[8px] border border-border text-sm text-muted-foreground">
        No approval steps returned by the API yet.
      </div>
    )
  }

  const sortedTrackings = [...trackings].sort((a, b) => a.stepNo - b.stepNo)

  return (
    <div className="grid gap-4 lg:grid-cols-3">
      {sortedTrackings.map((tracking, index) => {
        const status = tracking.status ?? "WAITING"
        const normalized = status.toUpperCase()
        const actorName = tracking.actionByUserId
          ? actorNameById?.get(tracking.actionByUserId.toLowerCase()) ?? tracking.actionByUserId.slice(0, 8)
          : "Waiting for action"

        return (
          <div key={`${tracking.stepNo}-${tracking.requiredRoleCode}`} className="relative">
            {index < sortedTrackings.length - 1 && (
              <div className="absolute left-[calc(50%+28px)] top-7 h-px w-[calc(100%-40px)] bg-border" />
            )}
            <div className="rounded-[20px] border border-border bg-card p-5">
              <div className="flex items-center justify-between gap-3">
                <div
                  className={cn(
                    "flex size-14 items-center justify-center rounded-full border text-slate-600",
                    normalized === "APPROVED" &&
                      "border-green-600/20 bg-green-600/10 text-green-700",
                    normalized === "REJECTED" &&
                      "border-red-600/20 bg-red-600/10 text-red-700",
                    normalized === "PENDING" &&
                      "border-amber-500/20 bg-amber-500/10 text-amber-700"
                  )}
                >
                  <StepIcon status={status} />
                </div>
                <StatusBadge status={status} />
              </div>
              <div className="mt-4 space-y-3">
                <div>
                  <p className="text-base font-semibold text-foreground">
                    {getStepTitle(tracking, requestType)}
                  </p>
                  <p className="mt-1 text-xs font-medium text-muted-foreground">
                    Step {tracking.stepNo}
                  </p>
                </div>
                <RoleBadge role={tracking.requiredRoleCode} />
                <div className="space-y-1 text-xs font-medium text-muted-foreground">
                  <p>Actor: {actorName}</p>
                  <p>Action: {tracking.actionOn ? formatDate(tracking.actionOn) : "Not actioned yet"}</p>
                </div>
                {tracking.comment && (
                  <p className="rounded-xl bg-muted px-3 py-2 text-xs text-muted-foreground">
                    {tracking.comment}
                  </p>
                )}
              </div>
            </div>
          </div>
        )
      })}
    </div>
  )
}
