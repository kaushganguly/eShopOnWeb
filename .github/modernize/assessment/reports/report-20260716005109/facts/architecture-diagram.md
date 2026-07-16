# Architecture Diagram

This repository implements an e-commerce reference application with separate Web UI, Public API, and shared core/infrastructure libraries. The diagrams below summarize its runtime architecture and component interactions.

## Application Architecture

```mermaid
flowchart TD
    subgraph Client["Client Layer"]
        Browser["Browser"]
        BlazorAdminClient["Blazor Admin WebAssembly"]
    end

    subgraph App["Application Layer - ASP.NET Core net8.0"]
        WebApp["Web App MVC plus Razor Pages"]
        PublicApi["Public API Minimal API plus Controllers"]
        AppCore["ApplicationCore Domain Services"]
        Infra["Infrastructure EF Core plus Identity"]
    end

    subgraph Data["Data Layer"]
        SqlDb[("SQL Server Catalog and Identity DB")]
        MemoryCache[("In-Memory Cache")]
    end

    subgraph External["External Services"]
        KeyVault["Azure Key Vault"]
    end

    Browser -->|"HTTPS pages and API calls"| WebApp
    BlazorAdminClient -->|"HTTP API calls"| PublicApi
    WebApp -->|"domain use cases"| AppCore
    PublicApi -->|"domain use cases"| AppCore
    AppCore -->|"repositories"| Infra
    Infra -->|"EF Core queries"| SqlDb
    WebApp -->|"session and token caching"| MemoryCache
    WebApp -->|"production secrets"| KeyVault
    PublicApi -->|"JWT and identity auth"| Infra
```

### Technology Stack Summary

| Layer | Technology | Version | Purpose |
|---|---|---|---|
| Presentation | ASP.NET Core MVC, Razor Pages, Blazor Server and WASM | net8.0 | Web storefront and admin UI |
| API | ASP.NET Core MinimalApi.Endpoint + Controllers + Swagger | net8.0 | Catalog and auth API surface |
| Business | ApplicationCore with MediatR and Ardalis.Specification | net8.0 | Domain services and use-case orchestration |
| Data | EF Core + ASP.NET Identity + SQL Server provider | EF Core 8.0.2 | Persistence for catalog, basket, orders, identity |
| Security | ASP.NET Identity + JWT bearer auth | AspNet 8.0.2 | Authentication and authorization |

### Data Storage & External Services

The app uses SQL Server for both catalog/order data and identity data through two EF Core DbContexts. It uses in-memory caching for token and session-related data, and production mode can load secrets from Azure Key Vault for SQL connection strings.

### Key Architectural Decisions

- Uses layered architecture with ApplicationCore abstractions (`IRepository`, `IReadRepository`) and Infrastructure EF implementation (`EfRepository`).
- Splits external API responsibilities into a dedicated `PublicApi` project while keeping storefront MVC/Razor in `Web`.
- Supports environment-specific infrastructure wiring (local/docker SQL configuration vs Azure Key Vault-backed production configuration).

## Component Relationships

```mermaid
flowchart LR
    subgraph Presentation
        HomePage["Razor Pages and MVC Controllers"]
        UserController["UserController"]
        OrderController["OrderController"]
        CatalogItemEndpoint["CatalogItem Endpoints"]
    end

    subgraph Business["Business Logic"]
        BasketService["BasketService"]
        OrderService["OrderService"]
        CatalogViewModelService["CatalogViewModelService"]
        MediatorHandlers["MediatR Query Handlers"]
    end

    subgraph DataAccess["Data Access"]
        RepoAbstractions["IRepository and IReadRepository"]
        EfRepository["EfRepository"]
        CatalogContext["CatalogContext"]
        IdentityContext["AppIdentityDbContext"]
    end

    subgraph Infra["Infrastructure"]
        AuthMiddleware["Authentication and Authorization"]
        HealthChecks["Health Checks"]
        MemoryCacheComp["IMemoryCache"]
    end

    HomePage -->|"basket and catalog actions"| BasketService
    OrderController -->|"order queries"| MediatorHandlers
    CatalogItemEndpoint -->|"catalog CRUD"| RepoAbstractions
    UserController -->|"issue JWT and logout"| MemoryCacheComp
    BasketService -->|"read and update basket"| RepoAbstractions
    OrderService -->|"create order"| RepoAbstractions
    CatalogViewModelService -->|"catalog read"| RepoAbstractions
    RepoAbstractions -->|"implemented by"| EfRepository
    EfRepository -->|"query and save"| CatalogContext
    AuthMiddleware -.->|"protects"| Presentation
    HealthChecks -.->|"monitors"| Presentation
    IdentityContext -.->|"identity storage"| AuthMiddleware
```

### Component Inventory

| Component | Layer | Type | Responsibility |
|---|---|---|---|
| `OrderController` | Presentation | MVC Controller | Returns authenticated user order history and detail views |
| `UserController` | Presentation | API Controller | Returns current user info and handles logout/token cache invalidation |
| `CatalogItemListPagedEndpoint` | Presentation | Minimal API Endpoint | Serves paged catalog item API responses |
| `BasketService` | Business Logic | Domain Service | Adds items, updates quantities, and transfers baskets |
| `OrderService` | Business Logic | Domain Service | Converts basket contents into persisted orders |
| `GetMyOrdersHandler` / `GetOrderDetailsHandler` | Business Logic | MediatR Handlers | Query-side order projections for Web UI |
| `EfRepository<T>` | Data Access | Repository Implementation | Generic EF-based repository for aggregates |
| `CatalogContext` | Data Access | EF Core DbContext | Catalog, basket, and order persistence |
| `AppIdentityDbContext` | Data Access | Identity DbContext | ASP.NET Identity users/roles persistence |
| `ApiHealthCheck` / `HomePageHealthCheck` | Infrastructure | Health Check | Validates API and homepage availability |
