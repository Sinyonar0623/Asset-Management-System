import { Geist_Mono, Inter, Prompt, Sarabun } from "next/font/google";

export const appMono = Geist_Mono({
  variable: "--font-app-mono",
  subsets: ["latin"],
});

export const promptSans = Prompt({
  subsets: ["latin", "thai"],
  weight: ["400", "500", "600", "700"],
  variable: "--font-prompt-sans",
});

export const sarabunSans = Sarabun({
  subsets: ["latin", "thai"],
  weight: ["400", "500", "600", "700"],
  variable: "--font-sarabun-sans",
});

export const englishSans = Inter({
  subsets: ["latin"],
  variable: "--font-english-sans",
});
