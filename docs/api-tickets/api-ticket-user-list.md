# API Ticket: User List

## Feature
User management table

## Screen
User Management

## Endpoint Needed
GET /auth/users

## Request

```json
{
  "pageNumber": 0,
  "pageSize": 20,
  "roleCode": "TEACHER",
  "search": "string"
}
```

## Response

```json
{
  "users": {
    "items": [
      {
        "id": "guid",
        "name": "string",
        "email": "string",
        "roleCode": "TEACHER",
        "status": "ACTIVE",
        "lastActiveAt": "datetime"
      }
    ],
    "count": 0,
    "pageNumber": 0,
    "pageSize": 20
  }
}
```

## Business Purpose

Allows administrators to review users, roles, account status, and recent activity from real backend data.

## Priority

High
