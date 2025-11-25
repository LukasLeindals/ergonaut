# Ergonaut Prototype Report
*(Aligned to the six-chapter structure used in DTU 62413 course reports)*

## 01_Intro
### 1.0 Hook: Why Ergonaut Matters (read this first)
- [ ] One-paragraph “catch” that ties Ergonaut to real pain: scattered tasks, siloed automation, brittle domain rules, and slow incident triage.
- [ ] Crisp value prop: Sentinel-first automation that reuses the domain core; API/UI act as governed surfaces to review and act on Sentinel-created work.
- [ ] Short promise of evidence: point to architecture diagram, demo path, and metrics sections for proof.
- [ ] FIGURE (medium): A before/after sketch showing chaotic workflows vs. Ergonaut’s unified, automated flow.
- [ ] External references that validate observability-driven ticket automation (for comparison and credibility): [Automatically create a Jira Software issue from a detected problem in Dynatrace](https://www.atlassian.com/devops/observability-tutorials/jira-dynatrace-issue); [Send Dynatrace notifications to Jira](https://docs.dynatrace.com/docs/analyze-explore-automate/notifications-and-alerting/problem-notifications/jira-integration); [Rootly & Jira Integration: Auto‑Create Incident Tickets](https://rootly.com/sre/rootly-jira-integration-auto-create-incident-tickets); [Atlassian + Dynatrace: automate issue creation from Dynatrace problems](https://www.atlassian.com/solutions/devops/integrations/dynatrace); [Atlassian Marketplace case: Dynatrace Integration for Jira reduces MTTR via auto ticketing](https://marketplace.atlassian.com/archive/1223165).

### 1.1 Purpose and Problem Framing
- [ ] State the real-world need: Sentinel-driven detection → triage → task creation that respects domain rules, with API/UI as the auditable control panels.
- [ ] Position this submission as a functional prototype deliverable; clarify scope vs. full product.
- [ ] Frame the primary research/engineering questions the prototype addresses.

### 1.2 Objectives and Success Criteria
- [ ] Measurable goals: end-to-end task lifecycle, automation-generated tasks, authenticated API/UI access.
- [ ] Acceptance criteria for each goal (e.g., API response codes, UI behaviors, automation latency).
- [ ] Non-goals for this iteration; point to `FUTURE_WORK.md`.

### 1.3 Stakeholders and Audience
- [ ] Engineering, product, operations—what each needs from the report.
- [ ] Reader assumptions (C#/Blazor/DDD familiarity); provide glossary pointer.
- [ ] Roles/responsibilities matrix for review and decision making.

### 1.4 System Overview
- [ ] Layer map (Sentinel-first): Sentinel worker (automation entry), `Ergonaut.Core` (domain), `Ergonaut.App` (use cases), `Ergonaut.Infrastructure` (EF Core), `Ergonaut.Api` (REST/JWT), `Ergonaut.UI` (Blazor Server).
- [ ] Default runtime context: SQLite at `data/sqlite/ergonaut.db`, Docker stack via `just run-docker`.
- [ ] Primary user journey summarised for orientation.
- [ ] FIGURE (high): Architecture + deployment + trust boundaries combined diagram (`docs/report/figures/architecture_topology_security.puml`).
- [ ] Sentinel end-to-end flow reference: see combined sequence (`docs/report/figures/flows_api_automation.puml`) for automation-first narrative.
- [ ] References: README overview, `docs/uml` diagrams, and `examples/sentinel-python` for automation pattern.

### 1.5 Constraints and Assumptions
- [ ] Time-boxed prototype deadline; dev-oriented auth tokens; dependence on an OTLP collector.
- [ ] Tech constraints: .NET 6+, Blazor Server model, EF Core migrations, single-instance deployment during prototype.
- [ ] Operational assumptions: dev TLS posture, log retention defaults, single-tenant expectation.
- [ ] References: security/config guidance in `README.md` and any `.env` generation notes from `just run-docker`.

## 02_Design
### 2.1 Architectural Style and Quality Attributes
- [ ] DDD layering, separation of concerns, loose coupling via interfaces/DTOs.
- [ ] Target qualities: maintainability, testability, evolvability; acknowledged trade-offs vs. performance.
- [ ] Key architectural decisions/ADRs with rationale.

### 2.2 Domain Model
- [ ] Core aggregates: Project, Task, AutomationRule; invariants and lifecycle rules.
- [ ] Value objects/enums: status, priority, rule conditions; domain events if present.
- [ ] How domain rules prevent invalid state and duplication.
- [ ] FIGURE (high): Domain aggregates mapped to persistence (`docs/report/figures/domain_and_data.puml`).

### 2.3 Application Layer
- [ ] Command/query services, validation pipeline, transaction boundaries.
- [ ] Mapping strategy between domain and DTOs; anti-corruption boundaries to external clients.
- [ ] Error handling strategy and propagation of cross-cutting concerns (logging, auth context).

### 2.4 Infrastructure
- [ ] EF Core DbContext, repository abstractions, migration strategy; path to swap providers from SQLite.
- [ ] Configuration/secrets: data-protection keys, `.env` generation during bootstrap.
- [ ] Resiliency and resource management plans (retries, timeouts, caching, connection limits).

### 2.5 API Layer
- [ ] REST surface, JWT auth flow, middleware stack (auth, exception, logging, CORS if used); positioned primarily as Sentinel’s governed ingress/egress and as the UI’s backend.
- [ ] Versioning stance and error/response conventions; exemplar endpoint shapes.
- [ ] Input validation strategy, rate limiting posture, pagination/ filtering conventions.
- [ ] FIGURE (medium): Combined automation vs API task flow sequence (`docs/report/figures/flows_api_automation.puml`).

### 2.6 UI Layer
- [ ] Blazor Server topology, navigation map, data-fetch patterns, and state handling; emphasize UI as a review/override surface for Sentinel-generated tasks rather than a standalone task board product.
- [ ] Accessibility/UX considerations; responsive design notes; prototype-level shortcuts.
- [ ] Client vs. server validation balance and error-handling UX.
- [ ] FIGURE (medium): UI sitemap/wireframe collage (`docs/report/figures/ui_sitemap.puml`) showing main pages and navigation to contextualize the user journey.

### 2.7 Sentinel Automation
- [ ] OTLP ingestion → rule evaluation → task creation via App services; highlight Sentinel as the primary differentiator and source of most task flow.
- [ ] Deduplication using `messageTemplate`; throttling/flood protection strategy.
- [ ] Deployment topology expectations and security boundaries for the worker.
- [ ] FIGURE (high): Automation flow is included in combined sequence (`docs/report/figures/flows_api_automation.puml`).

### 2.8 Cross-Cutting Concerns
- [ ] Security: authN/Z boundaries, least-privilege defaults, secret storage approach.
- [ ] Observability: logs/metrics/traces plan, correlation IDs, dashboard needs.
- [ ] Performance/scalability: expected load, performance budgets, caching decisions.
- [ ] Privacy/compliance considerations (PII handling, retention), and brief threat model.
- [ ] FIGURE (medium): Trust boundaries and data paths consolidated in `docs/report/figures/architecture_topology_security.puml`.

## 03_Implementation
### 3.1 Codebase Tour
- [ ] Layer-to-folder mapping: `src/Ergonaut.Core`, `Ergonaut.App`, `Ergonaut.Infrastructure`, `Ergonaut.Api`, `Ergonaut.UI`, `examples/sentinel-python`.
- [ ] Location of shared contracts/DTOs/interfaces and cross-layer utilities.
- [ ] Notable third-party dependencies and their roles.
- [ ] FIGURE (low): (Optional) dependency graph — not yet drawn to keep figures lean.

### 3.2 Patterns and Key Components
- [ ] Entities/value objects enforcing rules; repositories abstracting persistence; services orchestrating use cases.
- [ ] Validation/mapping tools (e.g., AutoMapper if present); transaction scopes and unit of work usage.
- [ ] Extension points/hooks intended for customization.

### 3.3 Data Storage
- [ ] Default SQLite file path, migration application, seeding/initialization behavior.
- [ ] Steps and configuration knobs to swap to another provider; connection string patterns.
- [ ] Backup/restore approach and migration rollback strategy.
- [ ] FIGURE (medium): Persistence view included inside `docs/report/figures/domain_and_data.puml`.

### 3.4 API Details
- [ ] Representative requests/responses for auth and project/task CRUD; automation-related endpoints (Sentinel posting tasks/events, querying automation status).
- [ ] Middleware configuration and policies (auth, exception, logging, CORS).
- [ ] Error codes taxonomy, pagination, filtering, and sorting conventions.

### 3.5 UI Details
- [ ] Main pages/components and their backing services; loading/error states.
- [ ] State persistence model (server-side circuits) and any client interactions.
- [ ] Input validation UX, form behaviors, navigation edge cases; clarify UI’s role as visibility and governance over Sentinel output (not the primary capture surface).
- [ ] FIGURE (low): Annotated UI screenshot of the task review page (state badges, provenance labels) — use real UI capture if available; fallback to mock.

### 3.6 Automation Flow
- [ ] Sentinel worker configuration, OTLP endpoint expectations, rule match flow to App services (central scenario for the system).
- [ ] Failure handling/retry semantics and observability hooks.
- [ ] Minimal run instructions (`just run-docker`, environment variables).
- [ ] FIGURE (medium): Deployment covered inside `architecture_topology_security.puml`.

### 3.7 Tooling and Commands
- [ ] Dev ergonomics: `just run-docker`, exposed ports (5075 API, 5242 UI), generated env files.
- [ ] Build/test commands, lint/static analysis tools, and code style enforcement (if configured).
- [ ] Debugging/test shortcuts or scripts.

### 3.8 Testing
- [ ] Current unit/integration tests in `tests`: coverage focus, mocking approach.
- [ ] Test data management, fixtures, isolation strategy.
- [ ] CI pipeline status (if any) and how tests integrate; gaps to close next (auth edges, automation correctness, UI e2e).
- [ ] FIGURE (low): Test balance (`docs/report/figures/test_pyramid.puml`).

### 3.9 Reproducibility and Environment
- [ ] Required toolchain versions (.NET SDK, Node if used), OS assumptions.
- [ ] Steps to provision local/CI environments; handling of secrets in non-prod.
- [ ] Configuration profiles for dev vs. release; feature flags if applicable.

## 04_Result
### 4.1 Prototype Status
- [ ] Working end-to-end flows and stability level; notable rough edges.
- [ ] Defect list snapshot with severity.
- [ ] FIGURE (medium): Kanban/status board screenshot from the UI (or prototype) showing automation-tagged tasks; use live capture.

### 4.2 Demonstration Path
- [ ] Stepwise walkthrough: start stack → authenticate → emit OTLP event → Sentinel creates/updates task → review/verify in UI/API (task CRUD as supporting flows).
- [ ] Expected observable outputs (API responses, UI screens) and success checkpoints.
- [ ] FIGURE (medium): Demo swimlane aligns with `flows_api_automation.puml` (reuse).

### 4.3 Metrics and Evidence
- [ ] Latency/throughput observations, error rates, coverage snapshot.
- [ ] Resource usage notes (CPU/memory) under demo load; bottlenecks observed.
- [ ] Missing measurements and how to gather them.
- [ ] FIGURE (high): Performance charts — generate from measurements; not UML.

### 4.4 Deferred Items
- [ ] Items intentionally postponed with links to `FUTURE_WORK.md`.
- [ ] How deferrals affect interpretation of results.

### 4.5 Validation and Acceptance
- [ ] Manual checks or scripted acceptance tests performed; pass/fail status.
- [ ] Known failing scenarios and their severity/impact.
- [ ] FIGURE (low): Acceptance checklist — capture as table/screenshot; not UML.

## 05_Discussion
### 5.1 Strengths
- [ ] Clean layering, shared App services, domain purity; developer experience benefits.
- [ ] Reuse across API/UI/Sentinel and ease of onboarding.

### 5.2 Limitations
- [ ] Single-DB default, limited role model, modest UI polish, partial observability; dependence on OTLP availability.
- [ ] Gaps in horizontal scalability and offline/edge scenarios.

### 5.3 Trade-offs Made
- [ ] Choices driven by prototype deadline (SQLite, Blazor Server, minimal caching) and their implications for scale and ops.

### 5.4 Risks and Mitigations
- [ ] Data consistency, auth hardening, automation false positives; mitigation plans and owners.
- [ ] Operational risks (log volume spikes, collector downtime) and fallbacks.
- [ ] FIGURE (medium): Risk matrix — to be created as chart; not UML.

### 5.5 Alternatives Considered
- [ ] Brief rationale for rejected options (frontend tech, telemetry ingestion alternatives, database choices).

### 5.6 Testing Gaps and Plan
- [ ] Priority test additions: auth edge cases, automation rule validation, UI e2e, load tests.
- [ ] Proposed tooling or frameworks to close gaps.

### 5.7 Threats to Validity
- [ ] Internal/external validity concerns for results and potential biases.

### 5.8 Lessons Learned
- [ ] Process and technical lessons; what to adjust in next iteration.

### 5.9 Ethical and Sustainability Considerations
- [ ] Data/privacy handling, auditability, potential misuse risks.
- [ ] Operational sustainability (energy/resource use) and long-term maintenance posture.
- [ ] FIGURE (low): Data lineage covered by trust/data paths in `architecture_topology_security.puml`; extend if PII scope expands.

## 06_Conclusion
### 6.1 Prototype Viability
- [ ] Whether objectives were met; evidence pointers in prior sections.
- [ ] Readiness assessment for MVP transition.

### 6.2 Decisions Needed
- [ ] Pending decisions (DB choice for prod, hosting model, auth/role expansion, observability tooling) and suggested owners.

### 6.3 Next Steps
- [ ] Prioritized backlog: critical fixes, test additions, deployment hardening, UX polish.
- [ ] Indicative milestones/timelines if known.

### 6.4 Release Readiness Checklist
- [ ] Security, testing, documentation, and operational checks required before promoting beyond prototype.

### 6.5 Future Work
- [ ] Link and briefly summarize `FUTURE_WORK.md`; categorize by impact/effort.

### 6.6 References
- [ ] Pointers to `docs/uml`, `FUTURE_WORK.md`, key source folders, and example configs/scripts.
- [ ] Cite external frameworks/libraries and standards followed.
