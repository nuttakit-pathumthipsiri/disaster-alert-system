# Disaster Alert System

A comprehensive disaster alert and risk assessment system built with .NET Core, designed to provide real-time alerts for natural disasters and weather-related emergencies. The system integrates with external weather services, manages user notifications, and provides risk assessment capabilities for different regions.

## Features

- **Real-time Disaster Alerts**: Automated alerts based on weather data and risk assessments
- **Multi-region Support**: Manage alerts and settings for different geographical regions
- **Risk Assessment Engine**: Advanced algorithms for disaster risk evaluation
- **Email Notifications**: Integrated email service using SendGrid
- **Redis Caching**: High-performance caching for weather data and alerts
- **RESTful API**: Clean, documented API endpoints for all system operations

## Prerequisites

- Docker and Docker Compose
- .NET 8.0 SDK
- SQL Server (or SQL Server Express)
- Redis Server

## Quick Start with Docker

### 1. Environment Configuration

Copy the example environment file and configure your settings:

```bash
cp src/API/appsettings.example.json src/API/appsettings.json
```

### 2. API Key Setup

Edit `src/API/appsettings.json` and configure the following API keys:

#### Where to Get API Keys:

**SendGrid API Key:**

- Go to [SendGrid Website](https://sendgrid.com/)
- Sign up for a free account
- Navigate to Settings → API Keys
- Create a new API Key with "Mail Send" permissions
- Copy the generated API key

**Weather API Key:**

- Go to [WeatherAPI.com](https://www.weatherapi.com/)
- Sign up for a free account
- Get your API key from the dashboard
- Free tier includes 1,000,000 calls per month

````

### 3. Run with Docker Compose

Start the required services (SQL Server and Redis) first:

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
````

### 4. Database Setup

Run the following commands to set up the database:

```bash
# Navigate to the API project
cd src/API

# Create and apply database migrations
dotnet ef database update

# Or if you prefer to run the migration script directly
dotnet run --project ../API
```

## How to Build the Project

### Prerequisites Installation

1. **Install .NET 8.0 SDK**

   - Download from [Microsoft .NET Downloads](https://dotnet.microsoft.com/download)
   - Verify installation: `dotnet --version`

2. **Install SQL Server**

   - Download SQL Server Express or Developer Edition
   - Or use Docker: `docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourPassword123" -p 1433:1433 --name sql1 --hostname sql1 -d mcr.microsoft.com/mssql/server:2019-latest`

3. **Install Redis**
   - Windows: Use Docker or WSL2
   - Or use Docker: `docker run --name redis -p 6379:6379 -d redis:alpine`

### Build Commands

```bash
# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run tests (if available)
dotnet test

# Publish for production
dotnet publish -c Release
```

## Technologies Used

- **Backend**: .NET 8.0, ASP.NET Core, Entity Framework Core, C#
- **Database**: SQL Server, Redis
- **External Services**: SendGrid, Weather API, USGS Earthquake API
- **Infrastructure**: Docker, Docker Compose
- **Architecture**: Repository Pattern, Service Layer, DTO Pattern, Dependency Injection
