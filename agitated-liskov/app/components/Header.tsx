"use client";

import { useAuth } from "../contexts/AuthContext";
import { ROLE_LABELS } from "../lib/auth";

interface HeaderProps {
  title: string;
  subtitle?: string;
  actions?: React.ReactNode;
}

export default function Header({ title, subtitle, actions }: HeaderProps) {
  const { session } = useAuth();

  const initials = session?.name?.charAt(0).toUpperCase() ?? "?";
  const displayName = session?.name?.split(" ").slice(-1)[0] ?? "User"; // last word (surname)

  return (
    <header className="bg-white border-b border-slate-200 px-6 py-4 flex items-center justify-between">
      <div>
        <h1 className="text-xl font-semibold text-slate-800">{title}</h1>
        {subtitle && <p className="text-sm text-slate-500 mt-0.5">{subtitle}</p>}
      </div>
      <div className="flex items-center gap-3">
        {/* Notification */}
        <button className="relative p-2 text-slate-500 hover:text-slate-700 hover:bg-slate-100 rounded-lg transition-colors">
          <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
          </svg>
          <span className="absolute top-1.5 right-1.5 w-2 h-2 bg-red-500 rounded-full"></span>
        </button>
        {/* User avatar — dynamic */}
        <div className="flex items-center gap-2">
          <div className="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center text-white text-sm font-semibold">
            {initials}
          </div>
          <div className="hidden sm:block text-right">
            <div className="text-sm font-medium text-slate-700 leading-tight">{displayName}</div>
            {session && (
              <div className="text-xs text-slate-400">{ROLE_LABELS[session.role]}</div>
            )}
          </div>
        </div>
        {actions}
      </div>
    </header>
  );
}
