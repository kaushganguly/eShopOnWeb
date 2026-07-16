# Assessment Overview

This directory contains supplementary analysis documents generated as part of the application assessment for **eShopOnWeb** — an ASP.NET Core 8 e-commerce reference application. These documents provide architectural context, dependency analysis, and workflow documentation to complement the AppCAT migration findings in `report.json`.

## Supplementary Documents

| Document | Description |
|----------|-------------|
| [architecture-diagram.md](architecture-diagram.md) | Two-layer architecture visualization: high-level application architecture (ASP.NET Core MVC, Blazor WASM, REST API, SQL Server) and detailed component relationship diagram showing controllers, services, repositories, and cross-cutting concerns |
| [dependency-map.md](dependency-map.md) | Visual map of all external NuGet dependencies grouped by functional category (Web Frameworks, Database/ORM, Security, API Documentation, Utilities) with version details, compatibility risks, and test dependencies |
| [api-service-contracts.md](api-service-contracts.md) | Catalog of all REST API endpoints (PublicApi and Web MVC), DTOs, communication patterns, security posture (JWT and Cookie authentication), and a sequence diagram of primary request flows |
| [data-architecture.md](data-architecture.md) | EF Core entity model with ER diagram, database configuration per environment, repository interfaces, caching strategy, data ownership boundaries, and PII/sensitivity classification |
| [configuration-inventory.md](configuration-inventory.md) | Comprehensive inventory of all configuration sources, runtime profiles (Development, Docker, Production), property keys, Azure Key Vault secrets provisioning workflow, and Docker Compose startup dependencies |
| [business-workflows.md](business-workflows.md) | End-to-end documentation of core business workflows (catalog browsing, basket management, order checkout, catalog administration), domain entity descriptions, business rules, guard clauses, and a Mermaid sequence diagram of the checkout flow |
