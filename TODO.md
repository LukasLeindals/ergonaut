# TODO

MVP-first: only items required for the first-edition functional prototype. Non-MVP work lives in `FUTURE_WORK.md`.

## Milestone 1 — Prototype Stability
- Refactor LogIngestion to clearly separate adapters, processors, and exporters.
- Ensure async usage is correct (awaiting, cancellation, avoiding thread-blocking paths).
- Apply concepts from the teaching materials where appropriate; refactor if needed.
- Clean up authentication/authorization: use long-lived tokens where required; remove any hardcoded secrets.

## Milestone 2 — Release Prep
- Re-evaluate placement of cross-cutting services (e.g., `WorkItemCreator`) in the project structure.

## Notes
- Group tasks by independent milestones; order items by priority/dependency within each group.

