# Model map — the "book" shapes and where each is used

A "book" is never one object in this codebase. On the **write** side it is two domain
aggregates — a `Work` (the abstract book) and an `Edition` (one physical release) — and on the
**read** side it is a single flat `Book` composed from them. Each layer has its own
representation on purpose (Clean Architecture + CQRS); this page says which is which so it is
clear what to touch where.

## The models

| Model | Layer | Direction | Role |
|---|---|---|---|
| `Work`, `Edition` (+ `Author`, `Publisher`, `Genre`, `DeweyDecimal`, value objects) | Domain | **write** | The aggregates that enforce invariants. `Work` holds authors by `AuthorId`; `Edition` references its `Work` by `WorkId` — aggregates reference each other by id, never by holding the whole object graph. |
| `BookCandidate` (+ `BookCandidateData`) | Application | **ingest** | Flat book metadata gathered from the external sources (ISBNDB, Google, Saxo), merged and enriched, then handed to persistence. Not a query result and not an API type. |
| `CachedBook` (+ `CachedBookMapper`) | Application | ingest (cache) | A flat, serializable snapshot of a `BookCandidate` for the `CachingBookSearch` decorator, because `BookCandidate` carries value objects with private constructors that don't serialize cleanly. |
| `Book` | Application | **read** | The single read/display model: a `Work` + `Edition` denormalized flat, returned by every query. A lookup preview (found externally, not yet saved) uses an empty `BookId`. |
| `BookRow`, `WorkAggregateRow` | Persistence | read | Dapper row shapes the read SQL projects into, mapped to `Book` / `Work`. |
| `EditionRecord`, `WorkRecord`, … | Persistence | write | Row shapes for the INSERTs. |
| `BookDetailsDto`, `CreateBookRequest`, `BooksResponse` | API (`Reveries.Api/Contracts`) | edge | The wire contract. Domain/Application types never cross this boundary; the API mappers convert to/from these. |

## The two flows

**Write — scan an ISBN and save it**
```
external sources → BookCandidate (merge + enrich, incl. Saxo link)
    → WorkPersistenceService: resolve authors/publisher, get-or-create Work, build Edition
    → EditionRecord / WorkRecord → INSERT
```

**Read — look up or display a book**
```
GET /books/isbn/{isbn}  → external lookup → BookCandidate → Book        (preview, empty BookId)
GET /books, /books/{id} → BookQueryRepository → BookRow → Book          (from the catalogue)
    → both map through the one BookDetailsMapper → BookDetailsDto
```

## Why not one shared `Book` model for everything

- **Write model ≠ read model (CQRS).** The aggregates enforce invariants (`Title.Create` throws,
  ISBN checksums, …); `Book` is a denormalized projection tuned for display. Merging them would
  couple the query shape to the domain invariants.
- **The `Work`/`Edition` split is the point.** De-duplication and "many editions share one work"
  exist only because they are separate aggregates; a single `Book` write model erases that.
- **Domain ≠ DTO.** `Reveries.Architecture.Tests` forbids the API contracts from depending on
  Domain; a shared class across the API boundary would reintroduce that coupling.

So `Book` is the read-side "single book model" — the composition that makes the flow readable —
while writes stay on the `Work`/`Edition` aggregates.