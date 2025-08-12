# Disaster Alert System

ระบบแจ้งเตือนภัยพิบัติที่พัฒนาด้วย .NET 8 และ Entity Framework Core

## ระบบใหม่

ระบบได้รับการปรับปรุงให้เก็บข้อมูลพื้นที่เสี่ยงลงฐานข้อมูลแทนการส่งเมลโดยตรง โดยมีฟีเจอร์ดังนี้:

### ตารางใหม่: Alerts

- เก็บข้อมูลพื้นที่เสี่ยงภัยพิบัติ
- มี flag `EmailSent` บอกสถานะการส่งเมล
- เก็บข้อมูล external API ที่ใช้คำนวณความเสี่ยง
- มี timestamp สำหรับการหมดอายุของข้อมูล

### API Endpoints ใหม่

#### 1. POST /api/alerts/send

ส่งแจ้งเตือนไปยังพื้นที่เสี่ยงสูง พร้อมบันทึกลงฐานข้อมูล

**Request Body:**

```json
{
  "regionId": 1,
  "disasterTypeId": 2,
  "customMessage": "ข้อความเพิ่มเติม (ไม่บังคับ)",
  "forceSend": false
}
```

**Response:**

```json
{
  "id": 1,
  "regionId": 1,
  "regionName": "กรุงเทพมหานคร",
  "disasterTypeId": 2,
  "disasterTypeName": "Flood",
  "riskScore": 85.5,
  "riskLevel": "High",
  "thresholdValue": 25.0,
  "emailSent": true,
  "emailSentAt": "2025-01-11T09:31:43Z",
  "alertMessage": "แจ้งเตือนภัยพิบัติ: ระดับความเสี่ยง สูง (คะแนนความเสี่ยง: 85.5%, เกณฑ์: 25.0%)",
  "detectedAt": "2025-01-11T09:31:43Z",
  "expiresAt": "2025-01-11T09:46:43Z"
}
```

#### 2. GET /api/alerts

ดึงประวัติการแจ้งเตือนจากฐานข้อมูล

**Response:**

```json
[
  {
    "id": 1,
    "regionId": 1,
    "regionName": "กรุงเทพมหานคร",
    "disasterTypeId": 2,
    "disasterTypeName": "Flood",
    "riskScore": 85.5,
    "riskLevel": "High",
    "thresholdValue": 25.0,
    "emailSent": true,
    "emailSentAt": "2025-01-11T09:31:43Z",
    "alertMessage": "แจ้งเตือนภัยพิบัติ: ระดับความเสี่ยง สูง (คะแนนความเสี่ยง: 85.5%, เกณฑ์: 25.0%)",
    "detectedAt": "2025-01-11T09:31:43Z",
    "expiresAt": "2025-01-11T09:46:43Z"
  }
]
```

#### 3. GET /api/alerts/region/{regionId}

ดึงการแจ้งเตือนตามพื้นที่

#### 4. GET /api/alerts/disaster-type/{disasterTypeId}

ดึงการแจ้งเตือนตามประเภทภัยพิบัติ

#### 5. GET /api/alerts/pending

ดึงการแจ้งเตือนที่ยังไม่ได้ส่งเมล

#### 6. POST /api/alerts/{alertId}/mark-email-sent

ทำเครื่องหมายว่าส่งเมลแล้ว

### การทำงานของระบบ

1. **การประเมินความเสี่ยง**: `DisasterRiskAssessmentService` จะประเมินความเสี่ยงจาก external APIs
2. **การเก็บข้อมูล**: เมื่อความเสี่ยงเกินเกณฑ์ จะเก็บข้อมูลลงตาราง `Alerts`
3. **การส่งแจ้งเตือน**: ใช้ API `/api/alerts/send` เพื่อส่งเมลและอัพเดทสถานะ
4. **การติดตาม**: สามารถดูประวัติการแจ้งเตือนและสถานะการส่งเมลได้

## การติดตั้ง

### Prerequisites

- .NET 8 SDK
- SQL Server
- Redis (optional)

### การรัน

1. Clone repository
2. ตั้งค่า connection string ใน `appsettings.json`
3. รัน migration:
   ```bash
   dotnet ef database update
   ```
4. รัน application:
   ```bash
   dotnet run
   ```

## โครงสร้างฐานข้อมูล

### ตารางหลัก

- **Regions**: พื้นที่ที่ติดตาม
- **DisasterTypes**: ประเภทภัยพิบัติ
- **AlertSettings**: ตั้งค่าการแจ้งเตือน
- **Users**: ผู้ใช้ในระบบ
- **Alerts**: ประวัติการแจ้งเตือน
- **Alerts**: พื้นที่เสี่ยงภัยพิบัติ (ใหม่)

### ความสัมพันธ์

- Region มี AlertSettings และ Alerts
- DisasterType มี AlertSettings และ Alerts
- User อยู่ใน Region
- Alert มี flag EmailSent และ EmailSentAt

## การพัฒนา

### การเพิ่มประเภทภัยพิบัติใหม่

1. เพิ่มในตาราง `DisasterTypes`
2. เพิ่ม logic ใน `CalculateRiskScoreAsync`
3. เพิ่มการจัดการใน `FetchExternalApiDataAsync`

### การปรับปรุงการคำนวณความเสี่ยง

แก้ไข method `CalculateRiskScoreAsync` ใน `DisasterRiskAssessmentService`

### การเพิ่ม external API

เพิ่ม service ใหม่และเรียกใช้ใน `FetchExternalApiDataAsync`
