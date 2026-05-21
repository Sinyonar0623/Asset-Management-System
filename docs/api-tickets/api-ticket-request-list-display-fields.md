# API Ticket: Request List Display Fields

## Feature
Readable request list table

## Screen
Request List

## Endpoint Needed
GET /Request

## Request

```json
{
  "pageNumber": 0,
  "pageSize": 20
}
```

## Response

```json
{
  "requests": {
    "items": [
      {
        "id": "guid",
        "requesterId": "guid",
        "requesterName": "string",
        "assetSummary": "string",
        "quantity": 1,
        "status": "PENDING",
        "currentStepNo": 2,
        "currentStepRoleCode": "TEACHER",
        "submittedOn": "datetime"
      }
    ],
    "count": 0,
    "pageNumber": 0,
    "pageSize": 20
  }
}
```

## Business Purpose

Allows staff to scan requester names and requested items without cross-referencing IDs manually.

## Priority

High
