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

## Installation

```bash
git clone <your-repo-url>
cd XZone_1
dotnet restore XZone.sln
```

Configure `appsettings.json` or **user secrets** (recommended for secrets) as described below, then apply migrations and run.

---

## Configuration

Sensitive values in the repo use placeholders. **Do not commit real secrets.**

| Section | Keys | Purpose |
|---------|------|---------|
| **ConnectionStrings** | `cs` | SQL Server connection for EF Core |
| **JWT** | `Key`, `Issuer`, `Audience`, `DurationInDays` | Symmetric JWT validation (`Key` / issuer / audience are required). *Note: access token lifetime in code is fixed at 1 hour in `AuthService`; align code and config if you need `DurationInDays`.* |
| **Stripe** | `SecretKey`, `PublishableKey`, `WebhookSecret`, `SuccessUrl`, `CancelUrl` | Server-side Stripe API; success/cancel URLs for Checkout redirects. `WebhookSecret` is present for future webhook use—**no Stripe webhook endpoint is implemented in this codebase**; confirmation uses the Checkout session API from `success` / `confirm` actions. |
| **EmailSettings** | `Host`, `Port`, `SenderEmail`, `SenderName`, `AppPassword` | SMTP (defaults target Gmail-style hosts in `appsettings.json`) |

**Development tip:** use `dotnet user-secrets` in the `XZone` project folder to override connection strings and API keys locally.

---

## Database and migrations

The database is created and evolved through **EF Core migrations** under `XZone/Infrastructure/Migrations/`. Identity tables and seed data (including roles such as **Admin** and **User**) are part of that history.

From the repository root:

```bash
cd XZone
dotnet ef database update
```

If the `dotnet-ef` tool is not installed:

```bash
dotnet tool install --global dotnet-ef
```

Ensure the **`cs`** connection string points at your SQL Server instance before updating.

---

## Run locally

```bash
cd XZone
dotnet run
```

Default HTTP profile (see `Properties/launchSettings.json`): **http://localhost:5047**  
Swagger UI (Development only): **`/swagger`**

Use Swagger’s **Authorize** button and pass `Bearer <your_jwt>` for protected endpoints.

---

## API overview

Base route pattern: **`/api/{Controller}`** (ASP.NET Core conventional casing).

### Auth (`/api/Auth`)

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/Register` | Anonymous | Create account; assign **User** role if it exists |
| POST | `/Login` | Anonymous | Returns JWT + refresh token payload |
| POST | `/LogOut` | Anonymous | Sign-in manager sign-out |
| POST | `/ResetPassword` | Anonymous* | Change password (*requires authenticated user id from claims in practice*) |
| POST | `/AddRole` | Anonymous | Assign role to user by id (treat as privileged in production) |
| GET | `/GetRefreshToken` | JWT | Rotate refresh token; sets **RefreshToken** cookie |
| POST | `/RevokeToken` | Anonymous | Revoke refresh token (body or cookie) |

### Games (`/api/Game`) — JWT required for all actions listed

| Method | Route | Roles | Description |
|--------|-------|-------|-------------|
| GET | `/` | Any authenticated | Paged/filtered games (`GameQueryParameters`) |
| GET | `/All` | Any authenticated | All games (includes category navigation) |
| GET | `/{id}` | Any authenticated | Game by id |
| POST | `/` | **Admin** | Create game (requires device ids) |
| PUT | `/{id}` | **Admin** | Update game |
| DELETE | `/{id}` | **Admin** | Delete game |

### Cart (`/api/Cart`) — JWT required

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/` | Current user’s cart |
| POST | `/add` | Add line |
| PUT | `/update-quantity` | Update quantity |
| DELETE | `/remove/{gameId}` | Remove line |
| DELETE | `/clear` | Empty cart |

### Checkout (`/api/Checkout`)

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/` | JWT | Create order + Stripe Checkout session |
| POST | `/confirm` | Anonymous | Confirm payment by Stripe session id |
| GET | `/success` | Anonymous | Confirm payment (query `sessionId`) |
| GET | `/cancel` | Anonymous | Cancel redirect acknowledgement |

### Orders (`/api/Order`)

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/my-orders` | JWT | List current user’s orders |
| GET | `/{orderId}` | JWT* | Order detail (*implementation checks user id*) |
| GET | `/admin/all` | **Admin** | All orders |
| GET | `/admin/{orderId}` | **Admin** | Admin order detail |
| POST | `/{id}/Cancel` | *Not attributed in code* | Cancel order for current user id from claims |

### Categories (`/api/Category`) — **Admin** only

CRUD: `GET /`, `GET /{id}`, `POST /`, `PUT /{id}`, `DELETE /{id}`.

### Devices (`/api/Device`) — **Admin** only

CRUD: `GET /`, `GET /{id}`, `POST /`, `PUT /{id}`, `DELETE /{id}`.

---

## Screenshots

_Add screenshots here (Swagger UI, sample requests/responses, or a future client app)._

Suggested captures:

1. Swagger overview with authenticated request  
2. Successful checkout session response (redact secrets)  
3. Order detail after payment  

---

## Roadmap and future improvements

Ideas grounded in the current codebase (not all are implemented):

- **Map SignalR hubs** and implement real-time notifications (hub class exists but is not registered on the HTTP pipeline).
- **Stripe webhooks** signed with `WebhookSecret` for idempotent, server-to-server payment events (config key exists without a controller).
- **Hardening:** `[Authorize]` on sensitive order/auth endpoints, lock down `AddRole` / `RevokeToken`, enable `RequireHttpsMetadata` for JWT in production, align JWT lifetime with configuration.
- **Public catalog:** optional `[AllowAnonymous]` for read-only game endpoints if the product should allow browsing without login.
- **Tests:** unit tests for `Order` rules and integration tests for checkout and refunds.
- **DevOps:** Dockerfile / CI pipeline, environment-specific `appsettings`, structured logging.
- **API consistency:** standardized status codes (e.g. `201` for register) and pagination metadata on all list endpoints.

---

## Author

**Maintainer:** *[Your name]*  
**Links:** *[Portfolio / GitHub / LinkedIn]*  

Replace the placeholders above with your preferred contact information.

---

## License

Specify your license in a `LICENSE` file at the repository root (e.g. MIT, Apache-2.0).
