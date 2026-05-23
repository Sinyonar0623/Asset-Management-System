import api from "@/lib/api"

export type RequestStatus =
  | "PENDING"
  | "APPROVED"
  | "REJECTED"
  | "CANCELLED"
  | "COMPLETED"

export type TrackingStatus =
  | "WAITING"
  | "PENDING"
  | "APPROVED"
  | "REJECTED"
  | "SKIPPED"
  | "CANCELLED"

export type RoleCode = "STUDENT" | "TEACHER" | "HOD" | "ADMIN" | string

export type PaginatedResult<T> = {
  items: T[]
  count: number
  pageNumber: number
  pageSize: number
}

export type AssetDto = {
  id?: string | null
  name: string
  description: string
  category: string
  isAvailable?: boolean | null
  availabilityStatus?: string | null
  location?: string | null
  updatedAt?: string | null
}

export type AssetUnitDto = {
  id?: string | null
  assetId?: string | null
  assetTag: string
  serialNo: string
  name: string
  brand: string
  availabilityStatus: string
  operationalStatus: string
  remark: string
  responsibleUserId?: string | null
}

export type AssetUnitImageDto = {
  id: string
  assetUnitId: string
  imageUrl: string
  description?: string | null
  fileName?: string | null
  contentType?: string | null
  fileSizeBytes?: number | null
}

export type AssetHistoryDto = {
  assetUnitId: string
  actionType: string
  fromAvailabilityStatus?: string | null
  toAvailabilityStatus?: string | null
  fromOperationalStatus?: string | null
  toOperationalStatus?: string | null
  fromResponsibleUserId?: string | null
  toResponsibleUserId?: string | null
  performedBy: string
  performedAt: string
  approvedBy?: string | null
  approvedAt?: string | null
  referenceNo?: string | null
  requestId?: string | null
  remark: string
}

export type AssetUnitDetailDto = {
  unit: AssetUnitDto
  images: AssetUnitImageDto[]
  histories: AssetHistoryDto[]
}

export type LaboratoryDto = {
  id?: string | null
  laboratoryName?: string
  roomNo?: string
  description?: string
  teacherId?: string | null
}

export type TeacherDto = {
  teacherId: string
  username: string
}

export type UserDto = {
  userId: string
  username: string
  email: string
  roleCode: RoleCode
  roleName: string
}

export type UserLookupDto = {
  userId: string
  username: string
}

export type RequestDetailDto = {
  purpose?: string | null
  borrowFrom?: string | null
  borrowTo?: string | null
  issueDescription?: string | null
  retireReason?: string | null
  extraNote?: string | null
}

export type RequestItemDto = {
  assetId: string
  createdAt: string
  updatedAt: string
}

export type CreateRequestItemPayload = {
  assetId: string
}

export type RequestTrackingDto = {
  stepNo: number
  requiredRoleCode: RoleCode
  assignedApproverId?: string | null
  status: TrackingStatus | string
  actionByUserId?: string | null
  actionOn?: string | null
  comment?: string | null
  isCurrent: boolean
}

export type RequestDto = {
  id: string
  requestType: string
  targetLaboratoryId: string
  status: RequestStatus | string
  requesterId: string
  reason: string
  currentStepNo?: number | null
  nextApproverId?: string | null
  submittedOn: string
  finalizedOn?: string | null
  detail?: RequestDetailDto | null
  items: RequestItemDto[]
  trackings: RequestTrackingDto[]
}

export type ParameterDto = {
  group: string
  value: string
  description: string
  active: boolean
}

export type CreateRequestPayload = {
  requestType: string
  targetLaboratoryId: string
  reason: string
  detail?: RequestDetailDto | null
  items?: CreateRequestItemPayload[] | null
}

export type CreateAssetPayload = {
  asset: Pick<AssetDto, "name" | "description" | "category">
  units: string[]
}

export type CreateAssetUnitPayload = {
  assetUnits: Array<Omit<AssetUnitDto, "id">>
}

