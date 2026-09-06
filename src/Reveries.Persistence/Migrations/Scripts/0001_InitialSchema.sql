CREATE SCHEMA catalog;

CREATE EXTENSION IF NOT EXISTS citext WITH SCHEMA public;

CREATE TABLE catalog.authors (
    id uuid NOT NULL PRIMARY KEY,
    name citext NOT NULL UNIQUE,
    created_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE catalog.publishers (
    id uuid NOT NULL PRIMARY KEY,
    name citext NOT NULL UNIQUE,
    created_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE catalog.series (
    id uuid NOT NULL PRIMARY KEY,
    name citext NOT NULL UNIQUE,
    created_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE catalog.genres (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name citext NOT NULL UNIQUE,
    created_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE catalog.dewey_decimals (
    id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    code text NOT NULL UNIQUE,
    created_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE catalog.works (
    id uuid NOT NULL PRIMARY KEY,
    title text NOT NULL,
    subtitle text,
    synopsis text,
    description text,
    series_id uuid REFERENCES catalog.series (id),
    series_number int,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT works_series_number_requires_series CHECK (series_number IS NULL OR series_id IS NOT NULL)
);

CREATE TABLE catalog.editions (
    id uuid NOT NULL PRIMARY KEY,
    work_id uuid NOT NULL REFERENCES catalog.works (id) ON DELETE CASCADE,
    isbn13 text UNIQUE,
    isbn10 text UNIQUE,
    publication_date text,
    page_count int,
    language text,
    edition_statement text,
    format text NOT NULL,
    image_url text,
    image_thumbnail text,
    saxo_url text,
    height_cm numeric,
    width_cm numeric,
    thickness_cm numeric,
    weight_g numeric,
    publisher_id uuid REFERENCES catalog.publishers (id),
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT editions_requires_isbn CHECK (isbn13 IS NOT NULL OR isbn10 IS NOT NULL)
);

CREATE TABLE catalog.works_authors (
    work_id uuid NOT NULL REFERENCES catalog.works (id) ON DELETE CASCADE,
    author_id uuid NOT NULL REFERENCES catalog.authors (id) ON DELETE CASCADE,
    PRIMARY KEY (work_id, author_id)
);

CREATE TABLE catalog.works_genres (
    work_id uuid NOT NULL REFERENCES catalog.works (id) ON DELETE CASCADE,
    genre_id int NOT NULL REFERENCES catalog.genres (id) ON DELETE CASCADE,
    is_primary boolean NOT NULL DEFAULT false,
    PRIMARY KEY (work_id, genre_id)
);

CREATE TABLE catalog.works_dewey_decimals (
    work_id uuid NOT NULL REFERENCES catalog.works (id) ON DELETE CASCADE,
    dewey_decimal_id int NOT NULL REFERENCES catalog.dewey_decimals (id) ON DELETE CASCADE,
    PRIMARY KEY (work_id, dewey_decimal_id)
);

CREATE INDEX idx_works_series_id ON catalog.works (series_id);
CREATE INDEX idx_works_title ON catalog.works (title);
CREATE INDEX idx_editions_work_id ON catalog.editions (work_id);
CREATE INDEX idx_editions_publisher_id ON catalog.editions (publisher_id);
CREATE INDEX idx_works_authors_author_id ON catalog.works_authors (author_id);
CREATE INDEX idx_works_genres_genre_id ON catalog.works_genres (genre_id);
CREATE INDEX idx_works_dewey_decimals_dewey_decimal_id ON catalog.works_dewey_decimals (dewey_decimal_id);
