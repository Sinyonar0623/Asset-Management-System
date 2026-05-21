# API Ticket: Asset History Related Requests

## Feature
Asset history and related request tabs

## Screen
Asset Detail

## Endpoint Needed
GET /Asset/{id}/history and GET /Asset/{id}/requests

## Request

```json
{}
```

## Response

```json
{
  "history": [
    {
      "assetUnitId": "guid",
      "actionType": "string",
      "performedBy": "guid",
      "performedAt": "datetime",
      "requestId": "guid",
      "remark": "string"
    }
  ],
  "requests": [
    {
      "id": "guid",
      "status": "PENDING",
      "requesterName": "string",
      "submittedOn": "datetime"
    }
  ]
}
```

## Business Purpose

Allows asset owners to audit asset changes and see active workflow dependencies for a specific asset.

## Priority

Medium
