"use client";

import { createContext, useContext, useMemo } from "react";

interface ParameterContextValue {
  isReady: boolean;
}

const ParameterContext = createContext<ParameterContextValue | null>(null);

export function ParameterProvider({ children }: { children: React.ReactNode }) {
  const value = useMemo<ParameterContextValue>(() => ({ isReady: true }), []);
  return (
    <ParameterContext.Provider value={value}>{children}</ParameterContext.Provider>
  );
}

export function useParameter(): ParameterContextValue {
  const ctx = useContext(ParameterContext);
  if (!ctx) throw new Error("useParameter must be used within <ParameterProvider>");
  return ctx;
}
