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

Before Phase 0, the outer layer was consolidated from **6 projects to 3** so that
test projects can mirror the production layers cleanly:

- `Reveries.Integration.Http` + `.GoogleBooks` + `.Isbndb` → **`Reveries.Integration`**
  (folders `Http/`, `GoogleBooks/`, `Isbndb/`; namespaces unchanged).
- `Reveries.Infrastructure` + `.Postgresql` + `.Redis` → first merged into one
  `Reveries.Infrastructure`, then persistence was split back out (it has the
  strongest case: heaviest isolated dependencies, largest, and the main
  integration-test target). Final split:
  - **`Reveries.Infrastructure`** — composition + Serilog logging + Redis caching.
  - **`Reveries.Persistence`** — the Dapper/Npgsql database adapter (namespaces
    `Reveries.Persistence.*`; DB session and `ITransactionManager` under `Context/`).

The outer layer is mirrored by test projects — `Reveries.Persistence.Tests`,
`Reveries.Integration.Tests`, and `Reveries.Api.Tests` — alongside the existing
`Reveries.Domain.Tests` and `Reveries.Application.Tests`.

`Reveries.Console` — the CLI entry point — was **deleted**. It was a manual
scratch/testing harness, and its last domain coupling (the `DataSource`-based
filtering and ordering) had already been removed earlier in the refactor. The
scanner (`Reveries.Blazor.BookScanner`) is now the only frontend.

Layer boundaries that assembly separation no longer enforces are recovered by a
**`Reveries.Architecture.Tests`** project using **NetArchTest**, asserting the
layer rules on the compiled namespaces. Four rules are in place:

1. Domain depends on no outer layer.
2. Application depends only on Domain (not Contracts, Infrastructure, Persistence,
   Integration, or Api).
3. Contracts has no dependency on Domain (no domain type crosses the API boundary).
4. The concrete `Reveries.Persistence.Repositories` types do not leak out of
   Persistence — outer layers reach them only through the `IRepository`
   interfaces and the `AddPostgres` DI extension.

---

## Phase 0 — Safety net (the keystone)

The one piece of work that de-risks both the domain and the query work at once.

- [x] Add a new **`Reveries.Persistence.Tests`** project (mirroring the
      Persistence layer) with **Testcontainers** (real Postgres in a throwaway
      container). Repository/SQL tests belong here, not in `Application.Tests`.
      A shared `PostgresContainerFixture` starts one container per collection,
      applies `db_schema.sql` via a single `ApplySchemaAsync` seam, and resets
      between tests with `TRUNCATE`.
- [x] Add **repository-level tests** that exercise the hand-written Dapper SQL
      against real Postgres — these are the safety net for Phase 2. Covered:
      `BookRepository` full view hydration (book + authors + genres + dewey +
      series), a write round-trip through a real transaction, the ISBN lookup
      cross-matching, `GetAllBooksAsync` (empty + multi-row without relation
      leakage), and the `PostgresDbContext` transaction lifecycle (re-entrancy
      guard and post-commit reuse). These already caught and fixed a real bug: a
      book with no publisher/series was hydrated as a fabricated object with a
      null name instead of `null`.
- [ ] In **`Reveries.Application.Tests`** (use-case tests, infrastructure
      stubbed), write characterisation tests for the critical path: scan ISBN →
      enrich (with stubbed ISBNDB / Google Books HTTP responses) → persist → read
      back. Delete the placeholder `UnitTest1.cs`.
- [x] Wire the new integration tests into CI (`pr.yml`) — already covered: the
      `build-test` action runs an unfiltered `dotnet test` over the whole
      solution, and `ubuntu-latest` provides a running Docker daemon, so
      Testcontainers runs on every PR with no extra configuration.

**Why integration and not unit?** The Dapper SQL is hand-written; it can only be
verified against a real database. Mocking the repositories would test the C#, not
the SQL — and the SQL is exactly what Phase 2 rewrites.

**Done when:** a green CI run proves the scan→persist path and the main read
queries behave as they do today, and the build fails if that behaviour changes.

---

## Phase 1 — Domain model (inside-out)

With the net in place, stabilise the core. Concrete targets spotted in the
current model:

