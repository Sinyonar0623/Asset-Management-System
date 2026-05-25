import type { LucideIcon } from "lucide-react"

import { Card, CardContent } from "@/components/ui/card"
import { cn } from "@/lib/utils"

export function KpiCard({
  title,
  value,
  description,
  icon: Icon,
  tone = "blue",
}: {
  title: string
  value: string | number
  description?: string
  icon: LucideIcon
  tone?: "blue" | "green" | "amber" | "red" | "slate"
}) {
  const tones = {
    blue: "bg-blue-600/10 text-blue-700",
    green: "bg-green-600/10 text-green-700",
    amber: "bg-amber-500/10 text-amber-700",
    red: "bg-red-600/10 text-red-700",
    slate: "bg-slate-600/10 text-slate-700",
  }

  return (
    <Card className="min-h-[154px]">
      <CardContent className="flex h-full items-start justify-between gap-7 px-7 py-7">
        <div className="space-y-4">
          <p className="text-base font-medium text-muted-foreground">{title}</p>
          <p className="text-[40px] font-semibold leading-none tracking-normal text-foreground">{value}</p>
          {description && <p className="text-sm font-medium text-muted-foreground">{description}</p>}
        </div>
        <div className={cn("flex size-14 items-center justify-center rounded-[18px]", tones[tone])}>
          <Icon className="size-6" />
        </div>
      </CardContent>
    </Card>
  )
}
