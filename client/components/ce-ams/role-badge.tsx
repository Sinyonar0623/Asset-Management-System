import { Badge } from "@/components/ui/badge"
import { cn } from "@/lib/utils"

const roleLabel: Record<string, string> = {
  STUDENT: "Student",
  TEACHER: "Teacher",
  HOD: "HOD",
  ADMIN: "Admin",
}

const roleStyles: Record<string, string> = {
  STUDENT: "border-sky-500/20 bg-sky-500/10 text-sky-700",
  TEACHER: "border-blue-600/20 bg-blue-600/10 text-blue-700",
  HOD: "border-slate-900/20 bg-slate-900/10 text-slate-900",
  ADMIN: "border-indigo-600/20 bg-indigo-600/10 text-indigo-700",
}

export function RoleBadge({
  role,
  className,
}: {
  role?: string | null
  className?: string
}) {
  const normalized = (role || "UNKNOWN").toUpperCase()

  return (
    <Badge
      variant="outline"
      className={cn("h-7 border px-3 text-xs font-semibold", roleStyles[normalized], className)}
    >
      {roleLabel[normalized] ?? normalized}
    </Badge>
  )
}
