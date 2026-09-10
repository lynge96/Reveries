# Google Books integration

Operational notes for the Google Books API. The `openapi.yaml` spec in this folder is
the *contract* (endpoints and response shapes); this file is the *runtime* knowledge the spec
does not carry — auth, quotas, query qualifiers.

- **Official docs:** https://developers.google.com/books/docs/v1/using
- **OpenAPI spec:** [`openapi.yaml`](./openapi.yaml) — OpenAPI 3.0, trimmed to the
  `volumes` and `series` surface only.

## Configuration

Bound from the `GoogleBooks` config section (`GoogleBooksServiceCollectionExtensions.AddGoogleBooks`),
validated on start (`ApiUrl` required; `ApiKey` optional — Google Books allows unauthenticated
volume reads at a lower quota).

| Key | appsettings | Env override | Value |
|---|---|---|---|
| Base URL | `GoogleBooks:ApiUrl` | `GoogleBooks__ApiUrl` | `https://www.googleapis.com/books/v1/` |
| API key | `GoogleBooks:ApiKey` | `GoogleBooks__ApiKey` | Never committed; user-secrets (dev) / env (prod) |

The dependency's display name in logs is `GoogleBooks API` (`GoogleBooksSettings.DisplayName`); the
config section it binds from is `GoogleBooks` (`GoogleBooksSettings.SectionName`).

## Authentication

Public volume reads work without authentication (at a lower quota); when an `ApiKey` is configured
it is appended as the `key` **query parameter** (built inline in `GoogleBooksClient`, and omitted
entirely when the key is absent) — no OAuth flow:

```
GET volumes?q=isbn:9788700000000&key=<ApiKey>
```

## Quota & rate limits

- Default quota is **~1,000 requests/day per project**, plus a per-user rate limit; both are
  managed in the Google Cloud Console. Verify the current quota against your project.
- Quota-exceeded errors can surface as **HTTP 403** (`rateLimitExceeded`), not only `429`. Both are
  retried by the standard resilience handler (backoff + jitter); one that survives the retries
  surfaces as an `ExternalDependencyException` (via `ExternalApiReader`), not a silent `null`, so a
  genuine quota breach is visible rather than looking like an empty result. Timeouts are owned by
  the resilience pipeline (30s total, 10s per attempt).

## Endpoints in use

| Method | Path | Purpose |
|---|---|---|
| GET | `volumes?q=isbn:{isbn}` | ISBN lookup |
| GET | `volumes?q=intitle:"{title}"` | Title search |
| GET | `volumes/{volumeId}` | Single volume by id |

`q` accepts prefix qualifiers: `isbn:`, `intitle:`, `inauthor:`, `inpublisher:`, `subject:`.

## Spec provenance

- **Source:** Google API Discovery document,
  https://books.googleapis.com/$discovery/rest?version=v1 (converted to OpenAPI).
- **Retrieved:** 2026-08 — confirm.
- **Local edits:** reduced from the full API to the `volumes` (search + get) and `series`
  endpoints and the schemas they return; `Volume` trimmed to `volumeInfo` + identity/`searchInfo`
  (sale/access/user/layer/recommended info dropped); security set to the `ApiKey` (`key`) scheme.