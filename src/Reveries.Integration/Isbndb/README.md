# ISBNdb integration

Operational notes for the ISBNdb book-metadata API. The `IsbnDbOpenAPI.json` spec in this
folder is the *contract* (endpoints and response shapes); this file is the *runtime*
knowledge the spec does not carry — auth, hosts, rate limits, quotas.

- **Official docs:** https://isbndb.com/apidocs/v2
- **OpenAPI spec:** [`IsbnDbOpenAPI.json`](./IsbnDbOpenAPI.json) — trimmed (deprecated endpoints
  and fields removed).

## Configuration

Bound from the `Isbndb` config section (`ServiceCollectionExtensions.AddIsbndb`), validated on
start (`ApiUrl`, `ApiKey` required; `MaxBulkIsbns` must be positive).

| Key | appsettings | Env override | Notes |
|---|---|---|---|
| Base URL | `Isbndb:ApiUrl` | `Isbndb__ApiUrl` | Host depends on plan — see below |
| API key | `Isbndb:ApiKey` | `Isbndb__ApiKey` | Never committed; user-secrets (dev) / env (prod) |
| Bulk cap | `Isbndb:MaxBulkIsbns` | `Isbndb__MaxBulkIsbns` | Default 100 |

The dependency's display name in logs is `ISBNdb API` (`IsbndbSettings.SectionName`), which is
distinct from the `Isbndb` config-section name.

## Authentication

The API key is sent as a raw `Authorization` header value — **no `Bearer` prefix**
(`IsbndbClientExtensions.ConfigureIsbndb`):

```
Authorization: <ApiKey>
```

## Hosts & rate limits

The base URL **and** the request-per-second limit depend on the subscription plan. Verify the
current values against your own plan before changing `ApiUrl`.

| Plan | Host (`ApiUrl`) | Rate limit |
|---|---|---|
| Basic | `https://api2.isbndb.com/` | ~1 request/sec |
| Premium | `https://api.premium.isbndb.com/` | ~3 requests/sec |
| Pro | `https://api.pro.isbndb.com/` | ~5 requests/sec |

The project is currently configured for the Basic host. A `429 Too Many Requests` is handled in
`ExternalBaseClient.HandleResponseAsync` by logging and returning `null` (no retry/back-off is
implemented — add one if a higher-throughput plan is adopted). HTTP timeout is 15 seconds.

## Endpoints in use

| Client | Method | Path | Purpose |
|---|---|---|---|
| `IsbndbBookClient` | GET | `book/{isbn}` | Single book by ISBN |
| `IsbndbBookClient` | GET | `books/{query}` | Search (params: `language`, `shouldMatchAll=1`) |
| `IsbndbBookClient` | POST | `books` | Bulk ISBN lookup (body `{ isbns: [...] }`, max `MaxBulkIsbns`) |
| `IsbndbAuthorClient` | GET | `author/{name}`, `authors/{query}` | Author details / search |
| `IsbndbPublisherClient` | GET | `publisher/{name}`, `publishers/{query}` | Publisher details / search |

## Spec provenance

- **Source:** ISBNdb OpenAPI (Swagger) export from https://isbndb.com/apidocs/v2 — confirm.
- **Retrieved:** 2026-08 — confirm.
- **Local edits:** deprecated operations (`/search/authors`, `/search/publishers`,
  `/search/subjects`) and deprecated `Book`/`ErrorResponse` fields removed; affected `required`
  arrays updated.