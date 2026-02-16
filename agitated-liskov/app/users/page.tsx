"use client";
import { useState } from "react";
import MainLayout from "../components/MainLayout";
import Header from "../components/Header";

const users = [
  { id: "USR-001", name: "นายสมชาย ใจดี", email: "somchai@student.ku.ac.th", role: "Student", studentId: "6410400001", status: "Active", joinDate: "2021-06-01" },
  { id: "USR-002", name: "นางสาวสุดา มีทอง", email: "suda@student.ku.ac.th", role: "Student", studentId: "6410400025", status: "Active", joinDate: "2021-06-01" },
  { id: "USR-003", name: "ผศ.ดร.วิชัย สุขใจ", email: "wichai@ku.ac.th", role: "Lecturer", studentId: "—", status: "Active", joinDate: "2015-06-01" },
  { id: "USR-004", name: "รศ.ดร.สมศักดิ์ ดีงาม", email: "somsak@ku.ac.th", role: "Department Head", studentId: "—", status: "Active", joinDate: "2010-01-01" },
  { id: "USR-005", name: "นางรัตนา ช่วยดี", email: "rattana@ku.ac.th", role: "Admin", studentId: "—", status: "Active", joinDate: "2018-03-01" },
  { id: "USR-006", name: "นายประสิทธิ์ เก่งมาก", email: "prasit@student.ku.ac.th", role: "Student", studentId: "6410400088", status: "Inactive", joinDate: "2021-06-01" },
];

const roleColors: Record<string, string> = {
  Student: "bg-blue-100 text-blue-700",
  Lecturer: "bg-purple-100 text-purple-700",
  Admin: "bg-orange-100 text-orange-700",
  "Department Head": "bg-red-100 text-red-700",
  Staff: "bg-teal-100 text-teal-700",
};

export default function UsersPage() {
  const [search, setSearch] = useState("");
  const [filterRole, setFilterRole] = useState("All");

  const filtered = users.filter((u) => {
    const matchSearch = u.name.toLowerCase().includes(search.toLowerCase()) ||
      u.email.toLowerCase().includes(search.toLowerCase());
    const matchRole = filterRole === "All" || u.role === filterRole;
    return matchSearch && matchRole;
  });

  return (
    <MainLayout>
      <Header
        title="User Management"
        subtitle={`${users.length} users in the system`}
        actions={
          <button className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 transition-colors">
            <span>+</span> Add User
          </button>
        }
      />

      <div className="flex-1 p-6 space-y-4">
        {/* Role Stats */}
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
          {[
            { label: "Students", count: users.filter(u => u.role === "Student").length, color: "blue" },
            { label: "Lecturers", count: users.filter(u => u.role === "Lecturer").length, color: "purple" },
            { label: "Admins", count: users.filter(u => u.role === "Admin").length, color: "orange" },
            { label: "Dept. Heads", count: users.filter(u => u.role === "Department Head").length, color: "red" },
          ].map((s) => (
            <div key={s.label} className="bg-white rounded-lg border border-slate-200 p-4 text-center shadow-sm">
              <div className="text-2xl font-bold text-slate-800">{s.count}</div>
              <div className="text-sm text-slate-500">{s.label}</div>
            </div>
          ))}
        </div>

        {/* Filters */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm p-4">
          <div className="flex flex-wrap gap-3">
            <div className="flex-1 min-w-48">
              <div className="relative">
                <svg className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
                <input
                  type="text"
                  placeholder="Search by name or email..."
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                  className="w-full pl-9 pr-4 py-2 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
            </div>
            <select
              value={filterRole}
              onChange={(e) => setFilterRole(e.target.value)}
              className="px-3 py-2 text-sm border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
            >
              <option value="All">All Roles</option>
              <option>Student</option>
              <option>Lecturer</option>
              <option>Admin</option>
              <option>Department Head</option>
              <option>Staff</option>
            </select>
          </div>
        </div>

        {/* Table */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="bg-slate-50 border-b border-slate-200 text-left">
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">User</th>
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Email</th>
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Role</th>
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Student ID</th>
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Status</th>
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Join Date</th>
                  <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {filtered.map((user) => (
                  <tr key={user.id} className="hover:bg-slate-50 transition-colors">
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-3">
                        <div className="w-8 h-8 bg-slate-200 rounded-full flex items-center justify-center text-sm font-semibold text-slate-600">
                          {user.name.charAt(0)}
                        </div>
                        <div>
                          <div className="font-medium text-slate-800">{user.name}</div>
                          <div className="text-xs text-slate-400">{user.id}</div>
                        </div>
                      </div>
                    </td>
                    <td className="px-4 py-3 text-slate-600 text-xs">{user.email}</td>
                    <td className="px-4 py-3">
                      <span className={`px-2.5 py-1 rounded-full text-xs font-medium ${roleColors[user.role] || "bg-slate-100 text-slate-600"}`}>
                        {user.role}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-slate-500 font-mono text-xs">{user.studentId}</td>
                    <td className="px-4 py-3">
                      <span className={`px-2 py-0.5 rounded-full text-xs font-medium ${user.status === "Active" ? "bg-green-100 text-green-700" : "bg-slate-100 text-slate-500"}`}>
                        {user.status}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-slate-500 text-xs">{user.joinDate}</td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-2">
                        <button className="text-xs text-blue-600 hover:underline">Edit</button>
                        <button className="text-xs text-slate-400 hover:underline">
                          {user.status === "Active" ? "Deactivate" : "Activate"}
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </MainLayout>
  );
}
