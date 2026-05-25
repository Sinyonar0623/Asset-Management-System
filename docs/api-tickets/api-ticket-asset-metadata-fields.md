# API Ticket: Asset Metadata Fields

## Feature
Asset list and detail metadata

## Screen
Asset List, Asset Detail

## Endpoint Needed
GET /Asset and GET /Asset/{id}

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
  "assets": {
    "items": [
      {
        "id": "guid",
        "name": "string",
        "description": "string",
        "category": "string",
        "isAvailable": true,
        "code": "string",
        "location": "string",
        "updatedAt": "datetime"
      }
    ],
    "count": 0,
    "pageNumber": 0,
    "pageSize": 20
  }
}
```

## Business Purpose

Supports the required asset table columns and makes asset records operationally scannable for department staff.

## Priority

Medium
