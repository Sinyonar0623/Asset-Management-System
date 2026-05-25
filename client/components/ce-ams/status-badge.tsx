import { Badge } from "@/components/ui/badge"
import { cn } from "@/lib/utils"

const statusStyles: Record<string, string> = {
  PENDING: "border-amber-500/20 bg-amber-500/10 text-amber-700",
  APPROVED: "border-green-600/20 bg-green-600/10 text-green-700",
  REJECTED: "border-red-600/20 bg-red-600/10 text-red-700",
  COMPLETED: "border-slate-500/20 bg-slate-500/10 text-slate-600",
  CANCELLED: "border-slate-700/20 bg-slate-700/10 text-slate-700",
  WAITING: "border-slate-300 bg-slate-100 text-slate-600",
  SKIPPED: "border-slate-300 bg-slate-100 text-slate-500",
  AVAILABLE: "border-green-600/20 bg-green-600/10 text-green-700",
  RESERVED: "border-amber-500/20 bg-amber-500/10 text-amber-700",
  IN_USE: "border-blue-600/20 bg-blue-600/10 text-blue-700",
  UNAVAILABLE: "border-slate-700/20 bg-slate-700/10 text-slate-700",
}

export function StatusBadge({
  status,
  className,
}: {
  status?: string | null
  className?: string
}) {
  const normalized = (status || "UNKNOWN").toUpperCase()

  return (
    <Badge
      variant="outline"
      className={cn("h-7 border px-3 text-xs font-semibold", statusStyles[normalized], className)}
    >
      {normalized}
    </Badge>
  )
}
