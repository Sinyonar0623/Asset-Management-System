"use client"

import { useEffect } from "react"
import { BellIcon, SearchIcon } from "lucide-react"
import { usePathname, useRouter } from "next/navigation"

import { AppSidebar } from "@/components/app-sidebar"
import { ThemeToggle } from "@/components/theme-toggle"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { SidebarProvider } from "@/components/ui/sidebar"
import { Spinner } from "@/components/ui/spinner"
import { useAuth } from "@/context/AuthContext"

function getHomePath(role?: string) {
  return role === "student" ? "/assetManagement/requests" : "/assetManagement/dashboard"
}

function canAccessPath(role: string | undefined, pathname: string) {
  if (role !== "student") return true

  if (pathname === "/assetManagement/assets/new") return false
  return (
    pathname === "/assetManagement/assets" ||
    pathname.startsWith("/assetManagement/assets/") ||
    pathname === "/assetManagement/requests" ||
    pathname.startsWith("/assetManagement/requests/")
  )
}

export function AssetManagementShell({
  children,
}: Readonly<{
  children: React.ReactNode
}>) {
  const { session, isLoading } = useAuth()
  const router = useRouter()
  const pathname = usePathname()

  useEffect(() => {
    if (!isLoading && !session) {
      router.replace("/login")
    }
  }, [isLoading, router, session])

  useEffect(() => {
    if (!isLoading && session && !canAccessPath(session.role, pathname)) {
      router.replace(getHomePath(session.role))
    }
  }, [isLoading, pathname, router, session])

  if (isLoading || !session || !canAccessPath(session.role, pathname)) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-background">
        <Spinner className="size-5" />
      </div>
    )
  }

  return (
    <SidebarProvider
      className="bg-background"
      style={
        {
          "--sidebar-width": "280px",
          "--sidebar-width-icon": "280px",
        } as React.CSSProperties
      }
    >
      <AppSidebar />
      <main className="min-h-svh w-full min-w-0 bg-background">
        <header className="sticky top-0 z-30 flex h-20 items-center justify-between border-b border-border bg-background/95 px-8 backdrop-blur">
          <div className="relative w-[520px]">
            <SearchIcon className="pointer-events-none absolute left-4 top-1/2 size-5 -translate-y-1/2 text-muted-foreground" />
            <Input
              className="h-12 pl-12 text-base"
              placeholder={session.role === "student" ? "Search assets and requests" : "Search assets, requests, users"}
            />
          </div>
          <div className="flex items-center gap-4">
            <ThemeToggle className="size-11" />
            <Button variant="outline" size="icon-lg" aria-label="Notifications">
              <BellIcon className="size-5" />
            </Button>
            <div className="flex items-center gap-4 rounded-[18px] border border-border bg-card px-4 py-3">
              <Avatar className="size-11">
                <AvatarFallback>
                  {session.name?.slice(0, 2).toUpperCase() || "CE"}
                </AvatarFallback>
              </Avatar>
              <div className="leading-tight">
                <p className="text-base font-semibold text-foreground">{session.name}</p>
                <p className="text-sm font-medium text-muted-foreground">{session.role}</p>
              </div>
            </div>
          </div>
        </header>
        <section className="p-8">{children}</section>
      </main>
    </SidebarProvider>
  )
}
