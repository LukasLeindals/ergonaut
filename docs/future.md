# Sentinel Task Flow Follow-Ups

## MVP Prerequisites
- Instrument a feedback loop for filtered-out events so relevance rules can be tuned with real data.
- Validate tasks before the duplicate check to keep malformed or low-severity items from entering the backlog.
- Ensure WorkItemService writes are idempotent, e.g., via version checks, to avoid clobbering analyst updates on retries.

## Post-MVP Opportunities
- Expand duplicate detection beyond direct backlog reads by leaning on richer search or throttling rules in WorkItemService.
- Model failure-handling paths (dead-letter queues, retry strategies) between pipeline stages to keep operations resilient under load.
