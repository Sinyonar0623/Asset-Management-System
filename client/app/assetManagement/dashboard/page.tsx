"use client";

import { Button } from "@/components/ui/button";
import { useAuth } from "../../../context/AuthContext";

export default function DashboardPage() {
  const { session } = useAuth();

  const handleLogSession = () => {
    console.log("session ", session);
  };

  return (
    <main className="px-3">
      <Button onClick={handleLogSession}>Log session</Button>
    </main>
  );
}
