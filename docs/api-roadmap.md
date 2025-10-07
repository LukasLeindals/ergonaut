# API Roadmap

## 1. Purpose
- Provide a stable HTTP interface for project/work item management.
- Decouple front-end releases from backend domain logic.
- Enable future consumers (CLI, automations, third-party integrations) without reworking persistence.

## 2. Consumers
- Current: `Ergonaut.UI` Razor components.
- Near-term: Internal tools or scripts.
- Future: External services (e.g., Home Assistant, partner apps).

## 3. Functional Scope (Initial Release)
- Projects
  - GET /projects — list summaries
  - POST /projects — create project
  - GET /projects/{id} — fetch detail (TBD)
  - PUT /projects/{id} — update title (TBD)
  - DELETE /projects/{id} — archive/remove (TBD)
- Work Items (backlog)
  - CRUD endpoints mirroring projects once work item model stabilizes.
- Validation & error contract
  - Standard problem-details payload for failures.
  - Data annotations reused from application layer.

## 4. Non-Functional Goals
- Authentication: JWT bearer (HS256) issued by first-party auth service.
- Authorization: Role/claim-based policies; minimum scope `projects:read` / `projects:write`.
- Versioning: Start with `v1` route prefix; align SemVer with breaking changes.
- Logging & Observability: Structured logging (Serilog) and request tracing (correlation IDs).
- Documentation: OpenAPI spec generated via Swashbuckle; publish to `/swagger`.
- Deployment: Co-hosted with UI initially; plan for independent scaling once usage grows.

## 5. Architecture Notes
- Application services expose CRUD via `IProjectService` returning DTOs.
- API translates DTOs to JSON; persistence concerns stay in infrastructure layer.
- DTOs remain separate from EF entities to prevent leaking ORM state.

## 6. Open Questions / Risks
- [ ] Where do we source user identities? (ASP.NET Core Identity vs external IdP)
- [ ] Desired JWT lifetime and refresh strategy.
- [ ] Rate limiting requirements for public exposure.
- [ ] Strategy for breaking change communication/version negotiation.
- [ ] Hosting certificate management when split deployment happens.

## 7. Next Actions
1. Finalize DTO definitions and service interfaces.
2. Prototype minimal API project with JWT middleware configured.
3. Add integration tests verifying authenticated project listing/creation.
4. Revisit open questions once prototype is running.
