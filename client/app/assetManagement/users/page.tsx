"use client"

import * as React from "react"
import { Loader2Icon, SaveIcon, SearchIcon, UserPlusIcon } from "lucide-react"

import { DataTableShell, TableEmptyState } from "@/components/ce-ams/data-table-shell"
import { PageHeader } from "@/components/ce-ams/page-header"
import { RoleBadge } from "@/components/ce-ams/role-badge"
import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { toast } from "@/components/ui/sonner"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { useAuth } from "@/context/AuthContext"
import { createUser, getUsers, updateUserRole, type UserDto } from "@/lib/ce-ams-api"

const roleOptions = [
  { value: "STUDENT", label: "Student" },
  { value: "TEACHER", label: "Teacher" },
  { value: "HOD", label: "HOD" },
  { value: "ADMIN", label: "Admin" },
]

function AddUserDialog({ onCreated }: { onCreated: () => void }) {
  const [open, setOpen] = React.useState(false)
  const [username, setUsername] = React.useState("")
  const [email, setEmail] = React.useState("")
  const [password, setPassword] = React.useState("")
  const [roleCode, setRoleCode] = React.useState("STUDENT")
  const [isSubmitting, setSubmitting] = React.useState(false)

  async function submit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSubmitting(true)
    try {
      await createUser({ username, email, password, roleCode })
      toast("User created")
      setOpen(false)
      setUsername("")
      setEmail("")
      setPassword("")
      setRoleCode("STUDENT")
      onCreated()
    } catch {
      toast({
        title: "User could not be created",
        description: "Please confirm the signup API and your permissions.",
        variant: "destructive",
      })
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button>
          <UserPlusIcon className="size-4" />
          Add User
        </Button>
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Add User</DialogTitle>
          <DialogDescription>Create a user with the backend signup API.</DialogDescription>
        </DialogHeader>
        <form className="space-y-4" onSubmit={submit}>
          <div className="grid grid-cols-2 gap-4">
            <label className="space-y-2">
              <span className="text-sm font-medium">Name</span>
              <Input value={username} onChange={(event) => setUsername(event.target.value)} required />
            </label>
            <label className="space-y-2">
              <span className="text-sm font-medium">Role</span>
              <Select value={roleCode} onValueChange={setRoleCode}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {roleOptions.map((role) => (
                    <SelectItem key={role.value} value={role.value}>
                      {role.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </label>
          </div>
          <label className="space-y-2">
            <span className="text-sm font-medium">Email</span>
            <Input type="email" value={email} onChange={(event) => setEmail(event.target.value)} required />
          </label>
          <label className="space-y-2">
            <span className="text-sm font-medium">Temporary password</span>
            <Input type="password" value={password} onChange={(event) => setPassword(event.target.value)} required />
          </label>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => setOpen(false)}>
              Cancel
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? "Creating..." : "Create"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}

export default function UserManagementPage() {
  const { session } = useAuth()
  const [users, setUsers] = React.useState<UserDto[]>([])
  const [selectedRoleByUser, setSelectedRoleByUser] = React.useState<Record<string, string>>({})
  const [search, setSearch] = React.useState("")
  const [roleFilter, setRoleFilter] = React.useState("ALL")
  const [isLoading, setLoading] = React.useState(true)
  const [savingUserId, setSavingUserId] = React.useState<string | null>(null)

  const isAdmin = session?.role === "admin"

  const loadUsers = React.useCallback(() => {
    if (!isAdmin) {
      setUsers([])
      setLoading(false)
      return
    }

    setLoading(true)
    getUsers()
      .then((items) => {
        setUsers(items)
        setSelectedRoleByUser(
          Object.fromEntries(items.map((user) => [user.userId, user.roleCode]))
        )
      })
      .catch(() => {
        toast({
          title: "Users could not be loaded",
          description: "Please confirm the user list API and your permissions.",
          variant: "destructive",
        })
      })
      .finally(() => setLoading(false))
  }, [isAdmin])

  React.useEffect(() => {
    loadUsers()
  }, [loadUsers])

  const visibleUsers = users.filter((user) => {
    const target = `${user.username} ${user.email} ${user.roleCode}`.toLowerCase()
    const matchesSearch = target.includes(search.toLowerCase())
    const matchesRole = roleFilter === "ALL" || user.roleCode === roleFilter
    return matchesSearch && matchesRole
  })

  function selectRole(userId: string, roleCode: string) {
    setSelectedRoleByUser((current) => ({
      ...current,
      [userId]: roleCode,
    }))
  }

  async function saveRole(user: UserDto) {
    const selectedRole = selectedRoleByUser[user.userId]
    if (!selectedRole || selectedRole === user.roleCode) return

    setSavingUserId(user.userId)
    try {
      const updatedUser = await updateUserRole(user.userId, selectedRole)
      setUsers((current) =>
        current.map((item) => {
          if (item.userId === updatedUser.userId) return updatedUser
          if (updatedUser.roleCode === "HOD" && item.roleCode === "HOD") {
            return {
              ...item,
              roleCode: "TEACHER",
              roleName: "TEACHER",
            }
          }
          return item
        })
      )
      setSelectedRoleByUser((current) => {
        const next = { ...current, [updatedUser.userId]: updatedUser.roleCode }
        if (updatedUser.roleCode === "HOD") {
          for (const item of users) {
            if (item.userId !== updatedUser.userId && item.roleCode === "HOD") {
              next[item.userId] = "TEACHER"
            }
          }
        }
        return next
      })
      toast("Role updated")
    } catch {
      toast({
        title: "Role could not be updated",
        description: "Assign another user as HOD before changing the current HOD role.",
        variant: "destructive",
      })
    } finally {
      setSavingUserId(null)
    }
  }

  return (
    <div className="space-y-6">
      <PageHeader
        title="User Management"
        subtitle="Create users and manage system roles."
        actions={isAdmin ? <AddUserDialog onCreated={loadUsers} /> : null}
      />

      <div className="flex items-center justify-between gap-4">
        <div className="relative w-[360px]">
          <SearchIcon className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            className="pl-9"
            placeholder="Search users"
            disabled={!isAdmin || isLoading}
          />
        </div>
        <Select value={roleFilter} onValueChange={setRoleFilter} disabled={!isAdmin || isLoading}>
          <SelectTrigger className="w-44">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="ALL">All roles</SelectItem>
            {roleOptions.map((role) => (
              <SelectItem key={role.value} value={role.value}>
                {role.label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <DataTableShell>
        {!isAdmin ? (
          <TableEmptyState
            title="Admin access required"
            description="User role management is available for admin accounts."
          />
        ) : visibleUsers.length === 0 ? (
          <TableEmptyState
            title={isLoading ? "Loading users" : "No users found"}
            description="Users returned by GET /auth/users will appear here."
          />
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Name</TableHead>
                <TableHead>Email</TableHead>
                <TableHead>Current Role</TableHead>
                <TableHead>New Role</TableHead>
                <TableHead className="text-right">Action</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {visibleUsers.map((user) => {
                const selectedRole = selectedRoleByUser[user.userId] ?? user.roleCode
                const canSave =
                  selectedRole !== user.roleCode &&
                  savingUserId !== user.userId

                return (
                  <TableRow key={user.userId}>
                    <TableCell>
                      <p className="font-semibold text-foreground">{user.username}</p>
                    </TableCell>
                    <TableCell className="text-muted-foreground">{user.email}</TableCell>
                    <TableCell>
                      <RoleBadge role={user.roleCode} />
                    </TableCell>
                    <TableCell>
                      <Select
                        value={selectedRole}
                        onValueChange={(roleCode) => {
                          if (user.roleCode === "HOD" && roleCode !== "HOD") {
                            toast({
                              title: "Current HOD cannot be removed",
                              description: "Assign another user as HOD to transfer the role.",
                              variant: "destructive",
                            })
                            selectRole(user.userId, "HOD")
                            return
                          }

                          selectRole(user.userId, roleCode)
                        }}
                        disabled={savingUserId === user.userId}
                      >
                        <SelectTrigger className="w-44">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          {roleOptions.map((role) => (
                            <SelectItem key={role.value} value={role.value}>
                              {role.label}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </TableCell>
                    <TableCell className="text-right">
                      <Button
                        type="button"
                        size="sm"
                        disabled={!canSave}
                        onClick={() => saveRole(user)}
                      >
                        {savingUserId === user.userId ? (
                          <Loader2Icon className="size-4 animate-spin" />
                        ) : (
                          <SaveIcon className="size-4" />
                        )}
                        Save
                      </Button>
                    </TableCell>
                  </TableRow>
                )
              })}
            </TableBody>
          </Table>
        )}
      </DataTableShell>
    </div>
  )
}
