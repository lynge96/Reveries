--
-- PostgreSQL database dump
--

\restrict x9VoRGbVnMOg77cN1bfZ0YFCQCQhBJhWWsUout0BhxupcusmWn2KbxQXhBG7HvR

-- Dumped from database version 18.3 (Debian 18.3-1.pgdg13+1)
-- Dumped by pg_dump version 18.3 (Debian 18.3-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: library; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA library;


--
-- Name: SCHEMA library; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON SCHEMA library IS 'standard public schema';


--
-- Name: pg_trgm; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS pg_trgm WITH SCHEMA library;


--
-- Name: EXTENSION pg_trgm; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON EXTENSION pg_trgm IS 'text similarity measurement and index searching based on trigrams';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: author_name_variants; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.author_name_variants (
    id integer NOT NULL,
    name_variant character varying NOT NULL,
    is_primary boolean DEFAULT false NOT NULL,
    author_id uuid NOT NULL
);


--
-- Name: author_name_variants_id_seq; Type: SEQUENCE; Schema: library; Owner: -
--

CREATE SEQUENCE library.author_name_variants_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: author_name_variants_id_seq; Type: SEQUENCE OWNED BY; Schema: library; Owner: -
--

ALTER SEQUENCE library.author_name_variants_id_seq OWNED BY library.author_name_variants.id;


--
-- Name: authors; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.authors (
    id uuid CONSTRAINT authors_domain_id_not_null NOT NULL,
    normalized_name character varying NOT NULL,
    first_name character varying,
    last_name character varying,
    date_created timestamp without time zone DEFAULT now() NOT NULL
);


--
-- Name: books; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.books (
    id uuid CONSTRAINT books_domain_id_not_null NOT NULL,
    title character varying NOT NULL,
    isbn13 character varying,
    isbn10 character varying,
    series_number integer,
    publication_date character varying,
    page_count integer,
    synopsis text,
    language character varying,
    edition character varying,
    binding character varying,
    image_url text,
    image_thumbnail text,
    msrp numeric,
    is_read boolean DEFAULT false NOT NULL,
    height_cm numeric,
    width_cm numeric,
    thickness_cm numeric,
    weight_g numeric,
    date_created timestamp without time zone DEFAULT now() NOT NULL,
    publisher_id uuid,
    series_id uuid
);


--
-- Name: books_authors; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.books_authors (
    book_id uuid NOT NULL,
    author_id uuid NOT NULL
);


--
-- Name: books_dewey_decimals; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.books_dewey_decimals (
    dewey_decimal_id integer NOT NULL,
    book_id uuid NOT NULL
);


--
-- Name: books_genres; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.books_genres (
    genre_id integer NOT NULL,
    book_id uuid NOT NULL
);


--
-- Name: dewey_decimals; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.dewey_decimals (
    id integer NOT NULL,
    code character varying(20) NOT NULL,
    date_created timestamp without time zone DEFAULT now() NOT NULL
);


--
-- Name: genres; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.genres (
    id integer NOT NULL,
    name character varying NOT NULL,
    date_created timestamp without time zone DEFAULT now() NOT NULL
);


--
-- Name: publishers; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.publishers (
    id uuid CONSTRAINT publishers_domain_id_not_null NOT NULL,
    name character varying NOT NULL,
    date_created timestamp without time zone DEFAULT now() NOT NULL
);


--
-- Name: series; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.series (
    id uuid CONSTRAINT series_domain_id_not_null NOT NULL,
    name character varying NOT NULL,
    date_created timestamp without time zone DEFAULT now() NOT NULL
);


--
-- Name: book_details_flat; Type: VIEW; Schema: library; Owner: -
--

CREATE VIEW library.book_details_flat AS
 SELECT b.id,
    b.title,
    b.isbn13,
    b.isbn10,
    b.publication_date AS publicationdate,
    b.page_count AS pagecount,
    b.synopsis,
    b.language,
    b.edition,
    b.binding,
    b.image_url AS coverimageurl,
    b.image_thumbnail AS imagethumbnailurl,
    b.msrp,
    b.is_read AS isread,
    b.series_number AS seriesnumber,
    b.height_cm AS heightcm,
    b.width_cm AS widthcm,
    b.thickness_cm AS thicknesscm,
    b.weight_g AS weightg,
    b.date_created AS datecreatedbook,
    p.id AS publisherid,
    p.name AS publishername,
    p.date_created AS datecreatedpublisher,
    a.id AS authorid,
    a.normalized_name AS normalizedname,
    a.first_name AS firstname,
    a.last_name AS lastname,
    a.date_created AS datecreatedauthor,
    g.id AS genreid,
    g.name AS genrename,
    g.date_created AS datecreatedgenre,
    dd.code,
    dd.date_created AS datecreateddeweydecimal,
    se.id AS seriesid,
    se.name AS seriesname,
    se.date_created AS datecreatedseries
   FROM ((((((((library.books b
     LEFT JOIN library.publishers p ON ((b.publisher_id = p.id)))
     LEFT JOIN library.books_authors ba ON ((b.id = ba.book_id)))
     LEFT JOIN library.authors a ON ((ba.author_id = a.id)))
     LEFT JOIN library.books_genres bg ON ((b.id = bg.book_id)))
     LEFT JOIN library.genres g ON ((bg.genre_id = g.id)))
     LEFT JOIN library.books_dewey_decimals bdd ON ((b.id = bdd.book_id)))
     LEFT JOIN library.dewey_decimals dd ON ((dd.id = bdd.dewey_decimal_id)))
     LEFT JOIN library.series se ON ((b.series_id = se.id)));


--
-- Name: books_view; Type: VIEW; Schema: library; Owner: -
--

CREATE VIEW library.books_view AS
 SELECT b.id,
    b.title,
    b.isbn13,
    b.isbn10,
    b.publication_date AS "publicationDate",
    b.page_count AS "pageCount",
    b.synopsis,
    b.language,
    b.edition,
    b.binding,
    b.image_url AS "coverImageUrl",
    b.image_thumbnail AS "imageThumbnailUrl",
    b.msrp,
    b.is_read AS "isRead",
    b.series_number AS "seriesNumber",
    b.height_cm AS "heightCm",
    b.width_cm AS "widthCm",
    b.thickness_cm AS "thicknessCm",
    b.weight_g AS "weightG",
    b.date_created AS "dateCreatedBook",
    p.id AS "publisherId",
    p.name AS "publisherName",
    p.date_created AS "dateCreatedPublisher",
    se.id AS "seriesId",
    se.name AS "seriesName",
    se.date_created AS "dateCreatedSeries",
    COALESCE(g.genres_json, '[]'::jsonb) AS genres,
    COALESCE(a.authors_json, '[]'::jsonb) AS authors,
    COALESCE(dd.dewey_codes, (ARRAY[]::text[])::character varying[]) AS "deweyCodes"
   FROM (((((library.books b
     LEFT JOIN library.publishers p ON ((b.publisher_id = p.id)))
     LEFT JOIN library.series se ON ((b.series_id = se.id)))
     LEFT JOIN LATERAL ( SELECT jsonb_agg(jsonb_build_object('Id', g_1.id, 'Name', g_1.name, 'DateCreated', g_1.date_created) ORDER BY g_1.name) AS genres_json
           FROM (library.books_genres bg
             JOIN library.genres g_1 ON ((bg.genre_id = g_1.id)))
          WHERE (bg.book_id = b.id)) g ON (true))
     LEFT JOIN LATERAL ( SELECT jsonb_agg(jsonb_build_object('Id', a_1.id, 'NormalizedName', a_1.normalized_name, 'FirstName', a_1.first_name, 'LastName', a_1.last_name, 'DateCreated', a_1.date_created) ORDER BY a_1.normalized_name) AS authors_json
           FROM (library.books_authors ba
             JOIN library.authors a_1 ON ((ba.author_id = a_1.id)))
          WHERE (ba.book_id = b.id)) a ON (true))
     LEFT JOIN LATERAL ( SELECT array_agg(DISTINCT dd_1.code ORDER BY dd_1.code) AS dewey_codes
           FROM (library.books_dewey_decimals bdd
             JOIN library.dewey_decimals dd_1 ON ((dd_1.id = bdd.dewey_decimal_id)))
          WHERE (bdd.book_id = b.id)) dd ON (true));


--
-- Name: dewey_decimals_id_seq; Type: SEQUENCE; Schema: library; Owner: -
--

CREATE SEQUENCE library.dewey_decimals_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: dewey_decimals_id_seq; Type: SEQUENCE OWNED BY; Schema: library; Owner: -
--

ALTER SEQUENCE library.dewey_decimals_id_seq OWNED BY library.dewey_decimals.id;


--
-- Name: genres_id_seq; Type: SEQUENCE; Schema: library; Owner: -
--

CREATE SEQUENCE library.genres_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: genres_id_seq; Type: SEQUENCE OWNED BY; Schema: library; Owner: -
--

ALTER SEQUENCE library.genres_id_seq OWNED BY library.genres.id;


--
-- Name: author_name_variants id; Type: DEFAULT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.author_name_variants ALTER COLUMN id SET DEFAULT nextval('library.author_name_variants_id_seq'::regclass);


--
-- Name: dewey_decimals id; Type: DEFAULT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.dewey_decimals ALTER COLUMN id SET DEFAULT nextval('library.dewey_decimals_id_seq'::regclass);


--
-- Name: genres id; Type: DEFAULT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.genres ALTER COLUMN id SET DEFAULT nextval('library.genres_id_seq'::regclass);


--
-- Name: author_name_variants author_name_variants_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.author_name_variants
    ADD CONSTRAINT author_name_variants_pkey PRIMARY KEY (id);


--
-- Name: books_authors books_authors_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_authors
    ADD CONSTRAINT books_authors_pkey PRIMARY KEY (book_id, author_id);


--
-- Name: books_dewey_decimals books_dewey_decimals_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_dewey_decimals
    ADD CONSTRAINT books_dewey_decimals_pkey PRIMARY KEY (book_id, dewey_decimal_id);


--
-- Name: books_genres books_genres_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_genres
    ADD CONSTRAINT books_genres_pkey PRIMARY KEY (book_id, genre_id);


--
-- Name: books books_isbn10_key; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books
    ADD CONSTRAINT books_isbn10_key UNIQUE (isbn10);


--
-- Name: books books_isbn13_key; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books
    ADD CONSTRAINT books_isbn13_key UNIQUE (isbn13);


--
-- Name: books books_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books
    ADD CONSTRAINT books_pkey PRIMARY KEY (id);


--
-- Name: dewey_decimals dewey_decimals_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.dewey_decimals
    ADD CONSTRAINT dewey_decimals_pkey PRIMARY KEY (id);


--
-- Name: genres genres_name_key; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.genres
    ADD CONSTRAINT genres_name_key UNIQUE (name);


--
-- Name: genres genres_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.genres
    ADD CONSTRAINT genres_pkey PRIMARY KEY (id);


--
-- Name: authors idx_authors_normalized_name_key; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.authors
    ADD CONSTRAINT idx_authors_normalized_name_key UNIQUE (normalized_name);


--
-- Name: authors idx_authors_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.authors
    ADD CONSTRAINT idx_authors_pkey PRIMARY KEY (id);


--
-- Name: publishers publishers_name_key; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.publishers
    ADD CONSTRAINT publishers_name_key UNIQUE (name);


--
-- Name: publishers publishers_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.publishers
    ADD CONSTRAINT publishers_pkey PRIMARY KEY (id);


--
-- Name: series series_name_key; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.series
    ADD CONSTRAINT series_name_key UNIQUE (name);


--
-- Name: series series_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.series
    ADD CONSTRAINT series_pkey PRIMARY KEY (id);


--
-- Name: dewey_decimals uq_dewey_decimals_code; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.dewey_decimals
    ADD CONSTRAINT uq_dewey_decimals_code UNIQUE (code);


--
-- Name: idx_author_name_variants_author_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_author_name_variants_author_id ON library.author_name_variants USING btree (author_id);


--
-- Name: idx_books_authors_author_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_books_authors_author_id ON library.books_authors USING btree (author_id);


--
-- Name: idx_books_authors_book_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_books_authors_book_id ON library.books_authors USING btree (book_id);


--
-- Name: idx_books_dewey_decimals_book_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_books_dewey_decimals_book_id ON library.books_dewey_decimals USING btree (book_id);


--
-- Name: idx_books_dewey_decimals_dewey_decimal_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_books_dewey_decimals_dewey_decimal_id ON library.books_dewey_decimals USING btree (dewey_decimal_id);


--
-- Name: idx_books_genres_book_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_books_genres_book_id ON library.books_genres USING btree (book_id);


--
-- Name: idx_books_genres_genre_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_books_genres_genre_id ON library.books_genres USING btree (genre_id);


--
-- Name: idx_books_publisher_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_books_publisher_id ON library.books USING btree (publisher_id);


--
-- Name: idx_books_series_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_books_series_id ON library.books USING btree (series_id);


--
-- Name: idx_books_title; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_books_title ON library.books USING btree (title);


--
-- Name: author_name_variants fk_author_name_variants_author_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.author_name_variants
    ADD CONSTRAINT fk_author_name_variants_author_id FOREIGN KEY (author_id) REFERENCES library.authors(id) ON DELETE CASCADE;


--
-- Name: books_authors fk_books_authors_author_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_authors
    ADD CONSTRAINT fk_books_authors_author_id FOREIGN KEY (author_id) REFERENCES library.authors(id) ON DELETE CASCADE;


--
-- Name: books_authors fk_books_authors_book_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_authors
    ADD CONSTRAINT fk_books_authors_book_id FOREIGN KEY (book_id) REFERENCES library.books(id) ON DELETE CASCADE;


--
-- Name: books_dewey_decimals fk_books_dewey_decimals_book_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_dewey_decimals
    ADD CONSTRAINT fk_books_dewey_decimals_book_id FOREIGN KEY (book_id) REFERENCES library.books(id) ON DELETE CASCADE;


--
-- Name: books_dewey_decimals fk_books_dewey_decimals_dewey_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_dewey_decimals
    ADD CONSTRAINT fk_books_dewey_decimals_dewey_id FOREIGN KEY (dewey_decimal_id) REFERENCES library.dewey_decimals(id) ON DELETE CASCADE;


--
-- Name: books_genres fk_books_genres_book_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_genres
    ADD CONSTRAINT fk_books_genres_book_id FOREIGN KEY (book_id) REFERENCES library.books(id) ON DELETE CASCADE;


--
-- Name: books_genres fk_books_genres_genre_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_genres
    ADD CONSTRAINT fk_books_genres_genre_id FOREIGN KEY (genre_id) REFERENCES library.genres(id) ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict x9VoRGbVnMOg77cN1bfZ0YFCQCQhBJhWWsUout0BhxupcusmWn2KbxQXhBG7HvR

