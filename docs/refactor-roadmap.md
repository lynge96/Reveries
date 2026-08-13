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

Layer boundaries that assembly separation no longer enforces are recovered by a
**`Reveries.Architecture.Tests`** project using **NetArchTest**, asserting the
layer rules on the compiled namespaces. Four rules are in place:

1. Domain depends on no outer layer.
2. Application depends only on Domain (not Contracts, Infrastructure, Persistence,
   Integration, or Api).
3. Contracts has no dependency on Domain (no domain type crosses the API boundary).
4. The concrete `Reveries.Persistence.Repositories` types do not leak out of
   Persistence — outer layers reach them only through the `IRepository`
   interfaces and the `AddPostgresql` DI extension.

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

- [ ] **`Book.Create`'s ~21-parameter signature** (`Domain/Models/Book.cs`)
      is a long-parameter-list smell and is asymmetric with `Reconstitute`,
      which already takes a single `BookReconstitutionData`. Introduce a
      `BookCreationData` (or similar) input record so both factories take a
      parameter object.
- [ ] **Weakly-typed fields → value objects / enums:**
      - `PublicationDate` is `string?` — model it as a real date (or a
        `PublicationDate` value object that can hold partial dates like
        year-only, which external APIs often return).
      - `Binding` is `string?` even though `Enums/BindingType.cs` exists and is
        unused on `Book`. Decide: enum or value object, then use it consistently.
      - Consider a `Language` value object holding both the ISO-639 code and the
        display name; today `Create` stores only the display name
        (`languageIso639.GetLanguageName()`), which is lossy.
- [ ] **Review the public setters** (`SetPublisher`, `SetSeries`,
      `UpdateDataSource`) for invariant coverage. `SetSeries` validates the
      number but allows a null series with a number — decide whether that pairing
      is legal and enforce it.
- [ ] Confirm the `Create` vs `Reconstitute` split stays clean: all
      validation/normalisation in `Create`, none in `Reconstitute`.
- [ ] **Introduce migration tooling here** — the domain changes above are the
      first schema changes, so this is the natural point. Adopt **DbUp**
      (lightweight, plain-SQL, no EF) so schema changes become one versioned
      migration file instead of the current three hand-edits (domain type,
      Dapper SQL, `infra/db_schema.sql`). Backfill an initial baseline migration
      from the existing `db_schema.sql`.

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