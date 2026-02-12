--
-- PostgreSQL database dump
--

\restrict t8ofxbqexday0iE8IM0ratj2WSwVO6yUUIRFCwyOq8srfCcAknhQl66eTpQ0pcp

-- Dumped from database version 18.0 (Debian 18.0-1.pgdg13+3)
-- Dumped by pg_dump version 18.0 (Debian 18.0-1.pgdg13+3)

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
    author_id integer NOT NULL,
    name_variant character varying NOT NULL,
    is_primary boolean DEFAULT false NOT NULL
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
    id integer NOT NULL,
    domain_id uuid NOT NULL,
    normalized_name character varying NOT NULL,
    first_name character varying,
    last_name character varying,
    date_created timestamp without time zone DEFAULT now() NOT NULL
);


--
-- Name: authors_id_seq; Type: SEQUENCE; Schema: library; Owner: -
--

CREATE SEQUENCE library.authors_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: authors_id_seq; Type: SEQUENCE OWNED BY; Schema: library; Owner: -
--

ALTER SEQUENCE library.authors_id_seq OWNED BY library.authors.id;


--
-- Name: books; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.books (
    id integer NOT NULL,
    domain_id uuid NOT NULL,
    title character varying NOT NULL,
    isbn13 character varying,
    isbn10 character varying,
    publisher_id integer,
    series_id integer,
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
    date_created timestamp without time zone DEFAULT now() NOT NULL
);


--
-- Name: books_authors; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.books_authors (
    book_id integer NOT NULL,
    author_id integer NOT NULL
);


--
-- Name: books_dewey_decimals; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.books_dewey_decimals (
    book_id integer NOT NULL,
    dewey_decimal_id integer NOT NULL
);


--
-- Name: books_genres; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.books_genres (
    book_id integer NOT NULL,
    genre_id integer NOT NULL
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
    id integer NOT NULL,
    domain_id uuid NOT NULL,
    name character varying NOT NULL,
    date_created timestamp without time zone DEFAULT now() NOT NULL
);


--
-- Name: series; Type: TABLE; Schema: library; Owner: -
--

CREATE TABLE library.series (
    id integer NOT NULL,
    domain_id uuid NOT NULL,
    name character varying NOT NULL,
    date_created timestamp without time zone DEFAULT now() NOT NULL
);


--
-- Name: book_details; Type: VIEW; Schema: library; Owner: -
--

CREATE VIEW library.book_details AS
 SELECT b.id,
    b.domain_id AS bookdomainid,
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
    p.domain_id AS publisherdomainid,
    p.name AS publishername,
    p.date_created AS datecreatedpublisher,
    a.id AS authorid,
    a.domain_id AS authordomainid,
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
    se.domain_id AS seriesdomainid,
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
-- Name: book_details_flat; Type: VIEW; Schema: library; Owner: -
--

CREATE VIEW library.book_details_flat AS
 SELECT b.id,
    b.domain_id AS "bookDomainId",
    b.title,
    b.isbn13,
    b.isbn10,
    b.publisher_id,
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
    p.domain_id AS "publisherDomainId",
    p.name AS "publisherName",
    p.date_created AS "dateCreatedPublisher",
    se.id AS "seriesId",
    se.domain_id AS "seriesDomainId",
    se.name AS "seriesName",
    se.date_created AS "dateCreatedSeries",
    COALESCE(g.genres_json, '[]'::jsonb) AS genres,
    COALESCE(a.authors_json, '[]'::jsonb) AS authors,
    COALESCE(dd.dewey_codes, (ARRAY[]::text[])::character varying[]) AS "deweyCodes"
   FROM (((((library.books b
     LEFT JOIN library.publishers p ON ((b.publisher_id = p.id)))
     LEFT JOIN library.series se ON ((b.series_id = se.id)))
     LEFT JOIN LATERAL ( SELECT jsonb_agg(jsonb_build_object('id', g_1.id, 'name', g_1.name, 'dateCreated', g_1.date_created) ORDER BY g_1.name) AS genres_json
           FROM (library.books_genres bg
             JOIN library.genres g_1 ON ((bg.genre_id = g_1.id)))
          WHERE (bg.book_id = b.id)) g ON (true))
     LEFT JOIN LATERAL ( SELECT jsonb_agg(jsonb_build_object('id', a_1.id, 'domainId', a_1.domain_id, 'normalizedName', a_1.normalized_name, 'firstName', a_1.first_name, 'lastName', a_1.last_name, 'dateCreated', a_1.date_created) ORDER BY a_1.normalized_name) AS authors_json
           FROM (library.books_authors ba
             JOIN library.authors a_1 ON ((ba.author_id = a_1.id)))
          WHERE (ba.book_id = b.id)) a ON (true))
     LEFT JOIN LATERAL ( SELECT array_agg(DISTINCT dd_1.code ORDER BY dd_1.code) AS dewey_codes
           FROM (library.books_dewey_decimals bdd
             JOIN library.dewey_decimals dd_1 ON ((dd_1.id = bdd.dewey_decimal_id)))
          WHERE (bdd.book_id = b.id)) dd ON (true));


