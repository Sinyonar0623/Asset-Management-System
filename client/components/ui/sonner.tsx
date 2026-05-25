"use client"

import * as React from "react"
import { Toast as ToastPrimitive } from "radix-ui"
import { XIcon } from "lucide-react"

import { cn } from "@/lib/utils"

type ToastMessage = {
  id: number
  title: string
  description?: string
  variant?: "default" | "destructive"
}

let addToast: ((message: Omit<ToastMessage, "id">) => void) | null = null

function toast(message: Omit<ToastMessage, "id"> | string) {
  const payload = typeof message === "string" ? { title: message } : message
  addToast?.(payload)
}

function Toaster() {
  const [messages, setMessages] = React.useState<ToastMessage[]>([])

  React.useEffect(() => {
    addToast = (message) => {
      const id = Date.now()
      setMessages((current) => [...current, { id, ...message }])
      window.setTimeout(() => {
        setMessages((current) => current.filter((item) => item.id !== id))
      }, 3800)
    }

    return () => {
      addToast = null
    }
  }, [])

  return (
    <ToastPrimitive.Provider swipeDirection="right">
      {messages.map((message) => (
        <ToastPrimitive.Root
          key={message.id}
          open
          data-variant={message.variant ?? "default"}
          className={cn(
            "grid w-96 max-w-[calc(100vw-32px)] grid-cols-[1fr_auto] items-start gap-3 rounded-[16px] border border-border bg-card p-4 shadow-[var(--ce-ams-shadow-card)] data-[variant=destructive]:border-destructive/30"
          )}
        >
          <div className="space-y-1">
            <ToastPrimitive.Title className="text-sm font-semibold">
              {message.title}
            </ToastPrimitive.Title>
            {message.description && (
              <ToastPrimitive.Description className="text-sm text-muted-foreground">
                {message.description}
              </ToastPrimitive.Description>
            )}
          </div>
          <ToastPrimitive.Close className="rounded-full p-1 text-muted-foreground hover:bg-muted">
            <XIcon className="size-4" />
          </ToastPrimitive.Close>
        </ToastPrimitive.Root>
      ))}
      <ToastPrimitive.Viewport className="fixed bottom-4 right-4 z-50 flex flex-col gap-3 outline-none" />
    </ToastPrimitive.Provider>
  )
}

export { Toaster, toast }
