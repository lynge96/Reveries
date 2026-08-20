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
-- Name: works; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.works (
    id uuid CONSTRAINT works_domain_id_not_null NOT NULL,
    title character varying NOT NULL,
    synopsis text,
    series_number integer,
    series_id uuid,
    date_created timestamp without time zone DEFAULT now() NOT NULL
);


--
-- Name: editions; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.editions (
    id uuid CONSTRAINT editions_domain_id_not_null NOT NULL,
    work_id uuid NOT NULL,
    isbn13 character varying,
    isbn10 character varying,
    publication_date character varying,
    page_count integer,
    language character varying,
    edition_statement character varying,
    binding character varying,
    image_url text,
    image_thumbnail text,
    msrp numeric,
    height_cm numeric,
    width_cm numeric,
    thickness_cm numeric,
    weight_g numeric,
    data_source character varying NOT NULL,
    publisher_id uuid,
    date_created timestamp without time zone DEFAULT now() NOT NULL
);


--
-- Name: works_authors; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.works_authors (
    work_id uuid NOT NULL,
    author_id uuid NOT NULL
);


--
-- Name: works_dewey_decimals; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.works_dewey_decimals (
    dewey_decimal_id integer NOT NULL,
    work_id uuid NOT NULL
);


--
-- Name: works_genres; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.works_genres (
    genre_id integer NOT NULL,
    work_id uuid NOT NULL,
    is_primary boolean DEFAULT false NOT NULL
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
-- Name: works_view; Type: VIEW; Schema: library; Owner: -
--

CREATE VIEW library.works_view AS
 SELECT w.id,
    w.title,
    w.synopsis,
    w.series_number AS "seriesNumber",
    w.date_created AS "dateCreatedWork",
    se.id AS "seriesId",
    se.name AS "seriesName",
    se.date_created AS "dateCreatedSeries",
    COALESCE(g.primary_genres_json, '[]'::jsonb) AS "primaryGenres",
    COALESCE(g.secondary_genres_json, '[]'::jsonb) AS "secondaryGenres",
    COALESCE(a.authors_json, '[]'::jsonb) AS authors,
    COALESCE(dd.dewey_codes, (ARRAY[]::text[])::character varying[]) AS "deweyCodes"
   FROM ((((library.works w
     LEFT JOIN library.series se ON ((w.series_id = se.id)))
     LEFT JOIN LATERAL ( SELECT jsonb_agg(jsonb_build_object('Id', g_1.id, 'Name', g_1.name, 'DateCreated', g_1.date_created) ORDER BY g_1.name) FILTER (WHERE wg.is_primary) AS primary_genres_json,
            jsonb_agg(jsonb_build_object('Id', g_1.id, 'Name', g_1.name, 'DateCreated', g_1.date_created) ORDER BY g_1.name) FILTER (WHERE (NOT wg.is_primary)) AS secondary_genres_json
           FROM (library.works_genres wg
             JOIN library.genres g_1 ON ((wg.genre_id = g_1.id)))
          WHERE (wg.work_id = w.id)) g ON (true))
     LEFT JOIN LATERAL ( SELECT jsonb_agg(jsonb_build_object('Id', a_1.id, 'NormalizedName', a_1.normalized_name, 'FirstName', a_1.first_name, 'LastName', a_1.last_name, 'DateCreated', a_1.date_created) ORDER BY a_1.normalized_name) AS authors_json
           FROM (library.works_authors wa
             JOIN library.authors a_1 ON ((wa.author_id = a_1.id)))
          WHERE (wa.work_id = w.id)) a ON (true))
     LEFT JOIN LATERAL ( SELECT array_agg(DISTINCT dd_1.code ORDER BY dd_1.code) AS dewey_codes
           FROM (library.works_dewey_decimals wdd
             JOIN library.dewey_decimals dd_1 ON ((dd_1.id = wdd.dewey_decimal_id)))
          WHERE (wdd.work_id = w.id)) dd ON (true));


--
-- Name: editions_view; Type: VIEW; Schema: library; Owner: -
--

CREATE VIEW library.editions_view AS
 SELECT e.id,
    e.work_id AS "workId",
    e.isbn13,
    e.isbn10,
    e.publication_date AS "publicationDate",
    e.page_count AS "pageCount",
    e.language,
    e.edition_statement AS "editionStatement",
    e.binding,
    e.image_url AS "coverImageUrl",
    e.image_thumbnail AS "imageThumbnailUrl",
    e.msrp,
    e.height_cm AS "heightCm",
    e.width_cm AS "widthCm",
    e.thickness_cm AS "thicknessCm",
    e.weight_g AS "weightG",
    e.data_source AS "dataSource",
    e.date_created AS "dateCreatedEdition",
    p.id AS "publisherId",
    p.name AS "publisherName",
    p.date_created AS "dateCreatedPublisher"
   FROM (library.editions e
     LEFT JOIN library.publishers p ON ((e.publisher_id = p.id)));


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
-- Name: works_authors works_authors_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.works_authors
    ADD CONSTRAINT works_authors_pkey PRIMARY KEY (work_id, author_id);


--
-- Name: works_dewey_decimals works_dewey_decimals_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.works_dewey_decimals
    ADD CONSTRAINT works_dewey_decimals_pkey PRIMARY KEY (work_id, dewey_decimal_id);


--
-- Name: works_genres works_genres_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.works_genres
    ADD CONSTRAINT works_genres_pkey PRIMARY KEY (work_id, genre_id);


--
-- Name: editions editions_isbn10_key; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.editions
    ADD CONSTRAINT editions_isbn10_key UNIQUE (isbn10);


--
-- Name: editions editions_isbn13_key; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.editions
    ADD CONSTRAINT editions_isbn13_key UNIQUE (isbn13);


--
-- Name: editions editions_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.editions
    ADD CONSTRAINT editions_pkey PRIMARY KEY (id);


--
-- Name: works works_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.works
    ADD CONSTRAINT works_pkey PRIMARY KEY (id);


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
-- Name: idx_works_authors_author_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_works_authors_author_id ON library.works_authors USING btree (author_id);


--
-- Name: idx_works_authors_work_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_works_authors_work_id ON library.works_authors USING btree (work_id);


--
-- Name: idx_works_dewey_decimals_work_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_works_dewey_decimals_work_id ON library.works_dewey_decimals USING btree (work_id);


--
-- Name: idx_works_dewey_decimals_dewey_decimal_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_works_dewey_decimals_dewey_decimal_id ON library.works_dewey_decimals USING btree (dewey_decimal_id);


--
-- Name: idx_works_genres_work_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_works_genres_work_id ON library.works_genres USING btree (work_id);


--
-- Name: idx_works_genres_genre_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_works_genres_genre_id ON library.works_genres USING btree (genre_id);


--
-- Name: idx_editions_work_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_editions_work_id ON library.editions USING btree (work_id);


--
-- Name: idx_editions_publisher_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_editions_publisher_id ON library.editions USING btree (publisher_id);


--
-- Name: idx_works_series_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_works_series_id ON library.works USING btree (series_id);


--
-- Name: idx_works_title; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_works_title ON library.works USING btree (title);


--
-- Name: author_name_variants fk_author_name_variants_author_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.author_name_variants
    ADD CONSTRAINT fk_author_name_variants_author_id FOREIGN KEY (author_id) REFERENCES library.authors(id) ON DELETE CASCADE;


--
-- Name: editions fk_editions_work_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.editions
    ADD CONSTRAINT fk_editions_work_id FOREIGN KEY (work_id) REFERENCES library.works(id) ON DELETE CASCADE;


--
-- Name: works_authors fk_works_authors_author_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.works_authors
    ADD CONSTRAINT fk_works_authors_author_id FOREIGN KEY (author_id) REFERENCES library.authors(id) ON DELETE CASCADE;


--
-- Name: works_authors fk_works_authors_work_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.works_authors
    ADD CONSTRAINT fk_works_authors_work_id FOREIGN KEY (work_id) REFERENCES library.works(id) ON DELETE CASCADE;


--
-- Name: works_dewey_decimals fk_works_dewey_decimals_dewey_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.works_dewey_decimals
    ADD CONSTRAINT fk_works_dewey_decimals_dewey_id FOREIGN KEY (dewey_decimal_id) REFERENCES library.dewey_decimals(id) ON DELETE CASCADE;


--
-- Name: works_dewey_decimals fk_works_dewey_decimals_work_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.works_dewey_decimals
    ADD CONSTRAINT fk_works_dewey_decimals_work_id FOREIGN KEY (work_id) REFERENCES library.works(id) ON DELETE CASCADE;


--
-- Name: works_genres fk_works_genres_genre_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.works_genres
    ADD CONSTRAINT fk_works_genres_genre_id FOREIGN KEY (genre_id) REFERENCES library.genres(id) ON DELETE CASCADE;


--
-- Name: works_genres fk_works_genres_work_id; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.works_genres
    ADD CONSTRAINT fk_works_genres_work_id FOREIGN KEY (work_id) REFERENCES library.works(id) ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict x9VoRGbVnMOg77cN1bfZ0YFCQCQhBJhWWsUout0BhxupcusmWn2KbxQXhBG7HvR

