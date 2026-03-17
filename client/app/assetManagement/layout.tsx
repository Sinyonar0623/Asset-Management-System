import { redirect } from "next/navigation";
import { cookies } from "next/headers";
import { AUTH_SESSION_COOKIE_KEY } from "@/lib/auth";
import { AssetManagementShell } from "./AssetManagementShell";

export default async function AssetManagementLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const cookieStore = await cookies();
  if (!cookieStore.has(AUTH_SESSION_COOKIE_KEY)) {
    redirect("/login");
  }

  return <AssetManagementShell>{children}</AssetManagementShell>;
}
