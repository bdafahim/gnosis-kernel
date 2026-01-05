# Gnosis Kernel – Backend API

This repository contains the backend API for **Gnosis Kernel**, a code intelligence platform that indexes source code, extracts symbols and relationships, and exposes them through a structured API for search, graph visualization, and conversational analysis.

The backend is implemented using **ASP.NET Core**, follows a **clean layered architecture**, and serves as the central orchestration layer between storage, indexing jobs, and the frontend application.

---

## Table of Contents

- [How to Run](#how-to-run)
- [Running Unit Tests](#running-unit-tests)
- [Architecture Overview](#architecture-overview)
- [Project Structure](#project-structure)
- [Core Domains & Services](#core-domains--services)
  - [Authentication](#authentication)
  - [Projects & Artifacts](#projects--artifacts)
  - [Search & Indexing](#search--indexing)
  - [Chat & AI Integration](#chat--ai-integration)
- [API Design](#api-design)
- [Technical Decisions](#technical-decisions)
- [Configuration](#configuration)
---

## How to Run

### Prerequisites

- .NET SDK **8.0+**
- PostgreSQL
- Docker (optional, for containerized execution)

### Project Structure

```text
.
├── GnosisKernel.Api
│   ├── Auth
│   │   ├── InternalKeyAuthAttribute.cs
│   │   ├── JwtTokenService.cs
│   │   └── PasswordHasher.cs
│   ├── Contracts
│   │   ├── ArtifactContracts.cs
│   │   ├── AuthContracts.cs
│   │   ├── ChatContracts.cs
│   │   ├── InternalJobContracts.cs
│   │   ├── JobContracts.cs
│   │   ├── ProjectContracts.cs
│   │   └── SearchContracts.cs
│   ├── Controllers
│   │   ├── ArtifactsController.cs
│   │   ├── AuthController.cs
│   │   ├── ChatController.cs
│   │   ├── InternalJobsController.cs
│   │   ├── JobsController.cs
│   │   ├── ProjectsController.cs
│   │   ├── SearchController.cs
│   │   └── UploadsController.cs
│   ├── Data
│   │   ├── AppDbContext.cs
│   │   └── Entities.cs
│   ├── Options
│   │   ├── AuthOptions.cs
│   │   ├── InternalOptions.cs
│   │   └── StorageOptions.cs
│   ├── Services
│   │   ├── ArtifactService.cs
│   │   ├── AuthService.cs
│   │   ├── ChatService.cs
│   │   ├── JobService.cs
│   │   ├── ProjectService.cs
│   │   ├── SearchService.cs
│   │   └── UploadService.cs
│   ├── Storage
│   │   ├── IFileStorage.cs
│   │   └── LocalFileStorage.cs
│   ├── Migrations
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Dockerfile
│   └── GnosisKernel.Api.http
```

### Running the API

From the project root:

```bash
dotnet restore
dotnet run --project GnosisKernel.Api
```

The API will start on:

```text
https://localhost:5001
http://localhost:5000
```

You can test endpoints using:

- Swagger (if enabled)
- `GnosisKernel.Api.http`
- Postman / Insomnia
- Frontend application

---

## Architecture Overview

The backend follows a layered, responsibility-driven architecture:

```text
Controllers → Services → Data / External Clients
```

### Key Principles

- **Controllers are thin** (HTTP + validation only)
- **Services contain business logic**
- **Contracts define API boundaries**
- **Data layer is isolated** via `AppDbContext`

---

## Core Domains & Services

### Authentication

**Location:** `Auth/`, `Services/AuthService.cs`

**Features:**

- JWT-based authentication
- Password hashing
- Internal API key authentication for background jobs

**Key components:**

- `JwtTokenService`
- `PasswordHasher`
- `InternalKeyAuthAttribute`

---

### Projects & Artifacts

**Location:** `ProjectsController`, `ArtifactsController`, `ArtifactService`

**Responsibilities:**

- Project lifecycle management
- Artifact metadata tracking
- Linking uploaded files to projects
- Persisting symbol and chunk data

---

### Search & Indexing

**Location:** `SearchController`, `SearchService`, `JobsController`

**Responsibilities:**

- Full-text and semantic search
- Indexing job orchestration
- Background processing coordination
- Symbol, chunk, and relationship lookup

**Indexing jobs:**

- Parse uploaded files
- Extract symbols
- Build dependency graphs
- Store searchable chunks

---

### Chat

**Location:** `ChatController`, `ChatService`

**Responsibilities:**

- Conversational queries over indexed code
- Context assembly from search results
- Prompt construction
- Streaming or batched AI responses

---

## API Design

### Controllers

Each controller corresponds to a bounded domain:

| Controller | Responsibility |
|-----------|----------------|
| `AuthController` | Login, token issuance |
| `ProjectsController` | Project CRUD |
| `UploadsController` | File upload & ingestion |
| `ArtifactsController` | Artifact metadata |
| `SearchController` | Search queries |
| `ChatController` | AI-assisted queries |
| `JobsController` | Indexing job lifecycle |
| `InternalJobsController` | Internal-only operations |

---

### Contracts

Located in `Contracts/`, these define:

- Request DTOs
- Response DTOs
- Stable API boundaries

This avoids leaking EF entities to the API surface.

---

## Technical Decisions

### Why ASP.NET Core?

- High performance
- Strong typing and tooling
- Excellent async and concurrency model
- First-class DI support

### Why Layered Architecture?

- Clear separation of concerns
- Testability
- Long-term maintainability
- Easy onboarding for new contributors

### Why Abstract Storage & AI?

- Storage can switch between local, S3, Azure Blob
- AI provider can evolve independently
- Easier testing and mocking

---

## Configuration

Configuration is managed via:

- `appsettings.json`
- `appsettings.Development.json`
- Environment variables

### Key Options

| Option | Description |
|--------|-------------|
| `AuthOptions` | JWT secrets, token lifetime |
| `InternalOptions` | Internal API keys |
| `StorageOptions` | File storage configuration |

---
