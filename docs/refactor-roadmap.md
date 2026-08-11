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

## Phase 0 — Safety net (the keystone)

The one piece of work that de-risks both the domain and the query work at once.

- [ ] Add **Testcontainers** (real Postgres in a throwaway container) to
      `Reveries.Application.Tests`; delete the placeholder `UnitTest1.cs`.
- [ ] Write characterisation **integration tests** for the critical path:
      scan ISBN → enrich (with stubbed ISBNDB / Google Books HTTP responses) →
      persist → read back. Assert the round-tripped `Book` matches what went in.
- [ ] Add **repository-level tests** that exercise the hand-written Dapper SQL
      against real Postgres — these are the safety net for Phase 2. Cover at
      least `BookRepository` hydration (book + authors + genres + dewey + series).
- [ ] Wire the new integration tests into CI (`pr.yml`) — Docker is already
      available in the pipeline, so Testcontainers runs there unchanged.

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
- [ ] Verify multi-table writes run inside a transaction via
      `IUnitOfWork.BeginTransactionAsync(ct)` so a book plus its authors/genres
      commit or roll back together.
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