"use client";

import {
  createContext,
  useContext,
  useEffect,
  useState,
  useCallback,
  useMemo,
  useRef,
} from "react";
import { useRouter } from "next/navigation";
import { loginApi, logoutApi, mapRoleCodeToRole, refreshSessionApi } from "../lib/auth-api";
import {
  AuthSession,
  AUTH_SESSION_COOKIE_KEY,
  LOCAL_STORAGE_KEY,
  MOCK_USERS,
} from "../lib/auth";

interface AuthContextValue {
  session: AuthSession | null;
  isLoading: boolean;
  login: (usernameOrEmail: string, password: string) => Promise<boolean>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

const TOKEN_REFRESH_LEAD_MS = 2 * 60 * 1000;
const TOKEN_REFRESH_CHECK_MS = 30 * 1000;
const ACTIVE_WINDOW_MS = 30 * 60 * 1000;

function getHomePath(role: AuthSession["role"]) {
  return role === "student" ? "/assetManagement/requests" : "/assetManagement/dashboard"
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [session, setSession] = useState<AuthSession | null>(null);
  const [isLoading, setLoading] = useState(true);
  const lastActivityAtRef = useRef(Date.now());
  const isRefreshingRef = useRef(false);
  const router = useRouter();

  const clearSessionCookie = useCallback(() => {
    document.cookie = `${AUTH_SESSION_COOKIE_KEY}=; path=/; max-age=0; samesite=lax`;
  }, []);

  const clearLocalSession = useCallback(() => {
    localStorage.removeItem(LOCAL_STORAGE_KEY);
    clearSessionCookie();
    setSession(null);
  }, [clearSessionCookie]);

  const setSessionCookie = useCallback(
    (expiresAt?: number) => {
      if (!expiresAt) {
        clearSessionCookie();
        return;
      }

      const maxAge = Math.max(Math.floor((expiresAt - Date.now()) / 1000), 0);
      if (maxAge <= 0) {
        clearSessionCookie();
        return;
      }

      document.cookie = `${AUTH_SESSION_COOKIE_KEY}=1; path=/; max-age=${maxAge}; samesite=lax`;
    },
    [clearSessionCookie]
  );

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
          clearLocalSession();
        } else {
          setSession(parsed);
          setSessionCookie(parsed.expiresAt);
        }
      } else {
        clearSessionCookie();
      }
    } catch {
      clearLocalSession();
    } finally {
      setLoading(false);
    }
  }, [clearLocalSession, clearSessionCookie, setSessionCookie]);

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
        setSessionCookie(newSession.expiresAt);
        setSession(newSession);
        router.push(getHomePath(newSession.role));
        return true;
      } catch {
        return false;
      }
    },
    [resolveEmailFromIdentifier, router, setSessionCookie]
  );

  const refreshSession = useCallback(async () => {
    if (isRefreshingRef.current) return;

    isRefreshingRef.current = true;
    try {
      const response = await refreshSessionApi();
      const role = mapRoleCodeToRole(response.roleCode);
      const refreshedSession: AuthSession = {
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

      localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(refreshedSession));
      setSessionCookie(refreshedSession.expiresAt);
      setSession(refreshedSession);
    } catch {
      clearLocalSession();
      router.push("/login");
    } finally {
      isRefreshingRef.current = false;
    }
  }, [clearLocalSession, router, setSessionCookie]);

  const logout = useCallback(() => {
    const doLogout = async () => {
      try {
        await logoutApi();
      } catch {
        // Ignore API logout failure and still clear local session.
      } finally {
        clearLocalSession();
        router.push("/login");
      }
    };

    void doLogout();
  }, [clearLocalSession, router]);

  useEffect(() => {
    if (!session) return;

    const markActive = () => {
      lastActivityAtRef.current = Date.now();
    };

    const activityEvents = ["pointerdown", "keydown", "scroll", "focus", "visibilitychange"];
    activityEvents.forEach((eventName) => {
      window.addEventListener(eventName, markActive, { passive: true });
    });

    return () => {
      activityEvents.forEach((eventName) => {
        window.removeEventListener(eventName, markActive);
      });
    };
  }, [session]);

  useEffect(() => {
    if (!session?.expiresAt) return;

    const maybeRefreshOrLogout = () => {
      const now = Date.now();
      const timeUntilExpiry = session.expiresAt! - now;

      if (timeUntilExpiry > TOKEN_REFRESH_LEAD_MS) return;

      const recentlyActive = now - lastActivityAtRef.current <= ACTIVE_WINDOW_MS;
      const tabVisible =
        typeof document === "undefined" || document.visibilityState === "visible";

      if (recentlyActive || tabVisible) {
        void refreshSession();
        return;
      }

      if (timeUntilExpiry <= 0) {
        logout();
      }
    };

    maybeRefreshOrLogout();
    const intervalId = window.setInterval(maybeRefreshOrLogout, TOKEN_REFRESH_CHECK_MS);

    return () => window.clearInterval(intervalId);
  }, [logout, refreshSession, session?.expiresAt]);

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
