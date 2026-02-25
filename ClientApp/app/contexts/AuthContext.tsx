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
import { AuthSession, LOCAL_STORAGE_KEY, rolecodeToRole } from "../lib/auth";
import { authApi, getToken, setToken, removeToken } from "../lib/api";

interface AuthContextValue {
  session: AuthSession | null;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<{ ok: boolean; error?: string }>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [session, setSession] = useState<AuthSession | null>(null);
  const [isLoading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    const token = getToken();
    if (token) {
      authApi.me()
        .then((me) => {
          const role = rolecodeToRole(me.roleCode);
          setSession({
            userId: me.userId,
            username: me.username,
            name: me.username,
            email: me.email || "",
            role,
            roleCode: me.roleCode,
            roleName: me.roleName,
          });
        })
        .catch(() => {
          removeToken();
          localStorage.removeItem(LOCAL_STORAGE_KEY);
        })
        .finally(() => setLoading(false));
    } else {
      try {
        const raw = localStorage.getItem(LOCAL_STORAGE_KEY);
        if (raw) {
          const parsed: AuthSession = JSON.parse(raw);
          setSession(parsed);
        }
      } catch {
        localStorage.removeItem(LOCAL_STORAGE_KEY);
      } finally {
        setLoading(false);
      }
    }
  }, []);

  const login = useCallback(
    async (email: string, password: string): Promise<{ ok: boolean; error?: string }> => {
      try {
        const res = await authApi.login(email, password);
        setToken(res.accessToken);

        const role = rolecodeToRole(res.roleCode);
        const newSession: AuthSession = {
          userId: res.userId,
          username: res.username,
          name: res.username,
          email: res.email,
          role,
          roleCode: res.roleCode,
          roleName: res.roleName,
        };
        localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(newSession));
        setSession(newSession);
        router.push("/dashboard");
        return { ok: true };
      } catch (err) {
        return { ok: false, error: (err as Error).message || "เข้าสู่ระบบไม่สำเร็จ" };
      }
    },
    [router]
  );

  const logout = useCallback(async () => {
    try {
      await authApi.logout();
    } catch {}
    removeToken();
    localStorage.removeItem(LOCAL_STORAGE_KEY);
    setSession(null);
    router.push("/login");
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
