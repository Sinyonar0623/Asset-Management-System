import * as z from "zod";

export const LoginFormSchema = z.object({
  username: z.string().trim().min(1, "Username/Email is required"),
  password: z.string().min(1, "Password is required"),
});

export type LoginFormValues = z.infer<typeof LoginFormSchema>;
