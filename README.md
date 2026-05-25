# CE-AMS

CE-AMS (Computer Engineering Asset Management System) คือระบบจัดการครุภัณฑ์สำหรับภาควิชาวิศวกรรมคอมพิวเตอร์ ใช้สำหรับเก็บข้อมูลทรัพย์สิน ยืม-คืน ติดตามสถานะ และจัดการคำขอผ่าน workflow การอนุมัติ

## เราทำอะไรในระบบนี้

- ระบบ Login/Logout ด้วย JWT และ role-based access
- จัดการผู้ใช้ตามบทบาท เช่น Admin, Department Head, Lecturer, Student
- จัดการข้อมูล Asset, Asset Unit, Laboratory และรูปภาพของ Asset Unit
- จัดการ Parameter สำหรับค่ากลางของระบบ
- สร้างคำขอ Borrow, Allocate, Repair และ Retire
- Workflow อนุมัติคำขอผ่าน Teacher และ HOD
- Reserve, Mark In Use, Release และ Return asset ผ่าน backend workflow
- บันทึกประวัติการเปลี่ยนสถานะของ asset unit
- ใช้ RabbitMQ/MassTransit สำหรับสื่อสารข้าม module เช่น Request ไป Asset

## Tech Stack

- Frontend: Next.js 16, React 19, TypeScript
- Backend: .NET 10, Carter, MediatR
- Database: PostgreSQL + EF Core
- Messaging: RabbitMQ + MassTransit
- Local services: Docker Compose

## Project Structure

```text
.
|-- client/                     # Next.js frontend
|-- Server/
|   |-- Application/Api/         # API host / composition root
|   |-- Modules/
|   |   |-- Auth/                # Login, logout, users, roles
|   |   |-- Asset/               # Assets, asset units, labs, history
|   |   |-- Parameter/           # System parameters
|   |   |-- Request/             # Borrow/allocate/repair/retire requests
|   |-- Shared/                  # CQRS, DDD, EF, messaging helpers
|-- scripts/                    # EF migration helper scripts
|-- docker-compose.yml          # PostgreSQL + RabbitMQ
```

## วิธีรันบนเครื่อง

### 1. เตรียมเครื่อง

ต้องมี:

- .NET SDK 10
- Node.js 20+
- Docker Desktop

### 2. Start PostgreSQL และ RabbitMQ

สร้างไฟล์ `.env` จาก `.env.example` แล้วใส่ค่าที่ใช้กับ Docker Compose เช่น database user/password และ RabbitMQ user/password

จากนั้นรัน:

```powershell
docker compose up -d
```

Service ที่จะได้:

- PostgreSQL: `localhost:5433`
- RabbitMQ: `localhost:5672`
- RabbitMQ Management UI: `http://localhost:15672`

### 3. ตั้งค่า backend secrets

แนะนำให้ใช้ `dotnet user-secrets` เพื่อไม่ต้อง commit secret ลง repo

```powershell
dotnet user-secrets --project Server/Application/Api/Api.csproj set "ConnectionStrings:Database" "Host=localhost;Port=5433;Database=AssetManagementDb;Username=<DB_USER>;Password=<DB_PASS>"
dotnet user-secrets --project Server/Application/Api/Api.csproj set "Jwt:Issuer" "AssetApi"
dotnet user-secrets --project Server/Application/Api/Api.csproj set "Jwt:Audience" "AssetFrontend"
dotnet user-secrets --project Server/Application/Api/Api.csproj set "Jwt:Key" "<RANDOM_KEY_MIN_32_CHARS>"
dotnet user-secrets --project Server/Application/Api/Api.csproj set "RabbitMQ:Host" "amqp://localhost:5672/"
dotnet user-secrets --project Server/Application/Api/Api.csproj set "RabbitMQ:Username" "<RABBITMQ_USER>"
dotnet user-secrets --project Server/Application/Api/Api.csproj set "RabbitMQ:Password" "<RABBITMQ_PASS>"
```

### 4. Apply database migrations

ใช้ helper script:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/ef-migrations.ps1 -Action update -Context all -NoBuild
```

หรือรัน EF Core แยก context:

```powershell
dotnet ef database update --project Server/Modules/Auth/Auth/Auth.csproj --startup-project Server/Application/Api/Api.csproj --context Auth.Data.AuthDbContext
dotnet ef database update --project Server/Modules/Asset/Asset/Asset.csproj --startup-project Server/Application/Api/Api.csproj --context Asset.Data.AssetDbContext
dotnet ef database update --project Server/Modules/Parameter/Parameter/Parameter.csproj --startup-project Server/Application/Api/Api.csproj --context Parameter.Data.ParameterDbContext
dotnet ef database update --project Server/Modules/Request/Request/Request.csproj --startup-project Server/Application/Api/Api.csproj --context Request.Data.RequestDbContext
```

### 5. Run backend

```powershell
dotnet restore Server.sln
dotnet build Server.sln
dotnet run --project Server/Application/Api/Api.csproj
```

Default backend URLs:

- `http://localhost:5176`
- `https://localhost:7158`

ถ้า build ไม่ผ่านเพราะไฟล์ `.dll` ถูก lock ให้หยุด API process ที่รันค้างอยู่ก่อน แล้วค่อย build ใหม่

### 6. Run frontend

เปิด terminal อีกหน้าหนึ่ง:

```powershell
cd client
npm install
npm run dev
```

Default frontend URL:

- `http://localhost:3000`

ถ้าต้องการชี้ frontend ไป backend URL อื่น ให้ตั้งค่า:

```powershell
$env:NEXT_PUBLIC_API_URL="http://localhost:5176"
npm run dev
```

## คำสั่งที่ใช้บ่อย

Backend:

```powershell
dotnet restore Server.sln
dotnet build Server.sln
dotnet run --project Server/Application/Api/Api.csproj
```

Frontend:

```powershell
cd client
npm run dev
npm run build
npm run lint
```

Infrastructure:

```powershell
docker compose up -d
docker compose down
```

Migrations:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/ef-migrations.ps1 -Action update -Context all -NoBuild
```

## หมายเหตุสำหรับการพัฒนา

- ห้าม commit secret ใน `.env` หรือ `appsettings*.json`
- Backend secret ควรเก็บผ่าน `dotnet user-secrets`
- ถ้าแก้ backend ขณะ API ยังรันอยู่ อาจต้อง restart API เพื่อให้โหลด DLL ใหม่
- ตอนนี้ยังไม่มี test project ที่ commit ไว้ ให้ใช้ `dotnet build Server.sln`, `npm run build`, และ `npm run lint` เป็น minimum verification

## Known Gaps

- ยังไม่มี automated test suite
- Refresh token/revocation strategy ยังไม่ครบแบบ production-grade
- บาง endpoint ยังต้องเพิ่ม authorization policy ให้ละเอียดขึ้น
- UX หลังคืน asset ของ borrow request ยังปรับปรุงต่อได้
