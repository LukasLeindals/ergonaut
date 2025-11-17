# Future Work

## Architecture & Patterns
- Evaluate MVVM (or similar) for the app structure if/when UI concerns grow.

## Examples
- Add sentinel page in ui  builds to show captured logs and created work items.
  - Should also be able to simulate log events from various adapters for testing (which is only shown in debug).
  - Set Sentinel configuration (e.g., severity threshold, deduplication window) at runtime.
- Add Python example application.
  - Ensure uncaught exceptions are captured and sent to Sentinel.
- Add C# example application (beyond the core prototype).

## Added Integrations
- **Datadog integration**: Ingest data from Datadog so Sentinel can create work items from Datadog logs/events.
- **Sync with popular task management systems**: allow automatic syncing of created work items with Trello, Jira, GitHub Issues, Slack Tasks.

## LLM Layer
- Implement an LLM layer to enhance work-item description generation and improve deduplication of similar log events.
- Explore the LLM's ability to analyze patterns in log data for more intelligent incident descriptions and possible solutions.

## Course Coverage Enhancements (Post-deadline)
- OOP / Delegates & Events (Ch. 5–13): introduce a `WorkItemCreated` domain event raised in `WorkItemService`, with subscribers (audit/notification) to showcase the `event` keyword and delegate patterns.
- Reflection / Late Binding (Ch. 16–17): add adapter discovery that loads `ILogEventAdapter` implementations via reflection or `AssemblyLoadContext`, illustrating attribute-driven activation and late binding.
- Parallelism / Threads (Ch. 14–15): enable bounded parallel handling in the Sentinel worker (e.g., `Parallel.ForEachAsync` or `Channel` + worker pool) to demonstrate multithreading beyond plain async.
