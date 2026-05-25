# Asset API Test Cases (Postman)

ใช้กับโมดูล `Server/Modules/Asset/Asset`.

## 1) Collection Variables

ตั้งค่าใน Postman Collection Variables:

- `baseUrl` = `http://localhost:5000` (ปรับตามพอร์ตจริง)
- `teacherId` = `11111111-1111-1111-1111-111111111111`
- `laboratoryId` = ``
- `assetUnitId` = ``
- `assetUnitId2` = ``
- `assetId` = ``
- `assetName` = ``

## 2) Test Flow (Happy Path)

### TC-LAB-01 Create Laboratory
- Method/URL: `POST {{baseUrl}}/Laboratory`
- Body:
```json
{
  "laboratory": {
    "id": "00000000-0000-0000-0000-000000000000",
    "laboratoryName": "Postman Lab {{$timestamp}}",
    "roomNo": "R-{{$randomInt}}",
    "teacherId": "{{teacherId}}",
    "description": "lab created by postman"
  }
}
```
- Expected: `200 OK`, body มี `id`
- Tests:
```javascript
pm.test("status is 200", () => pm.response.to.have.status(200));
const j = pm.response.json();
pm.collectionVariables.set("laboratoryId", j.id || j.Id);
```

### TC-LAB-02 Get Laboratory List
- Method/URL: `GET {{baseUrl}}/Laboratory`
- Expected: `200 OK`, มี array `laboratories`

### TC-LAB-03 Get Laboratory By Id
- Method/URL: `GET {{baseUrl}}/Laboratory/{{laboratoryId}}`
- Expected: `200 OK`, `laboratory.id` ตรงกับตัวแปร

### TC-LAB-04 Update Laboratory
- Method/URL: `PUT {{baseUrl}}/Laboratory/{{laboratoryId}}`
- Body:
```json
{
  "laboratory": {
    "id": "{{laboratoryId}}",
    "laboratoryName": "Postman Lab Updated",
    "roomNo": "R-999",
    "teacherId": "{{teacherId}}",
    "description": "updated by postman"
  }
}
```
- Expected: `200 OK`, `isSuccess = true`

### TC-UNIT-01 Create Asset Units (2 units)
- Method/URL: `POST {{baseUrl}}/AssetUnit`
- Body:
```json
{
  "assetUnits": [
    {
      "assetId": null,
      "assetTag": "AT-{{$timestamp}}-1",
      "serialNo": "SN-{{$timestamp}}-1",
      "name": "Unit One",
      "brand": "Dell",
      "availabilityStatus": "AVAILABLE",
      "operationalStatus": "READY",
      "remark": "seed1",
      "ownerId": null
    },
    {
      "assetId": null,
      "assetTag": "AT-{{$timestamp}}-2",
      "serialNo": "SN-{{$timestamp}}-2",
      "name": "Unit Two",
      "brand": "HP",
      "availabilityStatus": "AVAILABLE",
      "operationalStatus": "READY",
      "remark": "seed2",
      "ownerId": null
    }
  ]
}
```
- Expected: `200 OK`, body มี list `id`
- Tests:
```javascript
pm.test("status is 200", () => pm.response.to.have.status(200));
const j = pm.response.json();
const ids = j.id || j.Id || [];
pm.collectionVariables.set("assetUnitId", ids[0]);
pm.collectionVariables.set("assetUnitId2", ids[1]);
```

### TC-UNIT-02 Get Asset Unit By Id
- Method/URL: `GET {{baseUrl}}/AssetUnit/{{assetUnitId}}`
- Expected: `200 OK`

### TC-UNIT-03 Update Asset Unit
- Method/URL: `PUT {{baseUrl}}/AssetUnit/{{assetUnitId}}`
- Body:
```json
{
  "assetUnit": {
    "id": "{{assetUnitId}}",
    "assetId": null,
    "assetTag": "AT-UPD-{{$timestamp}}",
    "serialNo": "SN-UPD-{{$timestamp}}",
    "name": "Unit One Updated",
    "brand": "Lenovo",
    "availabilityStatus": "AVAILABLE",
    "operationalStatus": "READY",
    "remark": "updated",
    "ownerId": null
  }
}
```
- Expected: `200 OK`, `isSuccess = true`

### TC-ASSET-01 Create Asset
- Method/URL: `POST {{baseUrl}}/Asset`
- Pre-request Script:
```javascript
pm.collectionVariables.set("assetName", `PostmanAsset-${Date.now()}`);
```
- Body:
```json
{
  "asset": {
    "name": "{{assetName}}",
    "description": "asset created by postman",
    "category": "COMPUTER"
  },
  "units": []
}
```
- Expected: `201 Created` (endpoint นี้ currently ไม่คืน body)

