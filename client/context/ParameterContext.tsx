"use client";

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";

import api from "../lib/api";
import { useAuth } from "./AuthContext";

interface ParameterItem {
  group: string;
  value: string;
  description: string;
  active: boolean;
}

interface ParameterOption {
  text: string;
  value: string;
}

interface PaginatedResponse<TItem> {
  items: TItem[];
  count: number;
  pageNumber: number;
  pageSize: number;
}

interface ParameterContextValue {
  parameters: ParameterItem[];
  isLoading: boolean;
  isReady: boolean;
  refreshParameters: () => Promise<void>;
  getGeneralCodeByGroup: (group: string) => ParameterOption[];
  getGeneralCodeDescription: (group: string, value: string) => string;
}

const ParameterContext = createContext<ParameterContextValue | null>(null);

const PAGE_SIZE = 200;

function normalize(value: string): string {
  return value.trim().toLowerCase();
}

export function ParameterProvider({ children }: { children: React.ReactNode }) {
  const { session, isLoading: isAuthLoading } = useAuth();
  const [parameters, setParameters] = useState<ParameterItem[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  const fetchPage = useCallback(async (pageNumber: number) => {
    const { data } = await api.get<PaginatedResponse<ParameterItem>>("/Parameters", {
      params: {
        pageNumber,
        pageSize: PAGE_SIZE,
      },
    });

    return data;
  }, []);

  const refreshParameters = useCallback(async () => {
    if (!session) {
      setParameters([]);
      return;
    }

    setIsLoading(true);

    try {
      const firstPage = await fetchPage(0);
      let items = firstPage.items ?? [];
      const totalCount = firstPage.count ?? items.length;
      const totalPages = Math.ceil(totalCount / PAGE_SIZE);

      if (totalPages > 1) {
        const responses = await Promise.all(
          Array.from({ length: totalPages - 1 }, (_, index) => fetchPage(index + 1))
        );

        items = items.concat(...responses.flatMap((response) => response.items ?? []));
      }

      setParameters(items);
    } finally {
      setIsLoading(false);
    }
  }, [fetchPage, session]);

  useEffect(() => {
    if (isAuthLoading) {
      return;
    }

    if (!session) {
      setParameters([]);
      return;
    }

    void refreshParameters();
  }, [isAuthLoading, refreshParameters, session]);

  const getGeneralCodeByGroup = useCallback(
    (group: string): ParameterOption[] =>
      parameters
        .filter(
          (parameter) =>
            parameter.active && normalize(parameter.group) === normalize(group)
        )
        .map((parameter) => ({
          text: parameter.description,
          value: parameter.value,
        })),
    [parameters]
  );

  const getGeneralCodeDescription = useCallback(
    (group: string, value: string): string =>
      parameters.find(
        (parameter) =>
          normalize(parameter.group) === normalize(group) && parameter.value === value
      )?.description ?? value,
    [parameters]
  );

  const value = useMemo<ParameterContextValue>(
    () => ({
      parameters,
      isLoading,
      isReady: !isAuthLoading && !isLoading,
      refreshParameters,
      getGeneralCodeByGroup,
      getGeneralCodeDescription,
    }),
    [
      getGeneralCodeByGroup,
      getGeneralCodeDescription,
      isAuthLoading,
      isLoading,
      parameters,
      refreshParameters,
    ]
  );

  return (
    <ParameterContext.Provider value={value}>{children}</ParameterContext.Provider>
  );
}

export function useParameter(): ParameterContextValue {
  const ctx = useContext(ParameterContext);
  if (!ctx) throw new Error("useParameter must be used within <ParameterProvider>");
  return ctx;
}
