# API Ticket: Parameter Row Metadata

## Feature
Parameter row actions and updated timestamp

## Screen
Parameter List

## Endpoint Needed
GET /Parameters

## Request

```json
{
  "pageNumber": 0,
  "pageSize": 50
}
```

## Response

```json
{
  "response": {
    "items": [
      {
        "id": 1,
        "group": "string",
        "value": "string",
        "description": "string",
        "active": true,
        "updatedAt": "datetime"
      }
    ],
    "count": 0,
    "pageNumber": 0,
    "pageSize": 50
  }
}
```

## Business Purpose

Enables edit/disable row actions and displays recent configuration changes in the parameter table.

## Priority

Medium
