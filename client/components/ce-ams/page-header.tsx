import * as React from "react"

import { cn } from "@/lib/utils"

export function PageHeader({
  title,
  subtitle,
  actions,
  className,
}: {
  title: string
  subtitle?: string
  actions?: React.ReactNode
  className?: string
}) {
  return (
    <div className={cn("flex items-start justify-between gap-7", className)}>
      <div className="space-y-2">
        <h1 className="text-[34px] font-semibold leading-tight tracking-normal text-foreground">
          {title}
        </h1>
        {subtitle && <p className="text-base text-muted-foreground">{subtitle}</p>}
      </div>
      {actions && <div className="flex shrink-0 items-center gap-3">{actions}</div>}
    </div>
  )
}
