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
it sits on `Edition`, next to `CoverImageUrl` and `Msrp`. Because it derives purely
from the ISBN, it can be a computed value produced at read/mapping time rather than a
stored column — decide storage vs. derive-on-read when building it.

**If it grows to more shops.** Should other Danish shops (Bog & idé, William Dam, …)
be added later, put the link behind an `IStoreLinkProvider` abstraction in a
`Reveries.Integration/Saxo/` slice with its own `AddSaxo(configuration)` extension —
matching the existing `GoogleBooks/` and `Isbndb/` integration pattern — so an
`Edition` can carry several store links without changing the domain.

**Open questions.**

- Store the URL on `Edition` vs. derive it from the ISBN on read.
- Use ISBN-13 or fall back to ISBN-10 when only one is present.

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