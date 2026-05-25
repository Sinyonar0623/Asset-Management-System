"use client"

import Link from "next/link"
import { usePathname } from "next/navigation"
import {
  BarChart3Icon,
  BoxesIcon,
  FileTextIcon,
  LayoutDashboardIcon,
  LogOutIcon,
  SettingsIcon,
  SlidersHorizontalIcon,
  UsersIcon,
} from "lucide-react"

import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar"
import { useAuth } from "@/context/AuthContext"
import { cn } from "@/lib/utils"

const sections = [
  {
    label: "MAIN",
    items: [
      { title: "Dashboard", href: "/assetManagement/dashboard", icon: LayoutDashboardIcon },
      { title: "Assets", href: "/assetManagement/assets", icon: BoxesIcon },
      { title: "Requests", href: "/assetManagement/requests", icon: FileTextIcon },
    ],
  },
  {
    label: "MANAGEMENT",
    items: [
      { title: "Parameters", href: "/assetManagement/parameters", icon: SlidersHorizontalIcon },
      { title: "Users", href: "/assetManagement/users", icon: UsersIcon },
    ],
  },
  {
    label: "SYSTEM",
    items: [
      { title: "Reports", href: "/assetManagement/reports", icon: BarChart3Icon, disabled: true },
      { title: "Settings", href: "/assetManagement/settings", icon: SettingsIcon },
    ],
  },
]

function canShowItem(role: string | undefined, href: string) {
  if (role !== "student") return true
  return href === "/assetManagement/assets" || href === "/assetManagement/requests"
}

export function AppSidebar() {
  const pathname = usePathname()
  const { logout, session } = useAuth()
  const visibleSections = sections
    .map((section) => ({
      ...section,
      items: section.items.filter((item) => canShowItem(session?.role, item.href)),
    }))
    .filter((section) => section.items.length > 0)

  return (
    <Sidebar
      collapsible="none"
      className="sticky top-0 h-svh min-h-svh border-r border-sidebar-border"
    >
      <SidebarHeader className="px-7 py-8">
        <div className="space-y-2">
          <div className="text-[28px] font-semibold leading-none tracking-normal text-white">Asset Management System</div>
          
        </div>
      </SidebarHeader>

      <SidebarContent className="flex-1 px-5">
        {visibleSections.map((section) => (
          <SidebarGroup key={section.label} className="mb-8 gap-4 p-0">
            <p className="px-3 text-xs font-semibold uppercase tracking-[0.16em] text-slate-500">
              {section.label}
            </p>
            <SidebarMenu className="gap-3">
              {section.items.map((item) => {
                const Icon = item.icon
                const isActive =
                  pathname === item.href ||
                  (item.href !== "/assetManagement/dashboard" &&
                    pathname.startsWith(item.href))

                return (
                  <SidebarMenuItem key={item.title}>
                    <SidebarMenuButton
                      asChild={!item.disabled}
                      disabled={item.disabled}
                      className={cn(
                        "h-12 rounded-[14px] px-4 text-base font-medium text-slate-300 hover:bg-slate-900 hover:text-white data-[active=true]:border data-[active=true]:border-sky-300/40 data-[active=true]:bg-sky-100 data-[active=true]:font-semibold data-[active=true]:text-slate-950 data-[active=true]:shadow-[0_10px_30px_rgba(56,189,248,0.18)] data-[active=true]:[&_svg]:text-sky-700",
                        item.disabled && "cursor-not-allowed opacity-50"
                      )}
                      data-active={isActive}
                    >
                      {item.disabled ? (
                        <span className="flex items-center gap-3">
                          <Icon className="size-5" />
                          {item.title}
                        </span>
                      ) : (
                        <Link href={item.href} className="flex items-center gap-3">
                          <Icon className="size-5" />
                          {item.title}
                        </Link>
                      )}
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                )
              })}
            </SidebarMenu>
          </SidebarGroup>
        ))}
      </SidebarContent>

      <SidebarFooter className="mt-auto px-5 py-6">
        <button
          type="button"
          onClick={logout}
          className="flex h-12 w-full items-center gap-3 rounded-[14px] px-4 text-base font-medium text-slate-300 transition-colors hover:bg-slate-900 hover:text-white"
        >
          <LogOutIcon className="size-5" />
          Logout
        </button>
      </SidebarFooter>
    </Sidebar>
  )
}
