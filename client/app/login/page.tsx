"use client";

import { useEffect, useState } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import Image from "next/image";
import { useRouter } from "next/navigation";
import { Controller, useForm } from "react-hook-form";

import { ThemeToggle } from "@/components/theme-toggle";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Field,
  FieldError,
  FieldGroup,
  FieldLabel,
} from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import {
  LoginFormSchema,
  type LoginFormValues,
} from "@/schema/LoginForm";
import { useAuth } from "../../context/AuthContext";
import { Spinner } from "@/components/ui/spinner";

export default function LoginPage() {
  const { login, session, isLoading } = useAuth();
  const router = useRouter();
  const [authError, setAuthError] = useState("");

  const form = useForm<LoginFormValues>({
    resolver: zodResolver(LoginFormSchema),
    mode: "onBlur",
    defaultValues: {
      username: "",
      password: "",
    },
  });

  useEffect(() => {
    if (!isLoading && session) {
      router.replace("/assetManagement/dashboard");
    }
  }, [session, isLoading, router]);

  const onSubmit = async (values: LoginFormValues) => {
    setAuthError("");

    const ok = await login(values.username, values.password);
    if (!ok) {
      setAuthError("Invalid username/email or password. Please try again.");
    }
  };

  if (isLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-100 dark:bg-slate-950">
        <Spinner />
      </div>
    );
  }

  return (
    <main className="min-h-[100dvh] bg-background text-foreground">
      <div className="mx-auto flex min-h-[100dvh] w-full max-w-7xl flex-col px-4 py-4 sm:px-6 lg:px-10">
        <header className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="flex h-10 w-10 items-center justify-center overflow-hidden rounded-xl">
              <Image
                src="/AMS_logo.svg"
                alt="AMS Logo"
                width={36}
                height={36}
                className="rounded-lg object-cover"
              />
            </div>
            <p className="font-en text-xs font-semibold uppercase tracking-[0.14em] text-muted-foreground sm:text-sm">
              Asset Management System
            </p>
          </div>
          <ThemeToggle />
        </header>

        <section className="flex flex-1 items-center justify-center py-6 sm:py-10 xl:py-12">
          <div className="grid w-full max-w-6xl overflow-hidden rounded-2xl border border-border bg-card shadow-sm lg:min-h-[620px] lg:grid-cols-[1.15fr_1fr] xl:grid-cols-[1.25fr_1fr] shadow-2xl">
            <aside className="relative hidden overflow-hidden bg-gradient-to-br from-sky-600 via-blue-600 to-indigo-700 p-10 text-white lg:flex lg:flex-col lg:justify-between xl:p-12">
              <div className="space-y-8">
                <p className="font-en text-sm font-medium uppercase tracking-[0.18em] text-white/80">
                  Asset Management
                </p>
                <h1 className="font-en mt-3 text-3xl font-semibold leading-tight xl:text-4xl">
                  Welcome back
                </h1>
                <p className="font-en mt-4 text-sm text-white/85 xl:max-w-md xl:text-base">
                  Sign in to manage assets, requests, and approvals in one place.
                </p>
                <div className="bg-white p-3 rounded-xl shadow-2xl">
                  <Image
                    src="/en_com.png"
                    alt="CPE_logo"
                    width="400"
                    height="107"
                  />
                  
                </div>
                
              </div>
              <p className="font-en text-xs text-white/75">
                Srinakharinwirot University Computer Engineering Department Asset Platform
              </p>
            </aside>

            <div className="flex items-center p-6 sm:p-8 lg:p-10 xl:p-12">
              <Card className="w-full border border-border/70 bg-background/90 shadow-none">
                <CardHeader className="gap-2">
                  <p className="font-en text-xs font-semibold uppercase tracking-[0.12em] text-muted-foreground lg:hidden">
                    Asset Management
                  </p>
                  <CardTitle className="font-en text-2xl sm:text-3xl">
                    Login
                  </CardTitle>
                  <CardDescription className="space-y-1">
                    <p className="font-en">
                      Enter your username or email and password.
                    </p>
                  </CardDescription>
                </CardHeader>

                <CardContent>
                  <form
                    noValidate
                    onSubmit={form.handleSubmit(onSubmit)}
                    className="space-y-5"
                  >
                    {authError && (
                      <div className="rounded-lg border border-destructive/40 bg-destructive/10 px-4 py-3 text-sm text-destructive">
                        {authError}
                      </div>
                    )}

                    <FieldGroup>
                      <Controller
                        name="username"
                        control={form.control}
                        render={({ field, fieldState }) => (
                          <Field data-invalid={fieldState.invalid}>
                            <FieldLabel htmlFor="login-username" className="font-en">
                              Username or email
                            </FieldLabel>
                            <Input
                              {...field}
                              id="login-username"
                              type="text"
                              autoComplete="username"
                              placeholder="username / email"
                              className="h-11"
                              aria-invalid={fieldState.invalid}
                            />
                            {fieldState.invalid && (
                              <FieldError className="font-en" errors={[fieldState.error]} />
                            )}
                          </Field>
                        )}
                      />

                      <Controller
                        name="password"
                        control={form.control}
                        render={({ field, fieldState }) => (
                          <Field data-invalid={fieldState.invalid}>
                            <FieldLabel htmlFor="login-password" className="font-en">
                              Password
                            </FieldLabel>
                            <Input
                              {...field}
                              id="login-password"
                              type="password"
                              autoComplete="current-password"
                              placeholder="password"
                              className="h-11"
                              aria-invalid={fieldState.invalid}
                            />
                            {fieldState.invalid && (
                              <FieldError className="font-en" errors={[fieldState.error]} />
                            )}
                          </Field>
                        )}
                      />
                    </FieldGroup>

                    <Button
                      type="submit"
                      size="lg"
                      className="font-en h-11 w-full"
                      disabled={form.formState.isSubmitting}
                    >
                      {form.formState.isSubmitting ? "Signing in..." : "Login"}
                    </Button>
                  </form>
                </CardContent>
              </Card>
            </div>
          </div>
        </section>

        <footer className="py-2 text-center text-xs text-muted-foreground sm:text-sm">
          Built for classroom and department asset workflows
        </footer>
      </div>
    </main>
  );
}
