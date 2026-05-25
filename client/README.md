# CE-AMS Frontend

ส่วนนี้คือ frontend ของ CE-AMS (Computer Engineering Asset Management System) สำหรับให้ผู้ใช้ทำงานกับระบบจัดการครุภัณฑ์ผ่านหน้าเว็บ เช่น เข้าสู่ระบบ ดูข้อมูลทรัพย์สิน จัดการ asset unit และติดตามคำขอที่เกี่ยวข้องกับการยืม จัดสรร ซ่อม หรือปลดระวางครุภัณฑ์

Frontend พัฒนาด้วย Next.js, React และ TypeScript โดยเชื่อมต่อกับ backend API ในโฟลเดอร์ `Server/Application/Api/`

## หน้าที่หลัก

- แสดงข้อมูลและสถานะของครุภัณฑ์
- รองรับการเข้าสู่ระบบและการทำงานตามบทบาทผู้ใช้
- ใช้สำหรับสร้างและติดตาม request workflow
- เชื่อมต่อข้อมูล parameter และ asset จาก backend API

## โครงสร้างสำคัญ

```text
client/
|-- app/             # Next.js App Router pages and layouts
|-- components/      # Shared UI components
|-- context/         # React context providers
|-- lib/             # API helpers and utilities
|-- public/          # Static assets
```

## Scripts

```bash
npm run dev      # Run local development server
npm run build    # Build production frontend
npm run start    # Start production server after build
npm run lint     # Run ESLint
```
