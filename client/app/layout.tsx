import type { Metadata } from "next";

import { ThemeProvider } from "@/components/theme-provider";
import { Toaster } from "@/components/ui/sonner";

import "./globals.css";
import { AuthProvider } from "../context/AuthContext";
import { ParameterProvider } from "../context/ParameterContext";
import {
  appMono,
  englishSans,
  promptSans,
  sarabunSans,
} from "./fonts";

export const metadata: Metadata = {
  title: "Asset Management System",
  description: "Asset Management",
  icons: {
    icon: "/icon.svg",
    shortcut: "/icon.svg",
    apple: "/icon.svg",
  },
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body
        className={`${englishSans.variable} ${promptSans.variable} ${sarabunSans.variable} ${appMono.variable} min-h-[100dvh] bg-background text-foreground antialiased font-en`}
      >
        <ThemeProvider>
          <AuthProvider>
            <ParameterProvider>
              {children}
              <Toaster />
            </ParameterProvider>
          </AuthProvider>
        </ThemeProvider>
      </body>
    </html>
  );
}
