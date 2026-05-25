# API Ticket: Create Request Items

## Feature
Student submits an asset request with selected assets and quantity

## Screen
Create Request Dialog

## Endpoint Needed
POST /Request

## Request

```json
{
  "request": {
    "requestType": "BORROW",
    "targetLaboratoryId": "guid",
    "reason": "string",
    "detail": {
      "purpose": "string",
      "borrowFrom": "datetime",
      "borrowTo": "datetime",
      "extraNote": "string"
    },
    "items": [
      {
        "assetId": "guid",
        "quantity": 1
      }
    ]
  }
}
```

## Response

```json
{
  "id": "guid"
}
```

## Business Purpose

Allows students to submit the exact asset/item and quantity needed instead of creating a request with only laboratory and reason fields.

## Priority

High
