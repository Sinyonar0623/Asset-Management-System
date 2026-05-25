# API Ticket: Dashboard Request Status Counts

## Feature
Dashboard KPI cards for request status totals

## Screen
Dashboard

## Endpoint Needed
GET /Request/status-counts

## Request

```json
{}
```

## Response

```json
{
  "pending": 0,
  "approved": 0,
  "rejected": 0,
  "cancelled": 0,
  "completed": 0
}
```

## Business Purpose

Provides accurate dashboard KPI totals without deriving counts from a single paginated request page.

## Priority

High