--
-- Name: books_id_seq; Type: SEQUENCE; Schema: library; Owner: -
--

CREATE SEQUENCE library.books_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: books_id_seq; Type: SEQUENCE OWNED BY; Schema: library; Owner: -
--

ALTER SEQUENCE library.books_id_seq OWNED BY library.books.id;


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
-- Name: publishers_id_seq; Type: SEQUENCE; Schema: library; Owner: -
--

CREATE SEQUENCE library.publishers_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: publishers_id_seq; Type: SEQUENCE OWNED BY; Schema: library; Owner: -
--

ALTER SEQUENCE library.publishers_id_seq OWNED BY library.publishers.id;


--
-- Name: series_id_seq; Type: SEQUENCE; Schema: library; Owner: -
--

CREATE SEQUENCE library.series_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: series_id_seq; Type: SEQUENCE OWNED BY; Schema: library; Owner: -
--

ALTER SEQUENCE library.series_id_seq OWNED BY library.series.id;


--
-- Name: author_name_variants id; Type: DEFAULT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.author_name_variants ALTER COLUMN id SET DEFAULT nextval('library.author_name_variants_id_seq'::regclass);


--
-- Name: authors id; Type: DEFAULT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.authors ALTER COLUMN id SET DEFAULT nextval('library.authors_id_seq'::regclass);


--
-- Name: books id; Type: DEFAULT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books ALTER COLUMN id SET DEFAULT nextval('library.books_id_seq'::regclass);


--
-- Name: dewey_decimals id; Type: DEFAULT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.dewey_decimals ALTER COLUMN id SET DEFAULT nextval('library.dewey_decimals_id_seq'::regclass);


--
-- Name: genres id; Type: DEFAULT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.genres ALTER COLUMN id SET DEFAULT nextval('library.genres_id_seq'::regclass);


--
-- Name: publishers id; Type: DEFAULT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.publishers ALTER COLUMN id SET DEFAULT nextval('library.publishers_id_seq'::regclass);


--
-- Name: series id; Type: DEFAULT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.series ALTER COLUMN id SET DEFAULT nextval('library.series_id_seq'::regclass);


--
-- Data for Name: author_name_variants; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.author_name_variants (id, author_id, name_variant, is_primary) FROM stdin;
50	10	jamesislington	t
51	10	islingtonjames	f
52	10	ofislingtonfredericjamespost	f
53	10	fredericjamespostofislington	f
\.


--
-- Data for Name: authors; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.authors (id, domain_id, normalized_name, first_name, last_name, date_created) FROM stdin;
10	4a3787ed-5dd3-4e6f-9d36-b879ab9d86ed	james islington	James	Islington	2026-02-12 17:55:40.690028
\.


--
-- Data for Name: books; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.books (id, domain_id, title, isbn13, isbn10, publisher_id, series_id, series_number, publication_date, page_count, synopsis, language, edition, binding, image_url, image_thumbnail, msrp, is_read, height_cm, width_cm, thickness_cm, weight_g, date_created) FROM stdin;
\.


--
-- Data for Name: books_authors; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.books_authors (book_id, author_id) FROM stdin;
\.


--
-- Data for Name: books_dewey_decimals; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.books_dewey_decimals (book_id, dewey_decimal_id) FROM stdin;
\.


--
-- Data for Name: books_genres; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.books_genres (book_id, genre_id) FROM stdin;
\.


--
-- Data for Name: dewey_decimals; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.dewey_decimals (id, code, date_created) FROM stdin;
9	823.92	2026-02-12 17:55:40.690028
\.


--
-- Data for Name: genres; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.genres (id, name, date_created) FROM stdin;
47	Fiction	2026-02-12 17:55:40.690028
48	Action & Adventure	2026-02-12 17:55:40.690028
49	Fantasy	2026-02-12 17:55:40.690028
50	General	2026-02-12 17:55:40.690028
51	Epic	2026-02-12 17:55:40.690028
52	Historical	2026-02-12 17:55:40.690028
\.


--
-- Data for Name: publishers; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.publishers (id, domain_id, name, date_created) FROM stdin;
10	4925c6f9-3a14-4cfc-8a2e-abb60f8245d0	Simon And Schuster	2026-02-12 17:55:40.690028
\.


--
-- Data for Name: series; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.series (id, domain_id, name, date_created) FROM stdin;
\.


--
-- Name: author_name_variants_id_seq; Type: SEQUENCE SET; Schema: library; Owner: -
--

SELECT pg_catalog.setval('library.author_name_variants_id_seq', 53, true);


--
-- Name: authors_id_seq; Type: SEQUENCE SET; Schema: library; Owner: -
--

SELECT pg_catalog.setval('library.authors_id_seq', 10, true);


--
-- Name: books_id_seq; Type: SEQUENCE SET; Schema: library; Owner: -
--

