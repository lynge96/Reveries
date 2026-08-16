# Google Books integration

Operational notes for the Google Books API. The `GoogleBooksOpenAPI.json` spec in this folder is
the *contract* (endpoints and response shapes); this file is the *runtime* knowledge the spec
does not carry — auth, quotas, query qualifiers.

- **Official docs:** https://developers.google.com/books/docs/v1/using
- **OpenAPI spec:** [`GoogleBooksOpenAPI.json`](./GoogleBooksOpenAPI.json) — trimmed to the
  `volumes` and `series` surface only.

## Configuration

Bound from the `GoogleBooks` config section (`GoogleBooksServiceCollectionExtensions.AddGoogleBooks`),
validated on start (`ApiUrl`, `ApiKey` required).

| Key | appsettings | Env override | Value |
|---|---|---|---|
| Base URL | `GoogleBooks:ApiUrl` | `GoogleBooks__ApiUrl` | `https://www.googleapis.com/books/v1/` |
| API key | `GoogleBooks:ApiKey` | `GoogleBooks__ApiKey` | Never committed; user-secrets (dev) / env (prod) |

The dependency's display name in logs is `GoogleBooks API` (`GoogleBooksSettings.SectionName`),
distinct from the `GoogleBooks` config-section name.

## Authentication

Public volume reads need only an API key, passed as the `key` **query parameter** (built inline in
`GoogleBooksClient`) — no OAuth flow:

```
GET volumes?q=isbn:9788700000000&key=<ApiKey>
```

## Quota & rate limits

- Default quota is **~1,000 requests/day per project**, plus a per-user rate limit; both are
  managed in the Google Cloud Console. Verify the current quota against your project.
- Quota-exceeded errors can surface as **HTTP 403** (`rateLimitExceeded`), not only `429`. Note
  that `ExternalBaseClient.HandleResponseAsync` treats `403` as an invalid/expired key and returns
  `null` — a genuine quota breach is therefore logged as a key problem. Keep this in mind when
  diagnosing empty responses. HTTP timeout is 15 seconds.

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