import { AlertCircleIcon } from "lucide-react"

import { cn } from "@/lib/utils"

export function ApiNotice({
  title = "Backend API required",
  children,
  className,
}: {
  title?: string
  children: React.ReactNode
  className?: string
}) {
  return (
    <div
      className={cn(
        "flex gap-3 rounded-[16px] border border-blue-200 bg-blue-50 px-4 py-3 text-sm text-blue-950",
        className
      )}
    >
      <AlertCircleIcon className="mt-0.5 size-4 shrink-0 text-blue-700" />
      <div className="space-y-1">
        <p className="font-semibold">{title}</p>
        <div className="text-blue-900/80">{children}</div>
      </div>
    </div>
  )
}
