# 🚀 HumanBase

Fullstack application for managing **persons, categories, and financial
transactions**.

## 📦 Tech Overview

  Layer      Technology
  ---------- -------------------------------
  Frontend   React (Vite), TypeScript
  Backend    .NET 8 (ASP.NET Core Web API)
  Database   PostgreSQL

------------------------------------------------------------------------

# 🖥️ Frontend

User interface that consumes the HumanBase API.

## ⚙️ Tech Stack

-   React + Vite
-   TypeScript
-   Tailwind CSS
-   shadcn/ui
-   Axios

## ▶️ Getting Started

### 1. Install dependencies

``` bash
npm install
```

### 2. Configure environment

Create a `.env` file:

``` bash
VITE_API_URL=http://localhost:5000
```

### 3. Run the application

``` bash
npm run dev
```

App runs at: http://localhost:5173

## 📜 Scripts

``` bash
npm run dev
npm run build
npm run preview
npm run lint
```

------------------------------------------------------------------------

# ⚙️ Backend (API)

REST API for managing:

-   Persons
-   Categories
-   Financial transactions

## 🧰 Tech Stack

-   .NET 8 (ASP.NET Core)
-   Entity Framework Core + Npgsql
-   MediatR
-   FluentValidation
-   Swagger (OpenAPI)

------------------------------------------------------------------------

## ▶️ Getting Started

### Requirements

-   .NET SDK 8
-   PostgreSQL (local or Docker)

### 🐳 Run PostgreSQL with Docker

``` bash
docker run -d --name HumanBaseDB -e POSTGRES_USER="root" -e POSTGRES_PASSWORD="root" -e POSTGRES_DB="humanbaselocal" -p 5432:5432 docker.io/postgres
```

### 🔧 Configuration

Edit the connection string if needed:

ConnectionStrings:DefaultConnection

Files: - Service.Api/appsettings.Development.json -
Service.Api/appsettings.Local.json

### ▶️ Run the API

``` bash
dotnet restore
dotnet run --project Service.Api
```

### 📄 Swagger

Available at:

/swagger

(when running in Development or Local)

------------------------------------------------------------------------

# 🔗 API Endpoints

Base route:

/api/{controller}

## Categories

-   POST /api/Category
-   GET /api/Category?pageSize=&page=
-   GET /api/Category/{id}

## Persons

-   POST /api/Person
-   PUT /api/Person/{id}
-   DELETE /api/Person/{id}
-   GET /api/Person?pageSize=&page=
-   GET /api/Person/{id}

## Transactions

-   POST /api/AppTransaction
-   GET /api/AppTransaction?pageSize=&page=
-   GET /api/AppTransaction/{id}

------------------------------------------------------------------------

# 📝 Notes

-   Database is automatically created on startup (EnsureCreated)
-   CORS is fully open (development only)