export type CreateUserPayload = {
  username: string
  email: string
  password: string
  roleCode: string
}

export type MarkProcessedPayload = {
  requestId: string
  decision: "APPROVE" | "REJECT"
  comment?: string | null
  assetIds?: string[]
}

export async function getAssets(pageNumber = 0, pageSize = 10) {
  const { data } = await api.get<{ assets: PaginatedResult<AssetDto> }>("/Asset", {
    params: { pageNumber, pageSize },
  })
  return data.assets
}

export async function getAssetsByLaboratory(
  laboratoryId: string,
  pageNumber = 0,
  pageSize = 50
) {
  const { data } = await api.get<{ assets: PaginatedResult<AssetDto> }>(
    `/Asset/laboratory/${laboratoryId}`,
    {
      params: { pageNumber, pageSize },
    }
  )
  return data.assets
}

export async function getAllocatableAssets(pageNumber = 0, pageSize = 50) {
  const { data } = await api.get<{ assets: PaginatedResult<AssetDto> }>("/Asset/allocatable", {
    params: { pageNumber, pageSize },
  })
  return data.assets
}

export async function getAssetCount() {
  const { data } = await api.get<{ count: number }>("/Asset/count")
  return data.count
}

export async function getAssetById(id: string) {
  const { data } = await api.get<{ asset: AssetDto }>(`/Asset/${id}`)
  return data.asset
}

export async function getAssetHistory(assetId: string) {
  const { data } = await api.get<{ histories: AssetHistoryDto[] }>(
    `/Asset/${assetId}/history`
  )
  return data.histories
}

export async function returnAsset(id: string) {
  const { data } = await api.patch<{ isSuccess: boolean }>(`/Asset/${id}/return`)
  return data.isSuccess
}

export async function getAssetUnitsByAssetId(assetId: string) {
  const { data } = await api.get<{ assetUnits: AssetUnitDto[] }>(
    `/AssetUnit/asset/${assetId}`
  )
  return data.assetUnits
}

export async function getUnassignedAssetUnits() {
  const { data } = await api.get<{ assetUnits: AssetUnitDto[] }>("/AssetUnit/unassigned")
  return data.assetUnits
}

export async function getAssetUnitDetail(id: string) {
  const { data } = await api.get<{ assetUnit: AssetUnitDetailDto }>(`/AssetUnit/${id}/detail`)
  return data.assetUnit
}

export async function getAssetUnitImages(assetUnitId: string) {
  const { data } = await api.get<{ images: AssetUnitImageDto[] }>(
    `/AssetUnit/${assetUnitId}/images`
  )
  return data.images
}

export async function createAssetUnits(payload: CreateAssetUnitPayload) {
  const { data } = await api.post<{ id: string[] }>("/AssetUnit", payload)
  return data.id
}

export async function updateAssetUnit(assetUnitId: string, assetUnit: AssetUnitDto) {
  const { data } = await api.put<{ isSuccess: boolean }>(`/AssetUnit/${assetUnitId}`, {
    assetUnit,
  })
  return data.isSuccess
}

export async function uploadAssetUnitImages(
  assetUnitId: string,
  files: File[],
  description?: string
) {
  const formData = new FormData()
  files.forEach((file) => formData.append("files", file))

  if (description?.trim()) {
    formData.append("description", description.trim())
  }

  const { data } = await api.post<{ images: AssetUnitImageDto[] }>(
    `/AssetUnit/${assetUnitId}/Images`,
    formData,
    {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    }
  )

  return data.images
}

export async function getUserLookup(userIds: string[]) {
  const ids = Array.from(new Set(userIds.filter(Boolean)))
  if (ids.length === 0) return []

  const { data } = await api.get<{ users: UserLookupDto[] }>("/auth/users/lookup", {
    params: { ids: ids.join(",") },
  })
  return data.users
}

export async function createAsset(payload: CreateAssetPayload) {
  const { data } = await api.post<{ id: string }>("/Asset", payload)
  return data
}