### TC-ASSET-02 Get Asset List + Capture Asset Id
- Method/URL: `GET {{baseUrl}}/Asset?pageNumber=0&pageSize=50`
- Expected: `200 OK`
- Tests:
```javascript
const j = pm.response.json();
const wrap = j.assets || j.Assets || {};
const items = wrap.items || wrap.Items || [];
const name = pm.collectionVariables.get("assetName");
const hit = items.find(x => (x.name || x.Name) === name);
pm.test("asset exists in list", () => pm.expect(hit).to.exist);
if (hit) pm.collectionVariables.set("assetId", hit.id || hit.Id);
```

### TC-ASSET-03 Get Asset By Id
- Method/URL: `GET {{baseUrl}}/Asset/{{assetId}}`
- Expected: `200 OK`

### TC-ASSET-04 Get Asset Count
- Method/URL: `GET {{baseUrl}}/Asset/count`
- Expected: `200 OK`, มี `count`

### TC-ASSET-05 Assign Laboratory (Domain Event)
- Method/URL: `PATCH {{baseUrl}}/AssetAssignLaboratory`
- Body:
```json
{
  "assetId": "{{assetId}}",
  "labId": "{{laboratoryId}}"
}
```
- Expected: `200 OK`, `isSuccess = true`

### TC-ASSET-06 Get Asset By Laboratory
- Method/URL: `GET {{baseUrl}}/Asset/laboratory/{{laboratoryId}}?pageNumber=0&pageSize=50`
- Expected: `200 OK`, มี asset ที่ id = `{{assetId}}`

### TC-ASSET-07 Get Asset Count By Laboratory
- Method/URL: `GET {{baseUrl}}/Asset/laboratory/{{laboratoryId}}/count`
- Expected: `200 OK`, `count >= 1`

### TC-ASSET-08 Assign Asset Unit
- Method/URL: `PATCH {{baseUrl}}/AssetAssignUnit`
- Body:
```json
{
  "assetId": "{{assetId}}",
  "newUnits": ["{{assetUnitId2}}"]
}
```
- Expected: `200 OK`, `isSuccess = true`

### TC-UNIT-04 Get Units By Asset Id
- Method/URL: `GET {{baseUrl}}/AssetUnit/asset/{{assetId}}`
- Expected: `200 OK`, list มี `{{assetUnitId2}}`

## 3) Negative Cases

### TC-NEG-01 Get Asset By Id (Not Found)
- Method/URL: `GET {{baseUrl}}/Asset/00000000-0000-0000-0000-000000000001`
- Expected: `404` (problem details)

### TC-NEG-02 Assign Lab ด้วย Lab ที่ไม่มีจริง
- Method/URL: `PATCH {{baseUrl}}/AssetAssignLaboratory`
- Body:
```json
{
  "assetId": "{{assetId}}",
  "labId": "00000000-0000-0000-0000-000000000001"
}
```
- Expected: `404`

### TC-NEG-03 Assign Unit ด้วย Unit ที่ไม่มีจริง
- Method/URL: `PATCH {{baseUrl}}/AssetAssignUnit`
- Body:
```json
{
  "assetId": "{{assetId}}",
  "newUnits": ["00000000-0000-0000-0000-000000000001"]
}
```
- Expected: `404`

### TC-NEG-04 Delete Laboratory ที่ยังถูกใช้งาน
- Method/URL: `DELETE {{baseUrl}}/Laboratory/{{laboratoryId}}`
- Expected: `400 Bad Request`

## 4) Cleanup Cases

### TC-CLN-01 Delete Asset
- Method/URL: `DELETE {{baseUrl}}/Asset/{{assetId}}`
- Expected: `200 OK`, `isSuccess = true`

### TC-CLN-02 Delete Laboratory (หลังลบ Asset)
- Method/URL: `DELETE {{baseUrl}}/Laboratory/{{laboratoryId}}`
- Expected: `200 OK`, `isSuccess = true`

### TC-CLN-03 Delete Asset Units
- Method/URL 1: `DELETE {{baseUrl}}/AssetUnit/{{assetUnitId}}`
- Method/URL 2: `DELETE {{baseUrl}}/AssetUnit/{{assetUnitId2}}`
- Expected: `200 OK`, `isSuccess = true`

## 5) API Checklist ครบทุก Endpoint (Asset Module)

- `POST /Asset`
- `GET /Asset`
- `GET /Asset/{id}`
- `PUT /Asset/{id}`
- `DELETE /Asset/{id}`
- `GET /Asset/count`
- `GET /Asset/laboratory/{laboratoryId}`
- `GET /Asset/laboratory/{laboratoryId}/count`
- `PATCH /AssetAssignUnit`
- `PATCH /AssetAssignLaboratory`
- `POST /AssetUnit`
- `GET /AssetUnit/{id}`
- `GET /AssetUnit/asset/{assetId}`
- `PUT /AssetUnit/{id}`
- `DELETE /AssetUnit/{id}`
- `POST /Laboratory`
- `GET /Laboratory`
- `GET /Laboratory/{id}`
- `PUT /Laboratory/{id}`
- `DELETE /Laboratory/{id}`