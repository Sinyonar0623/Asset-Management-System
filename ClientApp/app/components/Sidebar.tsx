"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAuth } from "../contexts/AuthContext";
import { Role, ROLE_LABELS } from "../lib/auth";

interface NavItem {
  href: string;
  label: string;
  icon: string;
  allowedRoles: Role[] | null; // null = all authenticated roles
}

interface NavGroup {
  group: string;
  items: NavItem[];
}

const navItems: NavGroup[] = [
  {
    group: "MAIN",
    items: [
      { href: "/dashboard", label: "Dashboard",        icon: "📊", allowedRoles: null },
      { href: "/assets",    label: "Asset Management", icon: "📦", allowedRoles: null },
    ],
  },
  {
    group: "REQUESTS",
    items: [
      { href: "/borrow",    label: "Borrow Request", icon: "📤", allowedRoles: null },
      { href: "/repair",    label: "Repair Request", icon: "🔧", allowedRoles: ["lecturer", "depthead", "admin"] },
      { href: "/approvals", label: "Approvals",      icon: "✅", allowedRoles: ["lecturer", "depthead", "admin"] },
    ],
  },
  {
    group: "ADMIN",
    items: [
      { href: "/users",   label: "User Management", icon: "👥", allowedRoles: ["admin"] },
      { href: "/reports", label: "Reports",          icon: "📈", allowedRoles: ["depthead", "admin"] },
    ],
  },
];

export default function Sidebar() {
  const pathname = usePathname();
  const { session, logout } = useAuth();

  if (!session) return null;

  const userRole = session.role;
  const initials = session.name.charAt(0).toUpperCase();

  return (
    <aside className="w-64 min-h-screen bg-slate-800 text-slate-300 flex flex-col shrink-0">
      {/* Logo */}
      <div className="px-6 py-5 border-b border-slate-700">
        <div className="flex items-center gap-3">
          <div className="w-9 h-9 bg-blue-500 rounded-lg flex items-center justify-center text-white font-bold text-sm shrink-0">
            AM
          </div>
          <div>
            <div className="text-white font-semibold text-sm leading-tight">Asset Management</div>
            <div className="text-slate-400 text-xs">CE Department</div>
          </div>
        </div>
      </div>

      {/* Nav — filtered by role */}
      <nav className="flex-1 px-3 py-4 space-y-6 overflow-y-auto">
        {navItems.map((group) => {
          const visibleItems = group.items.filter(
            (item) =>
              item.allowedRoles === null ||
              item.allowedRoles.includes(userRole)
          );
          if (visibleItems.length === 0) return null;

          return (
            <div key={group.group}>
              <div className="text-xs font-semibold text-slate-500 uppercase tracking-wider px-3 mb-2">
                {group.group}
              </div>
              <ul className="space-y-1">
                {visibleItems.map((item) => {
                  const active =
                    pathname === item.href ||
                    pathname.startsWith(item.href + "/");
                  return (
                    <li key={item.href}>
                      <Link
                        href={item.href}
                        className={`flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
                          active
                            ? "bg-blue-600 text-white"
                            : "text-slate-300 hover:bg-slate-700 hover:text-white"
                        }`}
                      >
                        <span className="text-base">{item.icon}</span>
                        {item.label}
                      </Link>
                    </li>
                  );
                })}
              </ul>
            </div>
          );
        })}
      </nav>

      {/* User info + logout */}
      <div className="px-4 py-4 border-t border-slate-700">
        <div className="mb-2">
          <span className="text-xs px-2 py-0.5 rounded-full bg-slate-700 text-slate-300 font-medium">
            {ROLE_LABELS[userRole]}
          </span>
        </div>
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center text-sm font-semibold text-white shrink-0">
            {initials}
          </div>
          <div className="flex-1 min-w-0">
            <div className="text-sm font-medium text-white truncate">{session.name}</div>
            <div className="text-xs text-slate-400 truncate">{session.email}</div>
          </div>
          <button
            onClick={logout}
            title="ออกจากระบบ"
            className="text-slate-400 hover:text-white hover:bg-slate-700 transition-colors p-1.5 rounded-lg"
          >
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
            </svg>
          </button>
        </div>
      </div>
    </aside>
  );
}
