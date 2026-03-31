"use client";

import { useEffect } from "react";
import Image from "next/image";
import { useRouter } from "next/navigation";

import { AppSidebar } from "@/components/app-sidebar";
import { ThemeToggle } from "@/components/theme-toggle";
import { Spinner } from "@/components/ui/spinner";
import { Input } from "@/components/ui/input";
import { SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar";
import { useAuth } from "@/context/AuthContext";

export function AssetManagementShell({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const { session, isLoading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!isLoading && !session) {
      router.replace("/login");
    }
  }, [isLoading, router, session]);

  if (isLoading || !session) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-background">
        <Spinner className="size-5" />
      </div>
    );
  }

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
              <h6 className="font-en text-xs font-semibold uppercase tracking-[0.14em] text-muted-foreground sm:text-sm">
                Asset Management System
              </h6>
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
        <footer></footer>
      </main>
    </SidebarProvider>
  );
}
