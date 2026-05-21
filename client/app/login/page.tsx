"use client"

// API:
// POST /auth/login
// Returns: accessToken, tokenType, expiresIn, userId, username, email, roleCode, roleName

import { useEffect, useState } from "react"
import { zodResolver } from "@hookform/resolvers/zod"
import { LockKeyholeIcon, MailIcon } from "lucide-react"
import Image from "next/image"
import { useRouter } from "next/navigation"
import { Controller, useForm } from "react-hook-form"

import { ThemeToggle } from "@/components/theme-toggle"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field"
import { Input } from "@/components/ui/input"
import { Spinner } from "@/components/ui/spinner"
import { useAuth } from "@/context/AuthContext"
import { LoginFormSchema, type LoginFormValues } from "@/schema/LoginForm"

function getHomePath(role?: string) {
  return role === "student" ? "/assetManagement/requests" : "/assetManagement/dashboard"
}

export default function LoginPage() {
  const { login, session, isLoading } = useAuth()
  const router = useRouter()
  const [authError, setAuthError] = useState("")

  const form = useForm<LoginFormValues>({
    resolver: zodResolver(LoginFormSchema),
    mode: "onBlur",
    defaultValues: {
      username: "",
      password: "",
    },
  })

  useEffect(() => {
    if (!isLoading && session) {
      router.replace(getHomePath(session.role))
    }
  }, [session, isLoading, router])

  async function onSubmit(values: LoginFormValues) {
    setAuthError("")

    const ok = await login(values.username, values.password)
    if (!ok) {
      setAuthError("Invalid username/email or password. Please try again.")
    }
  }

  if (isLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-background">
        <Spinner />
      </div>
    )
  }

  return (
    <main className="relative flex min-h-[100dvh] items-center justify-center bg-[linear-gradient(180deg,#ffffff_0%,#f8fafc_100%)] px-6 dark:bg-[linear-gradient(180deg,#0f172a_0%,#020617_100%)]">
      <div className="absolute right-6 top-6 z-10">
        <ThemeToggle />
      </div>
      <div className="absolute left-6 top-6 z-10 flex w-[380px] items-center justify-center rounded-[20px] border border-border bg-white px-5 py-3 shadow-sm dark:border-white/10">
        <Image
          src="/en_com.png"
          alt="Srinakharinwirot University Computer Engineering logo"
          width={340}
          height={91}
          priority
          className="h-auto w-full object-contain"
        />
      </div>
      <Card className="relative w-full max-w-[580px] px-6 py-9 dark:border-white/10 dark:bg-[#111827]">
        <CardHeader className="gap-5 text-center">
          <div className="mx-auto flex size-20 items-center justify-center overflow-hidden rounded-[24px] border border-border bg-white p-3 shadow-sm dark:border-white/10">
            <Image
              src="/AMS_logo.svg"
              alt="Asset Management System logo"
              width={56}
              height={56}
              priority
              className="size-14 object-contain"
            />
          </div>
          <div className="space-y-1">
            <CardTitle className="text-[38px]">AMS</CardTitle>
            <CardDescription className="text-base">
              Asset Management System
            </CardDescription>
          </div>
        </CardHeader>

        <CardContent>
          <form noValidate onSubmit={form.handleSubmit(onSubmit)} className="space-y-7">
            {authError && (
              <div className="rounded-[14px] border border-destructive/30 bg-destructive/10 px-4 py-3 text-sm font-medium text-destructive">
                {authError}
              </div>
            )}

            <FieldGroup>
              <Controller
                name="username"
                control={form.control}
                render={({ field, fieldState }) => (
                  <Field data-invalid={fieldState.invalid}>
                    <FieldLabel htmlFor="login-username" className="text-base">
                      Username/Email
                    </FieldLabel>
                    <div className="relative">
                      <MailIcon className="pointer-events-none absolute left-4 top-1/2 size-5 -translate-y-1/2 text-muted-foreground" />
                      <Input
                        {...field}
                        id="login-username"
                        type="text"
                        autoComplete="username"
                        placeholder="username or name@department.edu"
                        className="h-14 pl-12 text-base dark:border-white/10 dark:bg-[#0f172a]"
                        aria-invalid={fieldState.invalid}
                      />
                    </div>
                    {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                  </Field>
                )}
              />

              <Controller
                name="password"
                control={form.control}
                render={({ field, fieldState }) => (
                  <Field data-invalid={fieldState.invalid}>
                    <FieldLabel htmlFor="login-password" className="text-base">
                      Password
                    </FieldLabel>
                    <div className="relative">
                      <LockKeyholeIcon className="pointer-events-none absolute left-4 top-1/2 size-5 -translate-y-1/2 text-muted-foreground" />
                      <Input
                        {...field}
                        id="login-password"
                        type="password"
                        autoComplete="current-password"
                        placeholder="Enter password"
                        className="h-14 pl-12 text-base dark:border-white/10 dark:bg-[#0f172a]"
                        aria-invalid={fieldState.invalid}
                      />
                    </div>
                    {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                  </Field>
                )}
              />
            </FieldGroup>

            <Button
              type="submit"
              size="lg"
              className="h-14 w-full text-base"
              disabled={form.formState.isSubmitting}
            >
              {form.formState.isSubmitting ? "Signing in..." : "Login"}
            </Button>
          </form>
        </CardContent>
      </Card>
    </main>
  )
}
