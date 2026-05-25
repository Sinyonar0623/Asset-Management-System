# API Ticket: Settings Preferences

## Feature
Persist system and account settings

## Screen
Settings

## Endpoint Needed
GET /settings and PUT /settings

## Request

```json
{
  "appearance": {
    "theme": "LIGHT",
    "density": "COMFORTABLE"
  },
  "notifications": {
    "requestUpdates": true,
    "approvalAssignments": true
  },
  "security": {
    "passwordPolicyEnabled": true
  }
}
```

## Response

```json
{
  "appearance": {
    "theme": "LIGHT",
    "density": "COMFORTABLE"
  },
  "notifications": {
    "requestUpdates": true,
    "approvalAssignments": true
  },
  "security": {
    "passwordPolicyEnabled": true
  },
  "updatedAt": "datetime"
}
```

## Business Purpose

Allows CE-AMS administrators to persist dashboard preferences and notification/security settings instead of relying on disabled UI controls.

## Priority

Low