SELECT pg_catalog.setval('library.books_id_seq', 7, true);


--
-- Name: dewey_decimals_id_seq; Type: SEQUENCE SET; Schema: library; Owner: -
--

SELECT pg_catalog.setval('library.dewey_decimals_id_seq', 9, true);


--
-- Name: genres_id_seq; Type: SEQUENCE SET; Schema: library; Owner: -
--

SELECT pg_catalog.setval('library.genres_id_seq', 52, true);


--
-- Name: publishers_id_seq; Type: SEQUENCE SET; Schema: library; Owner: -
--

SELECT pg_catalog.setval('library.publishers_id_seq', 10, true);


--
-- Name: series_id_seq; Type: SEQUENCE SET; Schema: library; Owner: -
--

SELECT pg_catalog.setval('library.series_id_seq', 1, false);


--
-- Name: author_name_variants author_name_variants_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.author_name_variants
    ADD CONSTRAINT author_name_variants_pkey PRIMARY KEY (id);


--
-- Name: authors authors_domain_id_key; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.authors
    ADD CONSTRAINT authors_domain_id_key UNIQUE (domain_id);


--
-- Name: authors authors_normalized_name_key; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.authors
    ADD CONSTRAINT authors_normalized_name_key UNIQUE (normalized_name);


--
-- Name: authors authors_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.authors
    ADD CONSTRAINT authors_pkey PRIMARY KEY (id);


--
-- Name: books_authors books_authors_pkey; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_authors
    ADD CONSTRAINT books_authors_pkey PRIMARY KEY (book_id, author_id);


--
-- Name: books books_domain_id_key; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books
    ADD CONSTRAINT books_domain_id_key UNIQUE (domain_id);


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
-- Name: books_dewey_decimals pk_books_dewey_decimals; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_dewey_decimals
    ADD CONSTRAINT pk_books_dewey_decimals PRIMARY KEY (book_id, dewey_decimal_id);


--
-- Name: publishers publishers_domain_id_key; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.publishers
    ADD CONSTRAINT publishers_domain_id_key UNIQUE (domain_id);


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
-- Name: series series_domain_id_key; Type: CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.series
    ADD CONSTRAINT series_domain_id_key UNIQUE (domain_id);


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
-- Name: idx_books_dewey_decimals_book_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_books_dewey_decimals_book_id ON library.books_dewey_decimals USING btree (book_id);


--
-- Name: idx_books_dewey_decimals_dewey_id; Type: INDEX; Schema: library; Owner: -
--

CREATE INDEX idx_books_dewey_decimals_dewey_id ON library.books_dewey_decimals USING btree (dewey_decimal_id);


--
-- Name: author_name_variants fk_author_name_variants_author; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.author_name_variants
    ADD CONSTRAINT fk_author_name_variants_author FOREIGN KEY (author_id) REFERENCES library.authors(id) ON DELETE CASCADE;


--
-- Name: books_authors fk_books_authors_author; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_authors
    ADD CONSTRAINT fk_books_authors_author FOREIGN KEY (author_id) REFERENCES library.authors(id) ON DELETE CASCADE;


--
-- Name: books_authors fk_books_authors_book; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_authors
    ADD CONSTRAINT fk_books_authors_book FOREIGN KEY (book_id) REFERENCES library.books(id) ON DELETE CASCADE;


--
-- Name: books_dewey_decimals fk_books_dewey_decimals_book; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_dewey_decimals
    ADD CONSTRAINT fk_books_dewey_decimals_book FOREIGN KEY (book_id) REFERENCES library.books(id) ON DELETE CASCADE;


--
-- Name: books_dewey_decimals fk_books_dewey_decimals_dewey; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_dewey_decimals
    ADD CONSTRAINT fk_books_dewey_decimals_dewey FOREIGN KEY (dewey_decimal_id) REFERENCES library.dewey_decimals(id) ON DELETE CASCADE;


--
-- Name: books fk_books_publisher; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books
    ADD CONSTRAINT fk_books_publisher FOREIGN KEY (publisher_id) REFERENCES library.publishers(id);


--
-- Name: books fk_books_series; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books
    ADD CONSTRAINT fk_books_series FOREIGN KEY (series_id) REFERENCES library.series(id);


--
-- Name: books_genres fk_books_subjects_book; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_genres
    ADD CONSTRAINT fk_books_subjects_book FOREIGN KEY (book_id) REFERENCES library.books(id) ON DELETE CASCADE;


--
-- Name: books_genres fk_books_subjects_subject; Type: FK CONSTRAINT; Schema: library; Owner: -
--

ALTER TABLE ONLY library.books_genres
    ADD CONSTRAINT fk_books_subjects_subject FOREIGN KEY (genre_id) REFERENCES library.genres(id) ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict t8ofxbqexday0iE8IM0ratj2WSwVO6yUUIRFCwyOq8srfCcAknhQl66eTpQ0pcp

