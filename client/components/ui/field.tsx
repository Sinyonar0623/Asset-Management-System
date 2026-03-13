import * as React from "react";
import type { FieldError as ReactHookFormFieldError } from "react-hook-form";

import { cn } from "@/lib/utils";

function Field({
  className,
  orientation = "vertical",
  ...props
}: React.ComponentProps<"div"> & {
  orientation?: "vertical" | "horizontal";
}) {
  return (
    <div
      data-slot="field"
      className={cn(
        "space-y-2",
        orientation === "horizontal" && "flex items-center justify-between gap-3",
        className
      )}
      {...props}
    />
  );
}

function FieldGroup({
  className,
  ...props
}: React.ComponentProps<"div">) {
  return (
    <div
      data-slot="field-group"
      className={cn("space-y-5", className)}
      {...props}
    />
  );
}

function FieldLabel({
  className,
  ...props
}: React.ComponentProps<"label">) {
  return (
    <label
      data-slot="field-label"
      className={cn("block text-sm font-medium text-foreground/90", className)}
      {...props}
    />
  );
}

function FieldDescription({
  className,
  ...props
}: React.ComponentProps<"p">) {
  return (
    <p
      data-slot="field-description"
      className={cn("text-sm text-muted-foreground", className)}
      {...props}
    />
  );
}

function FieldError({
  className,
  errors,
}: {
  className?: string;
  errors?: Array<ReactHookFormFieldError | undefined>;
}) {
  const messages = errors
    ?.map((error) => error?.message)
    .filter((message): message is string => Boolean(message));

  if (!messages?.length) {
    return null;
  }

  return (
    <div data-slot="field-error" className={cn("space-y-1", className)}>
      {messages.map((message) => (
        <p key={message} className="text-sm text-destructive">
          {message}
        </p>
      ))}
    </div>
  );
}

export { Field, FieldDescription, FieldError, FieldGroup, FieldLabel };
