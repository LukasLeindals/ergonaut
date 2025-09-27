# Project Layer Responsibilities

This document summarizes the primary responsibilities of the three main projects in the solution so it is easy to preserve layering boundaries as the system evolves.

## Ergonaut.Api
- Hosts the ASP.NET Core web application and exposes HTTP endpoints.
- Configures transport concerns such as authentication, authorization, middleware, and Swagger.
- Delegates all business behavior to the application layer instead of implementing it directly.

## Ergonaut.App
- Implements the application/business layer for reusable use cases.
- Defines service interfaces (for example `IProjectService`) and orchestrates domain operations using repositories and domain models.
- Avoids UI and transport specifics so both API and UI clients can depend on the same logic.

## Ergonaut.UI
- Provides the Blazor front end responsible for rendering components and handling user interactions.
- Calls the application layer through adapters (HTTP clients, view models) to fetch data and submit commands.
- Manages presentation concerns like view state and validation without embedding business logic.
