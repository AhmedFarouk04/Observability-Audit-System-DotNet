# Observability & Audit System 🚀

A robust, enterprise-level monitoring and auditing solution for .NET applications. This project demonstrates how to implement a high-performance Audit Log and Metrics system using SQL Server and Docker.

## 🌟 Features

- **Automated Audit Logging**: Custom Middleware to capture every HTTP request, user action, IP address, and duration.
- **Real-time Metrics**: Track system health with Latency monitoring (P50, P95, P99) and Error Rate tracking.
- **SQL Server Integration**: Fully migrated from MySQL to SQL Server with optimized EF Core configurations.
- **Health Checks**: Integrated health monitoring for both the API and the SQL Server database.
- **Data Export**: Built-in functionality to export audit logs to CSV for administrative reporting.
- **Dockerized Environment**: Ready-to-use Docker Compose for SQL Server 2022.

## 🛠️ Tech Stack

- **Framework**: .NET 8 (ASP.NET Core)
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core
- **Containerization**: Docker & Docker Compose
- **Monitoring**: Health Checks & Custom Metrics Middleware
- **Testing**: Postman for API Validation

## 🚀 Getting Started

### Prerequisites

- Docker Desktop
- .NET 8 SDK
- SQL Server Management Studio (SSMS) or Azure Data Studio
