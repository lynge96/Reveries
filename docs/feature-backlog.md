# Feature Backlog

Deferred feature ideas, captured so they are not forgotten. This is intentionally
separate from `refactor-roadmap.md`, which is the consolidation/hardening pass and
explicitly scopes new features out. Items here are picked up *after* that pass, and
nothing here is committed to a phase yet.

---

## Saxo product link on `Edition`

Show a link to each book's page in the Saxo online bookshop (`saxo.com`), so a book
in the catalogue can deep-link out to where it can be bought or previewed.

**Approach — ISBN deep-link, no scraping.** Searching Saxo by ISBN lands directly on
the product page, so the link is just a constructed URL:

```
https://www.saxo.com/dk/products/search?query={isbn13}
```

This is a plain deep link, not data extraction — no HTML scraping, no metadata pulled
from Saxo, so it sidesteps their bot protection and the EU database-right concerns
that real scraping would raise.

**Why not the affiliate feed.** Saxo's Partner-Ads product feed (ISBN → canonical
product URL, with commission) would be the cleaner source, but it is only open to
publishers and authors — not a third-party personal app — so it is not available to
this project.

**Where it belongs.** The link is edition-specific (one ISBN = one Saxo product), so
it sits on `Edition`, next to `CoverImageUrl` and `Msrp`.

**Status (partly implemented).** `Edition` now carries a stored `SaxoUrl` value object
(`Reveries.Domain/Editions/SaxoUrl.cs`) that flows through all layers and the
`editions` table. `SaxoUrl.TryCreate` validates *shape* only — an absolute `https`
URL on a Saxo host (`saxo.com`/`saxo.dk`) — and returns `null` otherwise, following
the TryCreate-skip pattern. Nothing populates the field yet: every mapper passes
`null`. Storage vs. derive-on-read is therefore settled as **stored**.

**The integration seam.** `ISaxoBookSearch`
(`Reveries.Application/Books/Interfaces`) is the Application contract for populating
the field: `Isbn → SaxoUrl?`. A future `Reveries.Integration/Saxo/` slice implements
it — with its own `AddSaxo(configuration)` extension, matching the `GoogleBooks/` and
`Isbndb/` pattern — and `BookLookupService` enriches each looked-up edition through
it (`Edition` needs an `AssignSaxoUrl` mutator, mirroring `SetPublisher`). Note the
existence caveat: a plain `search?query={isbn}` deep link always "resolves" — Saxo
answers `200` with a results page even for an unknown ISBN — so it does **not** prove
the specific book exists. A verifying implementation must follow the search through to
a canonical product URL and treat a no-hit result as `null`, guarding against soft-404
pages that return `200` for missing products.

**If it grows to more shops.** Should other Danish shops (Bog & idé, William Dam, …)
be added later, put the link behind an `IStoreLinkProvider` abstraction in a
`Reveries.Integration/Saxo/` slice with its own `AddSaxo(configuration)` extension —
matching the existing `GoogleBooks/` and `Isbndb/` integration pattern — so an
`Edition` can carry several store links without changing the domain.

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