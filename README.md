# Disaster Alert System

ระบบแจ้งเตือนภัยพิบัติที่ใช้ .NET 8 และ Clean Architecture

## โครงสร้างโปรเจค

- `src/API` - Web API layer
- `src/Core` - Business logic layer
- `src/Infrastructure` - Data access layer

## การติดตั้งและรัน

### 1. รัน Database และ Redis ด้วย Docker

```bash
# รัน SQL Server และ Redis
docker-compose up -d

# ดูสถานะ containers
docker-compose ps

# หยุด services
docker-compose down
```

### 2. สร้าง Database Migration

```bash
# จาก API project directory
cd src/API
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3. รัน API

```bash
# จาก API project directory
cd src/API
dotnet run
```

### 4. ทดสอบ API

- Swagger UI: https://localhost:7000/swagger
- API Base URL: https://localhost:7000/api

## API Endpoints

### POST /api/regions

สร้างพื้นที่ใหม่พร้อมพิกัดและประเภทภัยพิบัติที่ต้องการติดตาม

**Request Body:**

```json
{
  "name": "กรุงเทพมหานคร",
  "latitude": 13.7563,
  "longitude": 100.5018,
  "monitoredDisasterTypes": ["Flood", "Storm"]
}
```

## Database Schema

### Regions

- ID, Name, Latitude, Longitude
- MonitoredDisasterTypes (JSON array)
- CreatedAt, UpdatedAt

### AlertSettings

- ID, RegionId, DisasterType
- ThresholdRiskScore, IsActive
- CreatedAt, UpdatedAt

## เทคโนโลยีที่ใช้

- .NET 8
- Entity Framework Core
- SQL Server 2022
- Redis
- Docker
- Clean Architecture

## Database Connection

- **SQL Server**: localhost:1433
- **Database**: disaster_alert_db
- **Username**: sa
- **Password**: SqlServer123!
- **Redis**: localhost:6379
