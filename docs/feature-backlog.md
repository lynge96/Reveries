# Feature Backlog

Deferred feature ideas, captured so they are not forgotten. This is intentionally
separate from `refactor-roadmap.md`, which is the consolidation/hardening pass and
explicitly scopes new features out. Items here are picked up *after* that pass, and
nothing here is committed to a phase yet.

---

## Saxo product link on `Edition` — ✅ shipped (deep-link)

A per-book link to the Saxo online bookshop, so a catalogue entry can deep-link out to
where it can be bought or previewed.

**Built as an ISBN search deep link (no scraping).** `SaxoBookSearch` in the
`Reveries.Integration/Saxo/` slice implements `ISaxoBookSearch` (`Isbn → SaxoUrl?`) behind
its own `AddSaxo(configuration)` extension, matching the `GoogleBooks/`/`Isbndb/` pattern. It
builds an ISBN-13 search URL from a configurable `Saxo:SearchUrlTemplate` (default
`https://www.saxo.com/dk/products/search?query={0}`) — pure URL construction, no HTTP call.
The link is resolved off a shared seam in two places: `BookLookupService` enriches each
looked-up `BookCandidate` (so `GET /books/isbn/{isbn}` returns it before save), and
`WorkPersistenceService` resolves it **before opening the write transaction** and stores it on
the `Edition` (so the persisted row always has it — `CreateBookRequest` does not round-trip the
field). Both are best-effort. The stored `SaxoUrl` value object validates *shape* only (absolute
`https` on a Saxo host), following the TryCreate-skip pattern; it flows through all layers, the
`editions` table, and the API DTO.

**Why deep-link, not resolve-and-verify.** Verifying against a canonical product URL would need
to fetch and parse Saxo's search/product pages — which Saxo's `robots.txt` `Disallow`s for
`User-agent: *` (`*/ean/*`, `/search/`, `*/productpage/*`, `*/item_*`), which raises the EU
sui-generis database right, and which is fragile behind their bot protection. A stored deep link
fetches nothing (a human clicks it), so it sits outside those concerns. The trade-off: it does
**not** prove the book exists on Saxo — an unknown ISBN lands on an empty search page. The
Partner-Ads affiliate feed (ISBN → canonical URL) would be the clean source but is publisher/author
only.

**Open:** verify the exact working search-URL format in a browser and adjust
`Saxo:SearchUrlTemplate` if needed; the `SaxoUrl` host allow-list already rejects a
misconfigured template (invalid → `null`).

**If it grows to more shops** (Bog & idé, William Dam, …), put the link behind an
`IStoreLinkProvider` abstraction so an `Edition` can carry several store links without changing
the domain.

**Open questions.**

- Use ISBN-13 or fall back to ISBN-10 when only one is present.
- Deep-link only (cheap, but unverified) vs. resolve-and-verify the canonical product
  URL behind `ISaxoBookSearch` (proves existence, but needs a network call plus
  soft-404 handling); if the latter, cache `isbn → SaxoUrl?` in Redis (including
  negative hits) and keep the enrichment step best-effort so a Saxo failure never
  blocks saving the book.

---

## Personal & multi-user layer: Copy, Users, Shelves, reading tracking

These four are one cluster — the personal-ownership and social layer that sits on top
of the bibliographic model (`Work` / `Edition`). They share a dependency order:
**Users** is the foundation; **Copy** and **Shelves** hang off a user; **reading
tracking** is a status on a user's copy. The `Copy` aggregate is already anticipated
in the domain model (the third FRBR level, `Item`), currently deferred.

Background: `IsRead` was deliberately removed in full (field, service, endpoint, and
column) rather than parked on `Edition`, because read status is per-user, not per-
edition. It returns here, on a user's copy.

### Users

Introduce a user/account concept so the catalogue stops being implicitly single-user.
Foundation for shelves, ownership, and per-user reading status — everything below
references a user.

Open questions: authentication approach; whether existing data is backfilled to a
single seed user.

### Copy (FRBR Item)

A concrete physical copy a user owns, referencing the edition it is a copy of:
`CopyId → EditionId` (and, once Users exist, an owning `UserId`). This is where a scan
ultimately lands: ISBN → find-or-create `Edition` (→ find-or-create `Work`), then
attach a `Copy` to the shelf.

Carries per-copy facts distinct from the edition: condition, acquisition date, notes,
and reading status (below).

Open questions: does `Copy` require a `UserId` from the start, or is single-user
ownership implicit until Users ships?

### Shelves

Per-user collections that group copies (e.g. "read", "to-read", "wishlist", or custom
shelves). A copy can sit on one or more shelves.

Open questions: fixed system shelves vs. free-form user shelves; whether "to-read" /
"read" are shelves or a reading-status field on `Copy` (see below).

### Reading tracking

Bring back read/unread — and richer status — on a user's `Copy`, not on `Edition`.
Enables the reading insights and read/unread statistics named in the README vision.

Open questions: a simple read/unread flag vs. richer states (reading, finished,
abandoned, with dates and rating); whether this is modelled as shelf membership or a
first-class status on `Copy`.