- [x] **Long-parameter-list factories → parameter objects.** After the aggregate
      split, the old `Book.Create` became `Work.Create` (7 params) and
      `Edition.Create`. `Edition.Create` already took an `EditionData` record;
      `Work.Create` was the remaining positional smell and was asymmetric with
      `Work.Reconstitute` (which already took `WorkReconstitutionData`). Introduced
      a **`WorkData`** input record so both `Work` factories take a parameter object,
      mirroring the `EditionData`/`EditionReconstitutionData` pairing.
- [x] **Weakly-typed fields → value objects / enums:**
      - [x] `PublicationDate` is now a partial-date value object (`Editions/PublicationDate.cs`)
        holding year + optional month/day with a derived `DatePrecision`; it parses
        `YYYY` / `YYYY-MM` / `YYYY-MM-DD` best-effort and serializes back to the canonical
        string stored in the unchanged `publication_date` varchar column.
      - [x] `Binding` is now the `BookFormat` enum, normalized from raw strings via
        `Helpers/BookFormatNormalizer.GetStandardFormat()`. The property, record params,
        DB column and API contract were renamed `Binding` → `Format` because the enum
        spans media (`Ebook`, `Audiobook`) that are not bindings; the external ISBNDB DTO
        keeps its source name `Binding`.
      - [x] `Language` is now a value object (`Editions/Language.cs`) owning the canonical
        ISO-639-1 code and deriving the display name at the edge; validity is checked against
        `CultureInfo.GetCultures(NeutralCultures)` rather than the lossy `GetLanguageName()`.
      - [x] The two loose image `string?` fields are now a `Cover` value object
        (`Editions/Cover.cs`) holding `Url` (the full cover image) + optional `ThumbnailUrl`.
        `TryCreate` normalizes both (trim, empty→null) and drops anything that is not an
        absolute http(s) URL; if only one usable URL survives it becomes `Url`. The property
        was named `Url` (not `OriginalUrl`) so the domain does not inherit ISBNDB's
        `image_original` source vocabulary. `Edition.SetCover` mirrors `SetPublisher`, so the
        deferred ingestion step can swap the external URL for a stored one. The DB columns
        (`image_url`, `image_thumbnail`) and the view are unchanged.

        The self-hosting ingestion pipeline (download the source image at scan time, store it
        in Cloudflare R2, serve our own URL) is designed but deferred. The Application-side
        `ICoverImageStore` interface (returning `StoredCover`) is in place; the Infrastructure
        implementation — HttpClient download + magic-byte validation + ImageSharp thumbnail +
        `AWSSDK.S3` upload to R2, wired in before persistence and outside the DB transaction,
        returning `null` on failure so book creation degrades to the external URL — is a later
        step, to be built once the R2 bucket exists. ISBNDB's `image_original` (high-res but
        expires ~2h after the API response) is the preferred ingest source precisely because
        ingestion downloads it immediately; `image` (≤500px, durable) is the fallback.
      - [x] `Msrp` was **removed** rather than modeled. It came only from ISBNDB as a
        currency-less `decimal?` (a broken money model — an amount without a currency) holding
        a US list price of little value to a personal Danish shelf; Saxo (the intended DK price
        source) only yields a product URL via `ISaxoBookSearch`, not a price. A future
        "collection value" feature can reintroduce price as a proper `Money` value object
        (amount + `Currency`), user-entered or from a real retail feed. The `msrp` column was
        dropped from `editions` and `editions_view`.
      - [x] `Pages` stays an `int?` (a range-checked scalar does not reach the value-object
        threshold) but its inline `> 0` check moved into a unit-tested `Helpers/PageCountNormalizer`,
        which also drops implausibly large counts (`> 50000`) to null, matching the other
        per-field normalizers.
      - [x] `BookDimensions` gained a `Reconstitute(...)` factory that rehydrates without
        re-sanitizing/re-rounding; `EditionMappingExtensions.ToDomain` now uses it instead of
        `Create`, so the DB→domain path no longer re-validates persisted data. The two API-merge
        call sites keep `Create` because they form new combined data from live results. This
        closes the "Create vs Reconstitute stays clean" concern for the `Edition` aggregate.
      - [x] `DataSource` (an enum recording whether a book came from an API, cache, or DB) was
        **removed**. Provenance of a *read* is not a property of the aggregate — an `Edition` is
        the same edition regardless of where it was fetched — so it never belonged on the domain
        model or in the `editions` table. Its driven logic was also largely vestigial: the enum
        was a broken `[Flags]` (no power-of-two values, so `HasFlag` was meaningless) and
        `DataSource.Cache` was never assigned in production. The one real consumer was the
        Blazor "already saved?" check (`_isSaved`), which now uses an honest
        `BooksApi.ExistsAsync(isbn)` call instead — more correct, since the old switch had a dead
        `"ExternalApi"` branch that mislabelled already-owned books as unsaved. The Console
        source-based filtering/ordering was dropped (title ordering kept), the `BookSourceBadge`
        component deleted, and the `data_source` column dropped from `editions`/`editions_view`.
