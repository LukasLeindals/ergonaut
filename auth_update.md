# Auth Cleanup Plan (Lean)

## Findings from latest auth review (Nov 17, 2025)
- High — OTLP log ingest is completely open (`OtlpLogIngestionController` is `[AllowAnonymous]`); anyone can POST arbitrary logs or flood Kafka. Secure it with JWT bearer + ingest scope/api key.
- High — Refresh tokens are stored plaintext in a static in-memory dictionary and vanish on app recycle; not hashed, not shared across instances, and replay/memory-pressure prone. Persist + hash or drop refresh for service-to-service.
- High — Service “shared secrets” never expire; `ServiceTokenTtlMinutes` is defined but never used. If a token leaks, access is permanent. Add iat/exp to service tokens and enforce rotation.
- High — Signing material is committed: `.image/api/.env` includes the JWT signing key and service tokens. Move to user-secrets/env only, gitignore runtime env files, rotate everything exposed.
- Medium — `SigningKeyPath` implies RSA, but `GetSigningKey` always returns an HMAC key and will treat a PEM file as a raw string. Decide RSA vs symmetric; implement `RsaSecurityKey`/`RS256` or validate symmetric-only with length checks.
- Medium — Tokens carry only `sub`/`scope`; no `jti`, `iat`, audience/issuer hardening tweaks, or `ClockSkew` allowance. Add replay hints and small skew; keep access tokens short-lived.
- Low — `/auth/token` returns detailed Unauthorized text and has no throttling/rate limiting/backoff, easing brute-force. Make responses generic and add minimal rate limiting/delay.

## Immediate fixes to keep prototype safe
1) Secure OTLP ingestion with bearer auth + `logingestion:write` (or equivalent) scope; update collector to send the token.
2) Remove committed secrets, rotate the signing key and all service tokens, and load them via `dotnet user-secrets`/env only.
3) For service clients, use short-lived access tokens only (no refresh) until a durable, hashed refresh store exists.
4) Enforce service-token expiry by wiring `ServiceTokenTtlMinutes` into issued claims and validation.
5) Decide on signing strategy (RSA vs symmetric) and validate key material; add `jti`, `iat`, and small `ClockSkew`.