export async function getRequests(pageNumber = 0, pageSize = 10) {
  const { data } = await api.get<{ requests: PaginatedResult<RequestDto> }>("/Request", {
    params: { pageNumber, pageSize },
  })
  return data.requests
}

export async function getRequestById(id: string) {
  const { data } = await api.get<{ request: RequestDto }>(`/Request/${id}`)
  return data.request
}

export async function createRequest(payload: CreateRequestPayload) {
  const { data } = await api.post<{ id: string }>("/Request", { request: payload })
  return data
}

export async function markRequestProcessed(payload: MarkProcessedPayload) {
  const { data } = await api.patch<{ isSuccess: boolean }>("/MarkProcessed", payload)
  return data
}

export async function getParameters(pageNumber = 0, pageSize = 20) {
  const { data } = await api.get<
    | PaginatedResult<ParameterDto>
    | ParameterDto[]
    | { response?: PaginatedResult<ParameterDto>; result?: PaginatedResult<ParameterDto> }
  >("/Parameters", { params: { pageNumber, pageSize } })

  if (Array.isArray(data)) {
    return {
      items: data,
      count: data.length,
      pageNumber,
      pageSize,
    }
  }

  if ("items" in data) {
    return data
  }

  return data.response ?? data.result ?? { items: [], count: 0, pageNumber, pageSize }
}

export async function getParametersByGroup(group: string, pageNumber = 0, pageSize = 100) {
  const { data } = await api.get<
    ParameterDto[] | { response?: ParameterDto[]; result?: ParameterDto[] }
  >(`/Parameters/${group}`, {
    params: { pageNumber, pageSize },
  })

  if (Array.isArray(data)) {
    return data
  }

  return data.response ?? data.result ?? []
}

export async function createParameter(parameter: ParameterDto) {
  const { data } = await api.post<{ id: number }>("/Parameter", { parameter })
  return data
}

export async function updateParameter(id: number, parameter: ParameterDto) {
  const { data } = await api.put<{ id: number }>(`/Parameter/${id}`, { parameter })
  return data
}

export async function setParameterActive(id: number, active: boolean) {
  const path = active ? `/Parameter/Endable/${id}` : `/Parameter/Disable/${id}`
  const { data } = await api.patch<{ id: number }>(path, { id })
  return data
}

export async function getLaboratories() {
  const { data } = await api.get<{ laboratories: LaboratoryDto[] }>("/Laboratory")
  return data.laboratories
}

export async function getTeachers() {
  const { data } = await api.get<{ teachers: TeacherDto[] }>("/auth/teachers")
  return data.teachers
}

export async function getHod() {
  const { data } = await api.get<{ hod: TeacherDto }>("/auth/hod")
  return data.hod
}

export async function assignHod(userId: string) {
  const { data } = await api.patch<{ hod: TeacherDto }>("/auth/hod", {
    userId,
  })
  return data.hod
}

export async function assignTeacherToLaboratory(laboratoryId: string, teacherId: string) {
  const { data } = await api.patch<{ isSuccess: boolean }>(`/Laboratory/${laboratoryId}/Teacher`, {
    teacherId,
  })
  return data.isSuccess
}

export async function createUser(payload: CreateUserPayload) {
  const { data } = await api.post<{ userId: string }>("/auth/signup/user", payload)
  return data
}

export async function getUsers() {
  const { data } = await api.get<{ users: UserDto[] }>("/auth/users")
  return data.users
}

export async function updateUserRole(userId: string, roleCode: string) {
  const { data } = await api.patch<{ user: UserDto }>(`/auth/users/${userId}/role`, {
    roleCode,
  })
  return data.user
}

export function formatDate(value?: string | null) {
  if (!value) return "Requires API"
  return new Intl.DateTimeFormat("en", {
    year: "numeric",
    month: "short",
    day: "2-digit",
  }).format(new Date(value))
}

export function shortId(value?: string | null) {
  if (!value) return "-"
  return value.slice(0, 8).toUpperCase()
}
