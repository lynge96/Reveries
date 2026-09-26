# Refactor & Hardening Roadmap

Consolidation pass to get the codebase onto a solid, best-practice footing — tests,
domain model, queries, structure — so later feature work is cheap and safe.

**Appetite: moderate.** Keep Clean Architecture + Dapper. Tidy structure/tests and
harden the model, but do *not* swap the ORM, re-slice the projects wholesale, or adopt
event sourcing (see [Out of scope](#out-of-scope)).

> **Status.** The consolidation pass is effectively complete; the remaining open items
> below are deferred by design, not blocked. Feature work has begun — the first backlog
> item (the Saxo product deep link) has shipped. Feature ideas live in
> [`feature-backlog.md`](feature-backlog.md).

> **Note — the `Series` feature was removed** (aggregate, repository, `SetBookSeries`
> command/endpoint, the `works.series_*` columns and `catalog.series` table), dropped by
> migration `0002_DropSeries.sql`. It was unused and can return cleanly when needed. Older
> entries mentioning series are kept as a record and no longer reflect the model.

## Guiding principles

1. **Safety net before surgery** — pin current behaviour with tests before touching it.
2. **Work inside-out** — stabilise the domain first, map the edges last (the dependency
   graph's topological order).
3. **Small, green, shippable PRs** — `main` stays deployable (self-hosted on a Raspberry
   Pi behind a Cloudflare tunnel).
4. **Characterisation first** — freeze current behaviour (bugs included), then fix bugs as
   separate, labelled commits.

---

## Completed

**Solution restructuring.** Outer layer consolidated from 6 projects to 3 so test
projects mirror the production layers: `Reveries.Integration` (provider slices `Http/`,
`GoogleBooks/`, `Isbndb/`, each `Clients/Configuration/Dtos/Interfaces/Mappers/Services`),
`Reveries.Infrastructure` (composition + Serilog), and `Reveries.Persistence` (Dapper/Npgsql
+ `ITransactionManager`). `Reveries.Console` deleted. **`Reveries.Architecture.Tests`**
(NetArchTest) enforces the layer rules on the compiled namespaces.

**Application tidy.** CQRS semantics corrected (writes are `ICommand<T>`, reads `IQuery<T>`);
exception base renamed `ApplicationException` → `AppException` (BCL collision); DI extension
renamed to the `XxxServiceCollectionExtensions` convention.

**Phase 0 — Safety net.** `Reveries.Persistence.Tests` with **Testcontainers** (real
Postgres: shared fixture, `TRUNCATE` between tests) covering hydration, transactional write
round-trip, ISBN cross-matching. `BookLifecycleCharacterizationTests` drives the real
Application graph through `IMediator` (only the edges stubbed) for the scan → enrich →
persist → read path. Both run in CI on every PR (Docker on `ubuntu-latest`). *Why
integration, not unit: the hand-written Dapper SQL can only be verified against a real DB.*

**Phase 1 — Domain model.** Factories take parameter objects (`WorkData`/`EditionData` +
their `Reconstitution` twins). Weakly-typed fields became value objects / enums:
`PublicationDate` (partial date), `Binding` → `BookFormat` enum, `Language` (ISO-639-1),
`Cover`, `Description` alongside `Synopsis`; `Pages` check moved to `PageCountNormalizer`;
`Msrp` and the `DataSource` enum removed with their columns. `Create` (validating) vs
`Reconstitute` (non-validating) audited and made symmetric across every aggregate/value
object. Value-format validation lives in the domain, not as DB `CHECK`s.

**Phase 2 — Queries & persistence.**
- Read hydration has **no N+1** — one set-based query per book; each one-to-many relation
  (authors, genres, Dewey codes) aggregated into a name/code array in a `LEFT JOIN LATERAL` with
  `array_agg`, avoiding both per-relation round-trips and cartesian row-multiplication. The row
  mirrors the schema (no rename aliases); `BookRow → Book` mapping lives in `BookMappingExtensions`.
- Write-path N+1 collapsed — `GetOrCreate` and join-table inserts use a single `unnest`-based
  bulk upsert/insert per call (`GetOrCreateBatchTests`).
- Multi-table writes run in a transaction via `ITransactionManager` (a `CreateCommand` seam on
  `IDbContext` attaches the active transaction to every Dapper command).
- Caching sits in Application as a `CachingBookSearch` decorator (Scrutor `.Decorate`) over
  each `IBookSearch`, caching a flat `CachedBook` per source+ISBN. In-memory `HybridCache`
  (chosen over `IMemoryCache` for stampede protection); a Redis L2 can be added by registering
  an `IDistributedCache` with no call-site changes.

**Phase 3 — API & edges.** Rebuilt on **Minimal APIs** (`MapGroup("books")`, thin `static`
delegates → Mediator → `TypedResults`; `POST /books` now returns `201`). Errors via a chain of
`IExceptionHandler` + `AddProblemDetails()` (RFC 9457). Swashbuckle → built-in
`Microsoft.AspNetCore.OpenApi` + Scalar; **committed `openapi.json`** regenerated on demand
(`GenerateOpenApi` target) and CI-guarded against staleness. Request validation via built-in
.NET 10 validation (DataAnnotations → `400 ValidationProblemDetails`); the error contract is
declared in the spec. `Reveries.Contracts` folded into `Reveries.Api/Contracts`. Scanner
decoupled — generates its own models from `openapi.json` with **NSwag** (the seam for the
planned JS/TS frontend). `Reveries.Api.Tests` cover every status path via
`WebApplicationFactory<Program>`.

**Migration tooling.** **DbUp** adopted (lightweight, plain-SQL, no EF): schema changes ship as
one versioned migration under `Migrations/Scripts`, run at startup and journalled in
`public.schema_versions`.

---

## Open (deferred by design)

- [ ] **Index review.** The schema is well-indexed (identity/equality lookups all hit a PK or
      UNIQUE index; join-table filters ride the composite PK's leading column). The only
      candidate is a functional index `lower(btrim(title))` matching the dedup predicate — but
      the author-side index already gives that query a good plan, so it is marginal. `idx_works_title`
      (raw title) is currently unused. Optional cleanup: replace it with the functional index.
- [ ] **Author & work identity — external stable codes.** *Local* title+author de-duplication is
      done: `WorkPersistenceService` get-or-creates a `Work` via `FindWorkIdByTitleAndAuthorsAsync`
      (normalized title **and** a shared author), reusing the `WorkId`; it errs toward a separate
      work over a false merge. Identity stays the opaque `WorkId` (GUID) — de-duplication is a
      *matching* layer on top, never a derived slug. Deferred until an author-profile / dedup
      feature needs it: an optional `WorkCode` (OpenLibrary `OL…W`, keyed off the ISBN) and
      `AuthorCode` (Wikidata QID), each with split partial unique indexes
      (`UNIQUE(code) WHERE code IS NOT NULL` + a name/title fallback `WHERE code IS NULL`), resolved
      off the scan critical path (async/lazy, tight timeout, cached) with a name-based fallback.
      Build `WorkCode` as one vertical slice (OpenLibrary integration + column + save-path
      consumer), not the always-null field alone.
- [ ] **Robustness middleware** (as the API goes public over the tunnel): rate limiting, output
      caching, request timeouts — all built-in, no packages.
- [ ] **Authentication/authorization** — none today; deferred to the social-layer feature, but
      required before any non-personal exposure.
- [ ] **OpenTelemetry** — worth it once there are more services to correlate across; not needed
      for a single API now.

**Descoped:** pagination on `GetAllBooks` — a personal single-user catalogue is small enough to
return whole (YAGNI). *Loose end:* it still `404`s on an empty catalogue where an empty `200` is
correct — a small independent fix if ever wanted.

---

## Out of scope

- Swapping Dapper for an ORM (EF Core etc.).
- Re-slicing the project/solution structure wholesale.
- Event sourcing, CQRS read-model projections, or messaging.
- The social layer (shelves, reviews) — feature work for after this pass.

## Working agreement

- Phases are ordered by dependency, not preference.
- Every PR keeps CI green and `main` deployable.
- Bugs found during characterisation are fixed as separate, labelled commits.