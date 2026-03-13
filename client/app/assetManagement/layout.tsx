import { ThemeToggle } from "@/components/theme-toggle";
import { Input } from "@/components/ui/input";
import Image from "next/image";
import { SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar";
import { AppSidebar } from "@/components/app-sidebar";

export default function AssetManagementLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <SidebarProvider>
      <AppSidebar />
      <main className="flex min-h-svh w-full min-w-0 flex-1 flex-col">
        <header className="flex items-center justify-between gap-3 border-b p-3">
          <div className="flex items-center gap-3">
            <div className="flex items-center justify-center overflow-hidden rounded-xl">
              <Image
                src="/AMS_logo.svg"
                alt="AMS Logo"
                width={36}
                height={36}
                className="rounded-lg object-cover"
              />
            </div>

            <div>
              <h6 className="font-en text-xs font-semibold uppercase tracking-[0.14em] text-muted-foreground sm:text-sm">Asset Management System</h6>
              <p className="font-en text-xs">CPE Department</p>
            </div>

          </div>
          <div className="flex-1 px-2">
            <Input className="mx-auto max-w-md" />
          </div>
          <div>
            <ThemeToggle />
          </div>
          
        </header>
        <section className="flex min-h-0 flex-1 flex-col">
          <div className="px-3 py-2">
            <SidebarTrigger />
          </div>
          {children}
        </section>
        <footer>

        </footer>
      </main>
    </SidebarProvider>
  )
}
