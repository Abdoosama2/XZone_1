# XZone

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-Specify%20license-lightgrey)](LICENSE)

**XZone** is a **REST API** for a **digital game store**: users browse a catalog (with filters and pagination), manage a **per-user shopping cart**, place **orders**, and pay through **Stripe Checkout**. **ASP.NET Core Identity** supplies accounts and roles (**User** / **Admin**); **JWT** protects most customer flows, with **refresh tokens** stored on the user and delivered via **HTTP-only cookies** for rotation.

---

## Table of contents

- [Overview](#overview)
- [Key features](#key-features)
- [Tech stack](#tech-stack)
- [Architecture](#architecture)
- [Repository structure](#repository-structure)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Database and migrations](#database-and-migrations)
- [Run locally](#run-locally)
- [API overview](#api-overview)
- [Screenshots](#screenshots)
- [Roadmap and future improvements](#roadmap-and-future-improvements)
- [Author](#author)

---

## Overview

XZone models **games** linked to **categories** and **platforms (devices)**, tracks **inventory**, and persists **carts** and **orders** in **SQL Server** via **Entity Framework Core**. Checkout creates a **pending** order, opens a **Stripe Checkout Session** (metadata ties the session to the order), and **confirms** payment by verifying the session with Stripe—then decrements stock, clears the cart, and sends **transactional email** (MailKit). **Paid** order cancellation triggers **Stripe refunds** and **stock restoration** inside a database transaction.

Swagger UI is enabled in the **Development** environment for interactive API exploration.

---

## Key features

| Area | What the API does |
|------|---------------------|
| **Identity** | Register, login, JWT access token, refresh token rotation, revoke refresh token, password change, role assignment helper |
| **Catalog** | Games with category and devices; **paged list** with search, category filter, price range, sort (name / price / category) |
| **Admin** | CRUD for games, categories, and devices (role **Admin**) |
| **Cart** | One cart per user; add, update quantity, remove line, clear |
| **Checkout** | Validate cart and stock → create order → Stripe session URL → optional confirm/success callbacks |
| **Orders** | User order history and detail; admin order list/detail; cancel (pending vs paid + refund path) |
| **Email** | HTML notifications on checkout initiation, payment confirmation, and cancellation (via MailKit) |

---

## Tech stack

- **Runtime:** .NET 9 (`net9.0`)
- **Web framework:** ASP.NET Core (minimal hosting in `Program.cs`, attribute routing, controllers)
- **Data:** Entity Framework Core 9, SQL Server provider
- **Auth:** ASP.NET Core Identity, JWT Bearer, role-based authorization
- **Payments:** Stripe.net (Checkout Sessions, refunds)
- **Email:** MailKit + MimeKit
- **Mapping:** AutoMapper
- **API docs:** Swashbuckle (Swagger / OpenAPI) with JWT security scheme
- **Other:** SignalR package registered; a `NotificationHub` class exists (hub endpoints are not mapped in `Program.cs`)

---

## Architecture

The solution follows a **layered / n-tier** layout common in ASP.NET projects:

1. **`Api`** — HTTP surface: controllers, shared response types (`ApiResponse<T>`).
2. **`Application`** — use cases: services, DTOs, AutoMapper profiles, service interfaces.
3. **`Domain`** — entities (e.g. `Game`, `Order`), enums, repository abstractions (`IGameRepository`, `IUnitofWork`, …).
4. **`Infrastructure`** — EF Core `AppDbContext`, concrete repositories, Identity `ApplicationUser`, migrations.

**Patterns in use**

- **Repository pattern** — generic `Repository<T>` plus entity-specific repositories.
- **Unit of Work** — `IUnitofWork` exposes `SaveChangesAsync` and `BeginTransactionAsync` for checkout, payment confirmation, and cancellation flows.
- **Service layer** — controllers depend on `I*Service` interfaces; implementations coordinate repositories, Stripe, and email.
- **DTOs + AutoMapper** — API and persistence shapes are separated from domain entities where mapping is configured.
- **Rich domain model (partial)** — `Order` encapsulates state transitions (payment succeeded/failed, cancel, refund rules) and item collection behavior.

The layout is **inspired by clean architecture** (direction of dependencies mostly flows inward via interfaces) but it is **not strict Clean Architecture / DDD bounded contexts**: for example, the `Order` entity references `ApplicationUser` from the Infrastructure identity layer, and some controllers/services reference concrete infrastructure types—tightening those boundaries would be a natural evolution.

**CQRS:** not implemented (single model for reads and writes).

---

## Repository structure

```text
XZone_1/
├── XZone.sln                 # Visual Studio solution
└── XZone/                    # ASP.NET Core web project
    ├── Api/
    │   ├── Controllers/      # Auth, Game, Cart, Checkout, Order, Category, Device
    │   └── Models/           # ApiResponse, AuthResponse, …
    ├── Application/
    │   ├── DTO/              # Request/response and query DTOs
    │   ├── Mapping/          # AutoMapper profiles
    │   └── Services/         # Business orchestration (+ IServices)
    ├── Domain/
    │   ├── Entites/          # Game, Order, Cart, …
    │   ├── Enums/            # OrderStatus, PaymentStatus
    │   └── Interfaces/       # Repository and unit-of-work contracts
    ├── Infrastructure/
    │   ├── Data/             # AppDbContext
    │   ├── Identity/         # ApplicationUser
    │   ├── Migrations/       # EF Core migrations
    │   └── Repository/       # Repository & UnitOfWork implementations
    ├── Properties/
    │   └── launchSettings.json
    ├── Program.cs            # DI, middleware, JWT, Swagger
    ├── appsettings.json      # Example configuration (replace secrets)
    └── XZone.csproj
```

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB, Express, or full instance) reachable from the connection string
- A [Stripe](https://stripe.com/) account (test keys are fine for local development)
- (Optional) SMTP credentials (e.g. Gmail app password) for outbound email

---



CRUD: `GET /`, `GET /{id}`, `POST /`, `PUT /{id}`, `DELETE /{id}`.

---


