# Cleanup Before Log-Event Task Creation

## Required Cleanups
- **Parameterize the Kafka topic via environment variables.**  
  Strategy: introduce an `ERGONAUT_KAFKA_TOPIC` environment variable, surface it through configuration binding, and inject it into both the producer and consumer so the topic name no longer diverges between environments. Update docs and defaults to explain the precedence order (env → appsettings → fallback).

- **Consolidate log ingestion into a single Docker image/service.**  
  Strategy: replace the ad-hoc mix of Kafka/ZooKeeper containers with one curated image (or a slim compose profile) that provisions every dependency needed for log ingestion. Publish a `just` recipe that boots the stack and ensures topic creation happens automatically.

## Additional Findings To Tackle First
- **Enforce options validation for Kafka and Sentinel settings.**  
  Strategy: register `IValidateOptions` implementations (or use `Validate` delegates) so startup fails fast when required values like `BootstrapServers`, `Topic`, or `ProjectName` are missing. Add lightweight unit tests to guard against regressions.

- **Tidy the Sentinel worker’s event flow before adding task creation.**  
  Strategy: ensure `HandleEvent` short-circuits rejected events, logs accept/drop outcomes once, and routes accepted events through a dedicated adapter/service. Capture exceptions so the consumer loop keeps running.

- **Document the local ingestion pathway end-to-end.**  
  Strategy: write a short guide that covers starting the collector, producing a sample OTLP payload, and verifying that Kafka/Sentinel exchange messages. This removes ambiguity when the team later layers on automated task creation.