- [x] **Reviewed the public setters** (`SetPublisher`, `SetSeries`). The
      `SetSeries` "null series with a number" concern was already resolved by the
      `SeriesPlacement` value object: the number is a property *inside* a placement
      that requires a non-null `Series`, so "number without series" is
      unrepresentable, not merely validated. All three write paths uphold it —
      `Work.SetSeries` (non-null param + `SeriesPlacement.Create`), the
      `SetBookSeries` command (always creates a `Series` first), and
      `Work.Reconstitute` (drops an orphan number when the series is absent). Added a
      characterisation test pinning that reconstitute-time orphan drop.
      (`UpdateDataSource` is gone with `DataSource`.) A DB-level
      `CHECK (series_number IS NULL OR series_id IS NOT NULL)` for defence-in-depth
      is noted but deferred.
- [x] **Audited the `Create` vs `Reconstitute` split across every aggregate and
      value object.** Most rehydration paths were already clean (`Publisher`,
      `Series`, `Author`, `Language`, `Cover`, `BookDimensions`,
      `GenreClassification`, and `Isbn`'s ISBN-13 path all use a `Reconstitute`/ctor).
      Three genuine leaks were fixed by giving each value object a non-validating
      `Reconstitute` and switching the reconstitution call site to it: **`Genre`**
      and **`DeweyDecimal`** (the persistence mapper re-ran the normalising/validating
      `TryCreate` on stored names/codes) and **`PublicationDate`** (`Edition.Reconstitute`
      re-parsed the canonical date string via `TryCreate`). Visibility follows the
      existing convention — `Genre`/`DeweyDecimal.Reconstitute` are `public` (called
      from Persistence), `PublicationDate.Reconstitute` is `internal` (called only from
      `Edition.Reconstitute`). One latent case is deliberately left: `Isbn`'s
      ISBN-10-only fallback in `Edition.Reconstitute` calls `Isbn.Create`, but it is
      unreachable because `Isbn.Value13` is always non-null, so a persisted edition
      always stores `isbn13`.
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

- [x] **`Work` gained a `Description` value object** alongside `Synopsis`
      (`Works/Description.cs`). Google Books returns two descriptions — a short synopsis
      (search result) and a fuller text including editorial content (full volume) — so
      `Synopsis` now holds the short teaser and `Description` the full text; both strip HTML
      via the shared `HtmlToPlainText` helper. `GoogleBookService` was changed to keep both
      instead of collapsing to one (`Synopsis` from the search result, `Description` from the
      volume), and `EditionWithWorkMerger` merges both fields. A `description text` column was
      added to `works`/`works_view`. Both are work-level because the text describes the book's
      content, identical across every edition — putting them on `Edition` would duplicate them
      per ISBN.

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
- [x] **Collapse the write-path N+1.** The `GetOrCreate` repositories (`Author`,
      `Genre`, `DeweyDecimal`) issued one round-trip per element, and the
      join-table inserts ran one Dapper execution per row. All now use a single
      `unnest`-based bulk upsert/insert per call: saving a book with N authors /
      genres / dewey codes is a constant number of statements instead of scaling
      with N. Duplicates within a batch are handled with `SELECT DISTINCT` (which
      also avoids Postgres's "ON CONFLICT DO UPDATE cannot affect row a second
      time"). Pinned by `GetOrCreateBatchTests`.
- [x] Verify multi-table writes run inside a transaction so a book plus its
      authors/genres commit or roll back together. The `IUnitOfWork` aggregate was
      replaced by a focused `ITransactionManager` (transactional boundary only);
      repositories are injected directly into the services that need them. A
      `CreateCommand` seam on `IDbContext` attaches the active transaction to
      every Dapper command, and `BeginTransactionAsync` guards against re-entrancy.
      Proven by the write round-trip and `PostgresDbContext` lifecycle tests.
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