import * as React from "react"

import { Card } from "@/components/ui/card"
import { cn } from "@/lib/utils"

export function DataTableShell({
  children,
  className,
}: {
  children: React.ReactNode
  className?: string
}) {
  return (
    <Card className={cn("gap-0 overflow-hidden py-0", className)}>
      {children}
    </Card>
  )
}

export function TableEmptyState({
  title,
  description,
}: {
  title: string
  description: string
}) {
  return (
    <div className="flex min-h-72 flex-1 flex-col items-center justify-center gap-3 px-8 text-center">
      <p className="text-base font-semibold text-foreground">{title}</p>
      <p className="max-w-lg text-base leading-7 text-muted-foreground">{description}</p>
    </div>
  )
}
