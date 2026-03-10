"use client";

import {
  createContext,
  useContext,
  useEffect,
  useState,
  useCallback,
  useMemo,
} from "react";
import { useRouter } from "next/navigation";
import { loginApi, logoutApi, mapRoleCodeToRole } from "../lib/auth-api";
import { AuthSession, LOCAL_STORAGE_KEY, MOCK_USERS } from "../lib/auth";

interface AuthContextValue {
  session: AuthSession | null;
  isLoading: boolean;
  login: (usernameOrEmail: string, password: string) => Promise<boolean>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [session, setSession] = useState<AuthSession | null>(null);
  const [isLoading, setLoading] = useState(true);
  const router = useRouter();

  const resolveEmailFromIdentifier = useCallback((usernameOrEmail: string): string => {
    if (usernameOrEmail.includes("@")) return usernameOrEmail;
    const user = MOCK_USERS.find((u) => u.username === usernameOrEmail);
    return user?.email ?? usernameOrEmail;
  }, []);

  // Rehydrate from localStorage on mount (client-side only)
  useEffect(() => {
    try {
      const raw = localStorage.getItem(LOCAL_STORAGE_KEY);
      if (raw) {
        const parsed: AuthSession = JSON.parse(raw);
        if (parsed.expiresAt && parsed.expiresAt <= Date.now()) {
          localStorage.removeItem(LOCAL_STORAGE_KEY);
        } else {
          setSession(parsed);
        }
      }
    } catch {
      localStorage.removeItem(LOCAL_STORAGE_KEY);
    } finally {
      setLoading(false);
    }
  }, []);

  const login = useCallback(
    async (usernameOrEmail: string, password: string): Promise<boolean> => {
      try {
        const email = resolveEmailFromIdentifier(usernameOrEmail.trim());
        const response = await loginApi({ email, password });
        const role = mapRoleCodeToRole(response.roleCode);

        const newSession: AuthSession = {
          userId: response.userId,
          username: response.username,
          name: response.username,
          email: response.email,
          role,
          department: "",
          ownedAssetIds: [],
          accessToken: response.accessToken,
          tokenType: response.tokenType,
          expiresAt: Date.now() + response.expiresIn * 1000,
        };

        localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(newSession));
        setSession(newSession);
        router.push("/dashboard");
        return true;
      } catch {
        return false;
      }
    },
    [resolveEmailFromIdentifier, router]
  );

  const logout = useCallback(() => {
    const doLogout = async () => {
      try {
        await logoutApi();
      } catch {
        // Ignore API logout failure and still clear local session.
      } finally {
        localStorage.removeItem(LOCAL_STORAGE_KEY);
        setSession(null);
        router.push("/login");
      }
    };

    void doLogout();
  }, [router]);

  const value = useMemo(
    () => ({ session, isLoading, login, logout }),
    [session, isLoading, login, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within <AuthProvider>");
  return ctx;
}
