# Minimal Sentinel Implementation Plan

## Assess Existing Ingestion Flow
- Review `src/Ergonaut.App/LogIngestion/LogEventHub.cs` to confirm subscription mechanics and backpressure behavior.
- Inspect `src/Ergonaut.App/LogIngestion/OtlpLogIngestionPipeline.cs` to understand how OTLP events are transformed before reaching the hub.
- Identify the host process that will run Sentinel so dependencies and configuration points are clear.

## Define Sentinel MVP Components
- Create `SentinelOptions` to hold project ID, severity threshold, and allowed log sources.
- Draft `ILogEventFilter` for minimal severity/source checks and register a baseline implementation.
- Introduce `ISentinelTaskFactory` that converts an `ILogEvent` into `CreateWorkItemRequest` with `WorkItemSourceLabel.Sentinel`.
- Wrap `IWorkItemService` in an `ISentinelTaskCreator` adapter to encapsulate persistence concerns.

## Implement Sentinel Worker Pipeline
- Add `SentinelWorker : BackgroundService` that subscribes to `ILogEventSource` and manages cancellation and disposal.
- Inside the worker loop, apply `ILogEventFilter`; immediately drop events that fail the criteria.
- For accepted events, call `ISentinelTaskFactory` to build the request and pass it to `ISentinelTaskCreator` to create the work item.
- Log accept/drop outcomes and failures to aid future diagnostics.

## Configure and Wire Dependencies
- Register Sentinel options (e.g., from configuration section `Sentinel`) and validate required fields at startup.
- Add the filter, task factory, and task creator to DI with scoped or singleton lifetimes as appropriate.
- Register `SentinelWorker` as a hosted service in the worker host that already resolves `ILogEventSource`.
- Ensure configuration provides the Sentinel project ID and minimal filter settings for the deployment environment.
