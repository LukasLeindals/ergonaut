# Log Stream Enablement Roadmap

## Overview
- Stand up a lightweight, Datadog-inspired log monitor so Sentinel automation can consume real-time events without production dependencies.
- Keep the first iteration in-memory and deterministic; design contracts so future adapters can replay logs from Datadog’s APIs.
- Document configuration, testing, and guardrails so new contributors can extend the system safely.

## Domain Contracts (`src/Ergonaut.Core/Models/Logs/`)
- Extend `ILogEvent` with an immutable implementation (e.g., `LogEvent`) that includes `Timestamp`, `Message`, `MessageTemplate`, `LogLevel`, `string Source`, and optional metadata (`IReadOnlyDictionary<string,string?>`).
- Introduce `ILogEventSource` with:
  - `string Name { get; }`
  - `LogStreamOptions Options { get; }` (filters: minimum level, tags, throttle hints)
  - `IAsyncEnumerable<ILogEvent> ObserveAsync(CancellationToken ct = default)`
- Add supporting types:
  - `LogStreamOptions` record to carry filter configuration.
  - (Optional) `ILogEventSink` or `ILogEventObserver` interface if consumers need acknowledgment semantics.
- Map `LogLevel` to Datadog severities in XML doc comments so adapters have a canonical translation table.

## Application Layer Primitives (`src/Ergonaut.App/Sentinel/`)
- Implement an in-memory `LogEventHub` that manages sources, applies `LogStreamOptions`, and exposes `SubscribeAsync` returning `IAsyncEnumerable<ILogEvent>`.
- Build `InMemoryLogEventSource : ILogEventSource` backed by `Channel<ILogEvent>`:
  - `EnqueueAsync(ILogEvent log, CancellationToken ct = default)`
  - `Complete(Exception? error = null)`
  - `ObserveAsync` yields from the channel while honoring cancellation.
- Add `LogMonitorService` (or similar coordinator) to bridge the hub into Sentinel pipelines, ensuring downstream automation receives enriched events.
- Wire DI registrations in host startup (API/UI/worker) with feature flags to toggle the mock source on/off.

## Surfacing Events
- Expose a minimal API surface:
  - `GET /api/log-monitor/events/recent` returns the latest N events (in-memory or persisted).
  - `GET /api/log-monitor/events/stream` uses Server-Sent Events (SSE) or SignalR to push real-time updates to the UI.
- UI increment:
  - Add a diagnostic page displaying a live tail and filter controls (level, source).
  - Highlight sentinel-created work items to connect logs with automation outcomes.
- Ensure authentication and rate limits align with “Safe by Default” guidance to prevent accidental exposure of sensitive data.

## Datadog Adapter Preparation
- Provide a factory abstraction (`ILogEventSourceFactory`) that resolves sources based on configuration (`mock`, `datadog`, etc.).
- Document the planned Datadog integration points:
  - REST search (`/api/v2/logs/events/search`) for historical backfill.
  - Live tail or streaming API for near-real-time ingestion.
- Capture requirements:
  - API key/APP key handling (secure storage, rotation).
  - Pagination and cursor management for catch-up after downtime.
  - Rate limit behavior and exponential backoff strategy.
- Plan a persistence checkpoint (SQLite table or durable store) to record the last processed cursor and avoid duplicate ingestion.

## Testing & Safety
- Unit tests (`tests/Ergonaut.App.Tests/Sentinel/`) covering:
  - `InMemoryLogEventSource` enqueue/complete/cancel behavior.
  - `LogEventHub` fan-out, filter enforcement, and backpressure handling.
  - API endpoints returning expected payloads and respecting filters.
- Add smoke/integration tests once the sentinel ingestion pipeline consumes log events end-to-end.
- Validate malformed events and unknown severities are rejected or routed to a dead-letter path rather than crashing the pipeline.
- Update `TODO.md` with remaining open questions (e.g., retention policy, multi-tenant isolation).

## Incremental Delivery Checklist
1. Finalize core contracts and options in `Ergonaut.Core`.
2. Implement the in-memory hub + mock source; add unit tests.
3. Expose API/UI endpoints for monitoring (behind a feature flag).
4. Hook the sentinel automation pipeline into the hub for local loops.
5. Document configuration and ops expectations in `docs/log-monitoring.md`.
6. Prototype the Datadog adapter in a separate branch using sandbox credentials, then merge once hardened.
