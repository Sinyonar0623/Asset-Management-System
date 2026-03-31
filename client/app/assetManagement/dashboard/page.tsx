"use client";

import { useAuth } from "../../../context/AuthContext";

export default function DashboardPage() {
  const { session } = useAuth();

  const handleLogSession = () => {
    console.log("session ", session);
  };

  return (
    <main className="px-3">
      <section className="flex flex-row gap-8">
        <div className="grid grid-cols-12 gap-3">
          <div className="col-span-4"></div>
          <div className="col-span-4"></div>
          <div className="col-span-4"></div>
        </div>
      </section>
    </main>
  );
}
