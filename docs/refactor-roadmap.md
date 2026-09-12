# Refactor & Hardening Roadmap

Consolidation pass before new features. The goal is to get the current setup
onto a solid, best-practice footing — tests, domain model, queries, structure —
so that later feature work is cheap and safe rather than risky.

**Appetite: moderate modernisation.** Keep Clean Architecture + Dapper. Introduce
migration tooling and tidy structure/tests, but do *not* swap the ORM, re-slice
the projects wholesale, or adopt event sourcing. Those stay explicitly out of
scope (see [Out of scope](#out-of-scope)).

## Guiding principles

1. **Safety net before surgery.** Nothing structural changes until there are
   tests that pin the current behaviour. Today `Reveries.Application.Tests`
   contains only the default `UnitTest1.cs` — there is no integration or SQL
   coverage, so any change to the domain or queries is currently unguarded.
2. **Work inside-out.** Dependencies point inward, so changes to the core ripple
   outward but not vice versa. Stabilise the domain first; map the edges to it
   last. This is the topological order of the project dependency graph, and it
   minimises rework by construction.
3. **Small, green, shippable PRs.** One improvement at a time, each keeping CI
   green and `main` deployable (the app is self-hosted on a Raspberry Pi behind
   a Cloudflare tunnel — `main` must stay releasable). No big-bang branch.
4. **Characterisation over correctness (at first).** The initial tests freeze
   *current* behaviour — bugs included — so a refactor's effect is visible. Fix
   the bugs afterwards as deliberate, separate commits.

## Prioritised pain points

From the current review, the three pains driving the order are **missing tests**,
**the domain model feeling wrong**, and **query/performance concerns**. These map
cleanly onto the inside-out order: tests are the prerequisite for safely touching
either of the other two, and query work needs a real database to verify against.

---

## Completed — solution restructuring

The outer layer was consolidated from **6 projects to 3** so test projects mirror
the production layers cleanly:

- `Reveries.Integration.Http`/`.GoogleBooks`/`.Isbndb` → **`Reveries.Integration`**
  (folders `Http/`, `GoogleBooks/`, `Isbndb/`). A later tidy pass made each provider
  an identical slice (`Clients/Configuration/Dtos/Interfaces/Mappers/Services`):
  `DTOs/` → `Dtos/`, ISBNDB's `Dtos/Books/` flattened, JSON contexts moved beside
  their DTOs, the Google mapper renamed to the API name (`GoogleBooksMapper`), and the
  two `IBookSearch` implementations renamed for their role (`GoogleBooksSource`, `IsbndbSource`).
- `Reveries.Infrastructure`/`.Postgresql`/`.Redis` → **`Reveries.Infrastructure`**
  (composition + Serilog + Redis) with the Dapper/Npgsql adapter split back out as
  **`Reveries.Persistence`** (namespaces `Reveries.Persistence.*`; DB session +
  `ITransactionManager` under `Context/`).

Test projects mirror the outer layers (`Reveries.Persistence.Tests`,
`Reveries.Integration.Tests`, `Reveries.Api.Tests`, plus `Reveries.Domain.Tests`
and `Reveries.Application.Tests`). `Reveries.Console` (a manual scratch harness) was
**deleted**; the scanner is now the only frontend. **`Reveries.Architecture.Tests`**
(NetArchTest) enforces the layer rules on the compiled namespaces: Domain depends on
nothing outer, Application only on Domain, Contracts exposes no domain type, and the
concrete `Reveries.Persistence.Repositories` types stay inside Persistence.

---

## Phase 0 — Safety net (the keystone)

The one piece of work that de-risks both the domain and the query work at once.

- [x] **`Reveries.Persistence.Tests`** with **Testcontainers** (real Postgres):
      a shared `PostgresContainerFixture` (one container per collection, schema via
      `ApplySchemaAsync`, `TRUNCATE` between tests) plus repository/SQL tests covering
      `BookRepository` view hydration, a transactional write round-trip, ISBN
      cross-matching, `GetAllBooksAsync`, and the `PostgresDbContext` transaction
      lifecycle. Already caught a real bug (a publisher/series-less book hydrated as a
      fabricated null-name object instead of `null`).
- [ ] In **`Reveries.Application.Tests`** (use-case tests, infrastructure
      stubbed), write characterisation tests for the critical path: scan ISBN →
      enrich (with stubbed ISBNDB / Google Books HTTP responses) → persist → read
      back. Delete the placeholder `UnitTest1.cs`.
- [x] Integration tests wired into CI — `build-test` runs an unfiltered `dotnet test`
      and `ubuntu-latest` provides a Docker daemon, so Testcontainers runs on every PR.

**Why integration and not unit?** The Dapper SQL is hand-written; it can only be
verified against a real database. Mocking the repositories would test the C#, not
the SQL — and the SQL is exactly what Phase 2 rewrites.

**Done when:** a green CI run proves the scan→persist path and the main read
queries behave as they do today, and the build fails if that behaviour changes.

---

## Phase 1 — Domain model (inside-out)

With the net in place, stabilise the core. Concrete targets spotted in the
current model:

- [x] **Long-parameter-list factories → parameter objects.** `Work.Create`/`Reconstitute`
      both take a **`WorkData`**/`WorkReconstitutionData` record now, mirroring the
      existing `EditionData`/`EditionReconstitutionData` pairing.
- [x] **Weakly-typed fields → value objects / enums:**
      - [x] `PublicationDate` — partial-date value object (year + optional month/day, derived
        `DatePrecision`), parsing/serializing the canonical string in the unchanged column.
      - [x] `Binding` → `BookFormat` enum (via `BookFormatNormalizer`); property/column/contract
        renamed `Binding` → `Format` (the enum spans non-binding media); ISBNDB DTO keeps `Binding`.
      - [x] `Language` — value object owning the ISO-639-1 code, validated against `CultureInfo`
        neutral cultures, display name derived at the edge.
      - [x] image fields → `Cover` value object (`Url` + optional `ThumbnailUrl`, http(s)-only).
        Self-hosting ingestion (download → Cloudflare R2 → own URL) is designed but deferred; the
        `ICoverImageStore`/`StoredCover` seam is in place, the Infrastructure impl is a later step.
      - [x] `Msrp` **removed** (a currency-less US list price of little value); a future
        "collection value" feature can reintroduce it as a proper `Money` value object. Column dropped.
      - [x] `Pages` stays `int?` but its check moved to a unit-tested `PageCountNormalizer`
        (also drops implausible `> 50000` counts).
      - [x] `BookDimensions` gained a non-re-sanitizing `Reconstitute`; the DB→domain path uses it,
        the two API-merge sites keep `Create`.
      - [x] `DataSource` enum **removed** — read provenance is not an aggregate property; the Blazor
        "already saved?" check now uses `BooksApi.ExistsAsync(isbn)`. Column dropped.
- [x] **Reviewed the public setters.** The `SetSeries` "number without series" concern is
      structurally unrepresentable via the `SeriesPlacement` value object (the number lives
      inside a placement requiring a non-null `Series`); all write paths uphold it, pinned by a
      test. A DB-level `CHECK` for defence-in-depth is deferred.
- [x] **Audited `Create` vs `Reconstitute` across every aggregate/value object.** Most paths
      were already clean; three leaks fixed by adding a non-validating `Reconstitute` and switching
      the call site: **`Genre`**, **`DeweyDecimal`** (the mapper re-ran `TryCreate` on stored data)
      and **`PublicationDate`** (re-parsed the stored string). One latent `Isbn.Create` fallback in
      `Edition.Reconstitute` is left as unreachable.
- [ ] **Author identity — external stable code (guards same-name authors).**
      Two different people with the same name currently collapse into one row
      (`normalized_name` is the identity via `UNIQUE(normalized_name)`), mixing
      their books. The current model is deliberately name-only (`Name` +
      derived `NormalizedName`); the disambiguation upgrade is deferred until an
      author-profile feature actually needs it. When it does: add an optional
      `AuthorCode` on `Author` (a stable external id, e.g. a Wikidata QID
      `Q42`) stored resolvable, and split the single `UNIQUE(normalized_name)`
      into two partial indexes — `UNIQUE(code) WHERE code IS NOT NULL` and
      `UNIQUE(normalized_name) WHERE code IS NULL` — so dedup resolves by the
      code when present (`Code ?? normalized_name`). The removed
      `AuthorNameVariant` subsystem was an earlier, name-based attempt and is not
      coming back.

      *Wikidata enrichment notes (for when the integration is built):*
      - **Why Wikidata over OpenLibrary:** OpenLibrary bundles an ISBN → author
        chain in one place but is slow (`/api/books?jscmd=data` ~10s observed)
        and often only has sparse import stubs (a book scanned during this design
        returned an author literally named "322508 MJ"). Wikidata gives richer,
        more structured author data (birth/death, image, VIAF/ISNI, description)
        for free — but has no ISBN → author chain, so you must match by name and
        disambiguate.
      - **Endpoints:** `wbsearchentities` to resolve a name to a QID, then
        `https://www.wikidata.org/wiki/Special:EntityData/Q{id}.json` (CDN-cached)
        for the entity. Useful claims: `P569` birth, `P570` death, `P18` image
        (→ Commons file), `P214` VIAF, `P213` ISNI, plus the label/description.
      - **Matching is the hard part:** name lookup is ambiguous (many "John
        Smith"), so pair it with any signal you have (co-occurring title, birth
        era) or keep it manual/confirm-on-conflict. The QID becomes the
        `AuthorCode` and the stable dedup key thereafter.
      - **Latency / placement:** the lookup must never sit on the scan critical
        path. Do enrichment async or lazy, cache author entities in Redis with a
        long TTL (they are highly stable and shared across many books), and use a
        tight timeout with name-based fallback (`Code ?? normalized_name`).
- [ ] **Work identity & de-duplication — external stable code (groups editions,
      separates same-title works).** Two problems, one cause. First, there is
      currently **no `Work` de-duplication at all**: `WorkPersistenceService`
      always calls `InsertWorkAsync`, so every scanned edition creates a fresh
      `Work` even when it is another edition of one you already own — the
      Work/Edition split is structurally present but works are never actually
      shared. Second, different works share a title (Homer's *Odyssey* vs Stephen
      Fry's), so title cannot be the key.

      **Identity vs matching are separate concerns and must stay separate.**
      `Work` identity stays the opaque surrogate `WorkId` (GUID) — it is already
      correct and collision-free (two *Odyssey* rows just have different GUIDs). Do
      **not** turn the title into the key, and do **not** invent a derived slug like
      `odyssey:homer`: a slug bakes a lossy, mutable heuristic into the identity,
      breaks on title variants ("The Odyssey"), translations/transliteration
      (`Ὅμηρος`), and — worst — on same-name authors (`odyssey:johnsmith` cannot
      tell two John Smiths apart, the very ambiguity the author item above
      describes). De-duplication is a *matching* concern layered on top of the
      stable identity, not the identity itself.

      **The fix: an optional `WorkCode` resolved from OpenLibrary, used as the
      dedup key** — the work-level twin of the author `AuthorCode`. Dedup resolves
      by `WorkCode ?? (normalized title + primary-author signature)` with
      confirm-on-conflict when the code is absent, and the schema splits the key
      into two partial unique indexes exactly like the author plan:
      `UNIQUE(work_code) WHERE work_code IS NOT NULL` and a title+author fallback
      guard `WHERE work_code IS NULL`.

      **Why OpenLibrary for works (and Wikidata for authors):** OpenLibrary's data
      model *is* the ISBN → Edition → Work chain, so identity is keyed off the
      scanned ISBN deterministically rather than guessed from a name — the opposite
      of Wikidata, which has no reliable ISBN → work chain and thin book coverage.
      OpenLibrary's stub-quality problem matters less here because dedup needs only
      the stable `OL…W` key, not the record's contents.
      - **Endpoints (use the light record fetches, not the heavy `jscmd=data`
        Books API which is ~10s):** `https://openlibrary.org/isbn/{isbn}.json`
        returns the edition with `"works":[{"key":"/works/OL…W"}]` — store the
        `OL…W` as `WorkCode`; `https://openlibrary.org/works/OL…W.json` for the work
        record; `https://openlibrary.org/search.json?title=&author=&fields=…` as the
        title+author fallback / disambiguation path (each `doc` is a work with
        `author_name`).
      - **Bridge to Wikidata for authors:** OpenLibrary author records
        (`/authors/OL…A.json`) often carry `remote_ids.wikidata` (and VIAF/ISNI), so
        one ISBN-keyed OpenLibrary call yields both the work id *and* the authors'
        OLIDs, and the author `remote_ids` bridge onward to the Wikidata QID for the
        rich author enrichment above. OpenLibrary is the entry point (ISBN → work +
        authors); Wikidata is the enrichment layer on top.
      - **Placement:** same rules as author enrichment — off the scan critical path
        (async/lazy), a descriptive `User-Agent`, a tight timeout, and Redis caching
        with a long TTL (works are highly stable and shared across editions).

      **Build it as one vertical slice, not the field alone.** `WorkCode` has no
      value on its own — it is inert until *both* the OpenLibrary resolver populates
      it *and* the save-path dedup consumes it. Adding just the column now would be
      an always-null field of exactly the kind removed with `Msrp` and `DataSource`;
      and `WorkData`/`WorkReconstitutionData` already make adding the field later a
      small diff, so there is nothing to gain by front-running it. The three parts
      that only make sense together: (1) an `IWorkAuthoritySearch` OpenLibrary
      integration (its own `Reveries.Integration` folder, registered like ISBNDB /
      Google), (2) the `WorkCode` domain field + `works.work_code` column + partial
      indexes, and (3) get-or-create-by-code in `WorkPersistenceService` replacing
      the unconditional `InsertWorkAsync`. Share the OpenLibrary integration with
      the author-authority item and build the two together.
- [ ] **Introduce migration tooling here** — the domain changes above are the
      first schema changes, so this is the natural point. Adopt **DbUp**
      (lightweight, plain-SQL, no EF) so schema changes become one versioned
      migration file instead of the current three hand-edits (domain type,
      Dapper SQL, `infra/db_schema.sql`). Backfill an initial baseline migration
      from the existing `db_schema.sql`.

- [x] **`Work` gained a `Description` value object** alongside `Synopsis`. Google returns two
      descriptions — a short synopsis (search result) and fuller text (full volume) — so
      `Synopsis` holds the teaser and `Description` the full text (both HTML-stripped via
      `HtmlToPlainText`); `GoogleBooksSource` keeps both and `EditionWithWorkMerger` merges them.
      Both are work-level (identical across editions). A `description` column was added.

**Done when:** the domain model expresses its invariants through types and
methods, `Create`/`Reconstitute` are symmetric, and schema changes ship as
versioned migrations. Domain and integration tests stay green.

---

## Phase 2 — Queries & persistence

Now the domain is stable, optimise the outer edge with the Phase 0 tests as a net.

- [ ] **Profile the aggregate hydration.** There are nine repositories, including
      per-join-table ones (`BookAuthorsRepository`, `BookGenresRepository`,
      `BookDeweyDecimalsRepository`). Confirm whether loading one `Book` fans out
      into many round-trips (N+1) and collapse them into set-based joins where
      it helps.
- [x] **Collapsed the write-path N+1.** The `GetOrCreate` repositories and join-table inserts
      now use a single `unnest`-based bulk upsert/insert per call (constant statements regardless
      of N; in-batch duplicates via `SELECT DISTINCT`). Pinned by `GetOrCreateBatchTests`.
- [x] **Multi-table writes run in a transaction.** `IUnitOfWork` replaced by a focused
      `ITransactionManager`; a `CreateCommand` seam on `IDbContext` attaches the active
      transaction to every Dapper command, with re-entrancy guarded. Proven by the write
      round-trip and lifecycle tests.
- [ ] Review indexes against the real query patterns (ISBN lookups, title
      search, author joins). Add missing indexes as migrations (Phase 1 tooling).
- [ ] Re-check the Redis cache-aside paths (`IBookCacheService`) for correctness
      after any query shape changes.

**Done when:** the hot read/write paths have no obvious N+1, transactions are
verified, and indexes match the query patterns — all proven by the integration
tests running against real Postgres.

---

## Phase 3 — API & edges (light, last)

Least risky, so last.

**Note — direction for the API refactor:** when the API layer is refactored, move
from MVC controllers to **Minimal APIs** (endpoint routing, typed results). The
controller-specific items below are then reframed as the equivalent Minimal-API
concerns (thin endpoint delegates dispatching to Mediator, `TypedResults`,
`.WithOpenApi()` metadata, route groups for versioning).

- [ ] Review `BooksController` stays thin (translate Contracts → Mediator, no
      logic) and confirm no domain types leak across the API boundary.
- [ ] Tidy the OpenAPI/Swagger spec; consider response-type annotations and
      consistent error contracts via `ExceptionHandlingMiddleware`.
- [ ] Decide on an API versioning approach before the first breaking contract
      change lands with new features.

**Done when:** the API surface is documented, consistent, and versioned ready for
feature work.

---

## Out of scope

Explicitly *not* part of this pass (moderate appetite):

- Swapping Dapper for an ORM (EF Core etc.).
- Re-slicing the project/solution structure wholesale.
- Event sourcing, CQRS read-model projections, or messaging.
- The eventual social layer (shelves, reviews) — that is feature work for after
  this consolidation.

## Working agreement

- One phase's PRs merge before the next phase starts; phases are ordered by
  dependency, not preference.
- Every PR keeps CI green and `main` deployable.
- Bugs found during characterisation are fixed as separate, labelled commits so
  the change is visible in history.