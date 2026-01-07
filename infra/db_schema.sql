--
-- PostgreSQL database dump
--

\restrict kA6LZwa3nxeH4xPZSNCLypeBwiXbxaWI3ma28MnsxfuRN8xaX9hK5GEYvBmp2sr

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
-- Name: pg_trgm; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS pg_trgm WITH SCHEMA public;


--
-- Name: EXTENSION pg_trgm; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON EXTENSION pg_trgm IS 'text similarity measurement and index searching based on trigrams';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: author_name_variants; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.author_name_variants (
    id integer NOT NULL,
    author_id integer NOT NULL,
    name_variant character varying(255) NOT NULL,
    is_primary boolean DEFAULT false,
    date_created timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


--
-- Name: author_name_variants_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.author_name_variants_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: author_name_variants_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.author_name_variants_id_seq OWNED BY public.author_name_variants.id;


--
-- Name: authors; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.authors (
    id integer NOT NULL,
    normalized_name character varying(255) NOT NULL,
    first_name character varying(100),
    last_name character varying(100),
    date_created timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


--
-- Name: authors_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.authors_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: authors_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.authors_id_seq OWNED BY public.authors.id;


--
-- Name: book_dimensions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.book_dimensions (
    book_id integer NOT NULL,
    height_cm numeric(4,1),
    width_cm numeric(4,1),
    thickness_cm numeric(4,1),
    weight_g numeric(4,1)
);


--
-- Name: books; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.books (
    id integer NOT NULL,
    title character varying(500) NOT NULL,
    isbn13 character varying(13),
    isbn10 character varying(10),
    publisher_id integer,
    publication_date text,
    page_count integer,
    synopsis text,
    language character varying(50),
    edition character varying(50),
    binding character varying(50),
    image_url character varying(2000),
    msrp numeric(10,2),
    is_read boolean DEFAULT false NOT NULL,
    date_created timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    image_thumbnail character varying(2000),
    series_id integer,
    series_number integer,
    CONSTRAINT valid_isbn10 CHECK (((isbn10)::text ~ '^[0-9X]{10}$'::text)),
    CONSTRAINT valid_isbn13 CHECK (((isbn13)::text ~ '^[0-9]{13}$'::text))
);


--
-- Name: books_authors; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.books_authors (
    book_id integer NOT NULL,
    author_id integer NOT NULL,
    date_created timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


--
-- Name: books_subjects; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.books_subjects (
    book_id integer NOT NULL,
    subject_id integer NOT NULL,
    date_created timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


--
-- Name: dewey_decimals; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.dewey_decimals (
    book_id integer NOT NULL,
    code character varying(50) NOT NULL
);


--
-- Name: publishers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.publishers (
    id integer NOT NULL,
    name character varying(255) NOT NULL,
    date_created timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


--
-- Name: series; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.series (
    id integer NOT NULL,
    name character varying(255) NOT NULL,
    date_created timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


--
-- Name: subjects; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.subjects (
    id integer NOT NULL,
    genre character varying(255) NOT NULL,
    date_created timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


--
-- Name: book_details; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.book_details AS
 SELECT b.id,
    b.title,
    b.isbn13,
    b.isbn10,
    b.publication_date AS publishdate,
    b.page_count AS pages,
    b.synopsis,
    b.language,
    b.edition,
    b.binding,
    b.image_url AS imageurl,
    b.msrp,
    b.is_read AS isread,
    b.date_created AS datecreated,
    b.image_thumbnail AS imagethumbnail,
    b.series_number,
    p.id AS publisherid,
    p.name,
    p.date_created AS datecreatedpublisher,
    a.id AS authorid,
    a.normalized_name AS normalizedname,
    a.first_name AS firstname,
    a.last_name AS lastname,
    a.date_created AS datecreatedauthor,
    s.id AS subjectid,
    s.genre,
    s.date_created AS datecreatedsubject,
    bd.height_cm AS heightcm,
    bd.width_cm AS widthcm,
    bd.thickness_cm AS thicknesscm,
    bd.weight_g AS weightg,
    dd.code,
    se.id AS seriesid,
    se.name AS seriesname,
    se.date_created AS datecreatedseries
   FROM ((((((((public.books b
     LEFT JOIN public.publishers p ON ((b.publisher_id = p.id)))
     LEFT JOIN public.books_authors ba ON ((b.id = ba.book_id)))
     LEFT JOIN public.authors a ON ((ba.author_id = a.id)))
     LEFT JOIN public.books_subjects bs ON ((b.id = bs.book_id)))
     LEFT JOIN public.subjects s ON ((bs.subject_id = s.id)))
     LEFT JOIN public.book_dimensions bd ON ((b.id = bd.book_id)))
     LEFT JOIN public.dewey_decimals dd ON ((b.id = dd.book_id)))
     LEFT JOIN public.series se ON ((b.series_id = se.id)));


--
-- Name: book_details_with_aggregates; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.book_details_with_aggregates AS
 SELECT b.id,
    b.title,
    b.isbn13,
    b.isbn10,
    b.publisher_id,
    b.publication_date AS publishdate,
    b.page_count AS pages,
    b.synopsis,
    b.language,
    b.edition,
    b.binding,
    b.image_url AS imageurl,
    b.msrp,
    b.is_read AS isread,
    b.date_created AS datecreated,
    b.image_thumbnail AS imagethumbnail,
    b.series_number,
    string_agg((s.genre)::text, ', '::text ORDER BY (s.genre)::text) AS subjects,
    string_agg(((s.id)::character varying)::text, ', '::text ORDER BY s.id) AS subjectids,
    string_agg(((s.date_created)::character varying)::text, ', '::text ORDER BY s.date_created) AS datecreatedsubjects,
    p.id AS publisherid,
    p.name,
    p.date_created AS datecreatedpublisher,
    string_agg(DISTINCT ((a.id)::character varying)::text, ', '::text) AS authorids,
    string_agg(DISTINCT (a.normalized_name)::text, ', '::text) AS authornames,
    string_agg(DISTINCT (a.first_name)::text, ', '::text) AS authorfirstnames,
    string_agg(DISTINCT (a.last_name)::text, ', '::text) AS authorlastnames,
    string_agg(DISTINCT ((a.date_created)::character varying)::text, ', '::text) AS datecreatedauthors,
    bd.height_cm AS heightcm,
    bd.width_cm AS widthcm,
    bd.thickness_cm AS thicknesscm,
    bd.weight_g AS weightg,
    dd.code,
    se.id AS seriesid,
    se.name AS seriesname,
    se.date_created AS datecreatedseries
   FROM ((((((((public.books b
     LEFT JOIN public.publishers p ON ((b.publisher_id = p.id)))
     LEFT JOIN public.books_authors ba ON ((b.id = ba.book_id)))
     LEFT JOIN public.authors a ON ((ba.author_id = a.id)))
     LEFT JOIN public.books_subjects bs ON ((b.id = bs.book_id)))
     LEFT JOIN public.subjects s ON ((bs.subject_id = s.id)))
     LEFT JOIN public.book_dimensions bd ON ((b.id = bd.book_id)))
     LEFT JOIN public.dewey_decimals dd ON ((b.id = dd.book_id)))
     LEFT JOIN public.series se ON ((b.series_id = se.id)))
  GROUP BY b.id, b.title, b.isbn13, b.isbn10, b.publisher_id, b.publication_date, b.page_count, b.synopsis, b.language, b.edition, b.binding, b.image_url, b.msrp, b.is_read, b.date_created, b.image_thumbnail, b.series_number, p.id, p.name, p.date_created, bd.height_cm, bd.width_cm, bd.thickness_cm, bd.weight_g, dd.code, se.id, se.name, se.date_created;


--
-- Name: books_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.books_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: books_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.books_id_seq OWNED BY public.books.id;


--
-- Name: publishers_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.publishers_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: publishers_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.publishers_id_seq OWNED BY public.publishers.id;


--
-- Name: series_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.series_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: series_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.series_id_seq OWNED BY public.series.id;


--
-- Name: subjects_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.subjects_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: subjects_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.subjects_id_seq OWNED BY public.subjects.id;


--
-- Name: author_name_variants id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.author_name_variants ALTER COLUMN id SET DEFAULT nextval('public.author_name_variants_id_seq'::regclass);


--
-- Name: authors id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.authors ALTER COLUMN id SET DEFAULT nextval('public.authors_id_seq'::regclass);


--
-- Name: books id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.books ALTER COLUMN id SET DEFAULT nextval('public.books_id_seq'::regclass);


--
-- Name: publishers id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.publishers ALTER COLUMN id SET DEFAULT nextval('public.publishers_id_seq'::regclass);


--
-- Name: series id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.series ALTER COLUMN id SET DEFAULT nextval('public.series_id_seq'::regclass);


--
-- Name: subjects id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.subjects ALTER COLUMN id SET DEFAULT nextval('public.subjects_id_seq'::regclass);


--
-- Data for Name: author_name_variants; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.author_name_variants (id, author_id, name_variant, is_primary, date_created) FROM stdin;
2487	252	joe abercrombie	t	2025-09-18 12:02:25.821847+00
2488	252	abercrombie joe abercrombie	f	2025-09-18 12:02:25.821847+00
2489	252	abercrombie joe	f	2025-09-18 12:02:25.821847+00
2490	252	joe. abercrombie	f	2025-09-18 12:02:25.821847+00
2491	252	joe abercrombie ba	f	2025-09-18 12:02:25.821847+00
2492	252	dave senior joe abercrombie	f	2025-09-18 12:02:25.821847+00
2493	253	frank herbert	t	2025-09-18 12:02:26.277722+00
2494	253	herbert-frank	f	2025-09-18 12:02:26.277722+00
2495	253	frank-herbert	f	2025-09-18 12:02:26.277722+00
2496	253	herbert frank	f	2025-09-18 12:02:26.277722+00
2497	253	frenk herbert (frank herbert)	f	2025-09-18 12:02:26.277722+00
2498	253	frank herbert thomas	f	2025-09-18 12:02:26.277722+00
2499	253	frank herbert lachacz	f	2025-09-18 12:02:26.277722+00
2500	253	frank herbert hayward	f	2025-09-18 12:02:26.277722+00
2501	253	frank herbert tubbs	f	2025-09-18 12:02:26.277722+00
2502	253	frank patrick herbert	f	2025-09-18 12:02:26.277722+00
2503	253	frank herbert richardson	f	2025-09-18 12:02:26.277722+00
2504	253	frank p. herbert	f	2025-09-18 12:02:26.277722+00
2505	253	frank herbert l872	f	2025-09-18 12:02:26.277722+00
2506	253	hayward frank herbert	f	2025-09-18 12:02:26.277722+00
2507	253	herbert m. frank	f	2025-09-18 12:02:26.277722+00
2508	253	frank herbert simonds	f	2025-09-18 12:02:26.277722+00
2509	253	frank herbert simmonds	f	2025-09-18 12:02:26.277722+00
2510	254	fonda lee	t	2025-09-18 12:02:26.893163+00
2511	254	fonda. lee	f	2025-09-18 12:02:26.893163+00
2512	255	neal shusterman	t	2025-09-18 12:02:27.464361+00
2513	255	neal. shusterman	f	2025-09-18 12:02:27.464361+00
2514	255	shusterman neal &	f	2025-09-18 12:02:27.464361+00
2515	255	shusterman neal	f	2025-09-18 12:02:27.464361+00
2516	255	neal shusterman & jarrod shusterman	f	2025-09-18 12:02:27.464361+00
2517	255	neal shusterman jarrod shusterman	f	2025-09-18 12:02:27.464361+00
2518	255	jarrod shusterman neal shusterman	f	2025-09-18 12:02:27.464361+00
2519	255	neal shusterman e jarrod shusterman	f	2025-09-18 12:02:27.464361+00
2520	255	jarrod shusterman neal shusterman (author) (author)	f	2025-09-18 12:02:27.464361+00
2521	255	michelle knowlden neal shusterman	f	2025-09-18 12:02:27.464361+00
2522	255	shusterman neal elfman eric	f	2025-09-18 12:02:27.464361+00
2523	255	eric elfman neal shusterman	f	2025-09-18 12:02:27.464361+00
2524	256	andrzej sapkowski	t	2025-09-18 12:02:28.015143+00
2525	256	andrzej. sapkowski	f	2025-09-18 12:02:28.015143+00
2526	256	sapkowski andrzej	f	2025-09-18 12:02:28.015143+00
2527	256	andrzej (author.) sapkowski	f	2025-09-18 12:02:28.015143+00
2528	256	sapkowski andrzej bere stanisaw	f	2025-09-18 12:02:28.015143+00
2529	256	david french andrzej sapkowski	f	2025-09-18 12:02:28.015143+00
2530	256	sapkowski andrzej baniewicz artur orbitowski ukasz	f	2025-09-18 12:02:28.015143+00
2531	256	andrzej sapkowski eugeniusz dębski	f	2025-09-18 12:02:28.015143+00
2532	257	john gwynne	t	2025-09-18 12:02:28.535386+00
2533	257	john gwynne-timothy	f	2025-09-18 12:02:28.535386+00
2534	257	john gwynne evans	f	2025-09-18 12:02:28.535386+00
2535	257	john gwynne (engineer.)	f	2025-09-18 12:02:28.535386+00
2536	257	john-harold-gwynne	f	2025-09-18 12:02:28.535386+00
2537	257	john gwynne (engineer )	f	2025-09-18 12:02:28.535386+00
2538	257	george john gwynne	f	2025-09-18 12:02:28.535386+00
2539	257	john a. gwynne	f	2025-09-18 12:02:28.535386+00
2540	257	john a. gwynne jr.	f	2025-09-18 12:02:28.535386+00
2541	257	david john gwynne-james	f	2025-09-18 12:02:28.535386+00
2542	257	john r. w. gwynne-timothy	f	2025-09-18 12:02:28.535386+00
2543	257	john hill~gwynne pomfrett~christiane hermann	f	2025-09-18 12:02:28.535386+00
2544	257	1924 gwynne-timothy john r. w.	f	2025-09-18 12:02:28.535386+00
2545	257	darryl gwynne jeff hutchings	f	2025-09-18 12:02:28.535386+00
2546	258	andy weir	t	2025-09-18 12:02:29.065645+00
2547	258	weir andy	f	2025-09-18 12:02:29.065645+00
2548	258	andy weir bush	f	2025-09-18 12:02:29.065645+00
2549	258	marcin ring andy weir	f	2025-09-18 12:02:29.065645+00
2550	258	radoslaw madejski andy weir	f	2025-09-18 12:02:29.065645+00
2551	258	앤디 위어 (andy weir)	f	2025-09-18 12:02:29.065645+00
2552	258	安迪·威爾 (andy weir)	f	2025-09-18 12:02:29.065645+00
2553	258	drew. goddard (screenplay based on the novel by andy weir)	f	2025-09-18 12:02:29.065645+00
2554	259	james islington	t	2025-09-18 12:02:29.644126+00
2555	259	islington james	f	2025-09-18 12:02:29.644126+00
2556	259	of islington frederic james post	f	2025-09-18 12:02:29.644126+00
2576	261	marcus aurelius	t	2026-01-07 15:36:42.984912+00
2577	261	aurelius marcus aurelius	f	2026-01-07 15:36:42.984912+00
2578	261	marcus marcus aurelius	f	2026-01-07 15:36:42.984912+00
2579	261	aurelius marcus	f	2026-01-07 15:36:42.984912+00
2580	261	marcus aurelius aurelius	f	2026-01-07 15:36:42.984912+00
2581	261	marcus-aurelius	f	2026-01-07 15:36:42.984912+00
2582	261	marcus. aurelius	f	2026-01-07 15:36:42.984912+00
2583	261	marcus-aurelius casetta	f	2026-01-07 15:36:42.984912+00
2584	261	marcus t. aurelius	f	2026-01-07 15:36:42.984912+00
2585	261	marcus aurelius quote	f	2026-01-07 15:36:42.984912+00
2586	261	hierocles marcus aurelius	f	2026-01-07 15:36:42.984912+00
2587	261	marcus aurelius. antoninius	f	2026-01-07 15:36:42.984912+00
2588	261	marcus (aurelius antoninus.)	f	2026-01-07 15:36:42.984912+00
2589	261	aurelius marcus (author)	f	2026-01-07 15:36:42.984912+00
2590	261	marcus aurelius severinus	f	2026-01-07 15:36:42.984912+00
2591	261	marcus aurelius antoninus	f	2026-01-07 15:36:42.984912+00
2592	261	emp marcus aurelius	f	2026-01-07 15:36:42.984912+00
2593	261	marcus aurelius leslie	f	2026-01-07 15:36:42.984912+00
2594	261	marcus aurelius autonimus	f	2026-01-07 15:36:42.984912+00
\.


--
-- Data for Name: authors; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.authors (id, normalized_name, first_name, last_name, date_created) FROM stdin;
252	joe abercrombie	Joe	Abercrombie	2025-09-18 12:02:25.821847+00
253	frank herbert	Frank	Herbert	2025-09-18 12:02:26.277722+00
254	fonda lee	Fonda	Lee	2025-09-18 12:02:26.893163+00
255	neal shusterman	Neal	Shusterman	2025-09-18 12:02:27.464361+00
256	andrzej sapkowski	Andrzej	Sapkowski	2025-09-18 12:02:28.015143+00
257	john gwynne	John	Gwynne	2025-09-18 12:02:28.535386+00
258	andy weir	Andy	Weir	2025-09-18 12:02:29.065645+00
259	james islington	James	Islington	2025-09-18 12:02:29.644126+00
261	marcus aurelius	Marcus	Aurelius	2026-01-07 15:36:42.984912+00
\.


--
-- Data for Name: book_dimensions; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.book_dimensions (book_id, height_cm, width_cm, thickness_cm, weight_g) FROM stdin;
338	24.3	4.5	16.3	725.7
339	23.9	5.3	16.3	997.9
340	24.5	4.1	15.9	739.0
341	24.3	6.1	16.6	975.2
342	24.6	5.7	17.0	839.1
343	2.7	13.8	21.6	290.0
344	24.0	4.4	15.4	\N
345	24.0	15.3	\N	\N
346	24.0	153.0	\N	\N
347	24.0	3.3	16.3	453.6
348	23.4	4.5	15.2	553.4
349	20.0	4.1	13.2	437.0
350	25.7	4.7	16.8	753.0
351	22.9	4.3	15.2	706.0
352	26.9	4.7	16.4	771.1
353	19.8	2.8	12.9	330.0
355	17.8	11.1	1.1	100.0
\.


--
-- Data for Name: books; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.books (id, title, isbn13, isbn10, publisher_id, publication_date, page_count, synopsis, language, edition, binding, image_url, msrp, is_read, date_created, image_thumbnail, series_id, series_number) FROM stdin;
338	A Little Hatred	9780316187169	031618716X	221	2019-09-17	480	From New York Times bestselling author Joe Abercrombie comes the first book in a new blockbuster fantasy trilogy where the age of the machine dawns, but the age of magic refuses to die. The chimneys of industry rise over Adua and the world seethes with new opportunities. But old scores run deep as ever. On the blood-soaked borders of Angland, Leo dan Brock struggles to win fame on the battlefield, and defeat the marauding armies of Stour Nightfall. He hopes for help from the crown. But King Jezal's son, the feckless Prince Orso, is a man who specializes in disappointments. Savine dan Glokta - socialite, investor, and daughter of the most feared man in the Union - plans to claw her way to the top of the slag-heap of society by any means necessary. But the slums boil over with a rage that all the money in the world cannot control. The age of the machine dawns, but the age of magic refuses to die. With the help of the mad hillwoman Isern-i-Phail, Rikke struggles to control the blessing, or the curse, of the Long Eye. Glimpsing the future is one thing, but with the guiding hand of the First of the Magi still pulling the strings, changing it will be quite another... For more from Joe Abercrombie, check out: The First Law TrilogyThe Blade ItselfBefore They Are HangedLast Argument of Kings Best Served ColdThe HeroesRed Country The Shattered Sea TrilogyHalf a KingHalf a WorldHalf a War	English	First Edition	Hardcover	https://images.isbndb.com/covers/15354693482300.jpg	27.00	f	2025-09-18 12:02:25.821847+00	http://books.google.com/books/content?id=VWJCtAEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	157	1
339	Dune	9780593099322	059309932X	222	2019-10-01	688	Frank Herbert’s epic masterpiece—a triumph of the imagination and one of the bestselling science fiction novels of all time. This deluxe hardcover edition of Dune includes: · An iconic new cover · A stamped and foiled case featuring a quote from the Litany Against Fear · Stained edges and fully illustrated endpapers · A beautifully designed poster on the interior of the jacket · A redesigned world map of Dune · An updated Introduction by Brian Herbert Set on the desert planet Arrakis, Dune is the story of Paul Atreides, heir to a noble family tasked with ruling an inhospitable world where the only thing of value is the “spice” melange, a drug capable of extending life and enhancing consciousness. Coveted across the known universe, melange is a prize worth killing for... When House Atreides is betrayed, the destruction of Paul’s family will set the boy on a journey toward a destiny greater than he could ever have imagined. And as he evolves into the mysterious man known as Muad’Dib, he will bring to fruition humankind’s most ancient and unattainable dream. A stunning blend of adventure and mysticism, environmentalism and politics, Dune won the first Nebula Award, shared the Hugo Award, and formed the basis of what is undoubtedly the grandest epic in science fiction. • DUNE: PART TWO • THE MAJOR MOTION PICTURE Directed by Denis Villeneuve, screenplay by Denis Villeneuve and Jon Spaihts, based on the novel Dune by Frank Herbert • Starring Timothée Chalamet, Zendaya, Rebecca Ferguson, Josh Brolin, Austin Butler, Florence Pugh, Dave Bautista, Christopher Walken, Stephen McKinley Henderson, Léa Seydoux, with Stellan Skarsgård, with Charlotte Rampling, and Javier Bardem	English	Deluxe Edition	Hardcover	https://images.isbndb.com/covers/3983013482399.jpg	40.00	f	2025-09-18 12:02:26.277722+00	http://books.google.com/books/content?id=-hmtDwAAQBAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	\N
340	Jade City	9780316440868	0316440868	221	2017-11-07	512	In this epic saga of magic and kungfu, four siblings battle rival clans for honor and power in an Asia-inspired fantasy metropolis. * World Fantasy Award for Best Novel, winner* Aurora Award for Best Novel, winner* Nebula Award for Best Novel, nominee* Locus Award for Best Fantasy Novel, finalist Jade is the lifeblood of the island of Kekon. It has been mined, traded, stolen, and killed for -- and for centuries, honorable Green Bone warriors like the Kaul family have used it to enhance their magical abilities and defend the island from foreign invasion. Now, the war is over and a new generation of Kauls vies for control of Kekon's bustling capital city. They care about nothing but protecting their own, cornering the jade market, and defending the districts under their protection. Ancient tradition has little place in this rapidly changing nation. When a powerful new drug emerges that lets anyone -- even foreigners -- wield jade, the simmering tension between the Kauls and the rival Ayt family erupts into open violence. The outcome of this clan war will determine the fate of all Green Bones -- from their grandest patriarch to the lowliest motorcycle runner on the streets -- and of Kekon itself. Jade City is the first novel in an epic trilogy about family, honor, and those who live and die by the ancient laws of blood and jade.	English	1	Hardcover	https://images.isbndb.com/covers/17891683482300.jpg	26.00	f	2025-09-18 12:02:26.893163+00	http://books.google.com/books/content?id=N7XYAQAACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	158	1
341	Jade Legacy	9780316440974	0316440973	221	2021-11-30	736	WINNER OF THE LOCUS AWARD FOR BEST FANTASY NOVEL, 2022 "Lee's series will stand as a pillar of epic fantasy and family drama." --Library Journal (starred review) The Kaul siblings battle rival clans for honor and control over an East Asia-inspired fantasy metropolis in Jade Legacy, the page-turning conclusion to the Green Bone Saga. Jade, the mysterious and magical substance once exclusive to the Green Bone warriors of Kekon, is now coveted throughout the world. Everyone wants access to the supernatural abilities it provides. As the struggle over the control of jade grows ever larger and more deadly, the Kaul family, and the ancient ways of the Kekonese Green Bones, will never be the same. Battered by war and tragedy, the Kauls are plagued by resentments and old wounds as their adversaries are on the ascent and their country is riven by dangerous factions and foreign interference. The clan must discern allies from enemies, set aside bloody rivalries, and make terrible sacrifices . . . but even the unbreakable bonds of blood and loyalty may not be enough to ensure the survival of the Green Bone clans and the nation they are sworn to protect. Praise for the Green Bone Saga: "Jade City has it all: a beautifully realized setting, a great cast of characters, and dramatic action scenes. What a fun, gripping read!" --Ann Leckie "An instantly absorbing tale of blood, honor, family, and magic, spiced with unexpectedly tender character beats."--NPR The Green Bone Saga Jade City Jade War Jade Legacy	English	\N	Hardcover	https://images.isbndb.com/covers/17892743482300.jpg	0.00	f	2025-09-18 12:02:27.406942+00	http://books.google.com/books/content?id=WGs8zgEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	158	3
342	Jade War	9780316440929	0316440922	221	2019-07-23	608	In Jade War, the sequel to the World Fantasy Award-winning novel Jade City, the Kaul siblings battle rival clans for honor and control over an Asia-inspired fantasy metropolis. On the island of Kekon, the Kaul family is locked in a violent feud for control of the capital city and the supply of magical jade that endows trained Green Bone warriors with supernatural powers they alone have possessed for hundreds of years. Beyond Kekon's borders, war is brewing. Powerful foreign governments and mercenary criminal kingpins alike turn their eyes on the island nation. Jade, Kekon's most prized resource, could make them rich - or give them the edge they'd need to topple their rivals. Faced with threats on all sides, the Kaul family is forced to form new and dangerous alliances, confront enemies in the darkest streets and the tallest office towers, and put honor aside in order to do whatever it takes to ensure their own survival - and that of all the Green Bones of Kekon. Jade War is the second book of the Green Bone Saga, an epic trilogy about family, honor, and those who live and die by the ancient laws of blood and jade. The Green Bone SagaJade CityJade War	English	1	Hardcover	https://images.isbndb.com/covers/17892293482300.jpg	26.00	f	2025-09-18 12:02:27.439464+00	http://books.google.com/books/content?id=E99cuwEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	158	2
343	Scythe	9781406379242	1406379247	223	2018-02-01	448	A dark, gripping and witty thriller in which the only thing humanity has control over is death. In a world where disease, war and crime have been eliminated, the only way to die is to be randomly killed ("gleaned") by professional scythes. Citra and Rowan are teenagers who have been selected to be scythes' apprentices, and despite wanting nothing to do with the vocation, they must learn the art of killing and understand the necessity of what they do. Only one of them will be chosen as a scythe's apprentice and as Citra and Rowan come up against a terrifyingly corrupt Scythedom, it becomes clear that the winning apprentice's first task will be to glean the loser. "Pretty much a perfect teen adventure novel [...] Over the years, I've heard many books touted as the successor to Hunger Games, but Scythe is the first one that I would really, truly stand behind, as it offers teens a complementary reading experience to that series rather than a duplicate one. Like Hunger Games, Scythe invites readers to both turn pages quickly but also furrow their brows over the ethical questions it asks." Maggie Stiefvater, author of The Shiver Trilogy and The Raven Cycle Scythe has been on the New York Times bestseller list for 11 weeks and is a Publishers Weekly Best Book of 2016. It's received five starred reviews in the US and is also the winner of a Michael L. Printz Honor. Universal has optioned the film rights to Scythe.	English	\N	Paperback	https://images.isbndb.com/covers/19988903482688.jpg	0.00	f	2025-09-18 12:02:27.464361+00	http://books.google.com/books/content?id=waxTswEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	159	\N
344	Season of Storms	9781399611138	1399611135	224	2023-01-05	432	Before he was Ciri's guardian, Geralt of Rivia was a legendary swordsman. Season of Storms is an adventure set in the world of the Witcher, the book series that inspired the hit Netflix show and bestselling video games. Now in a brand-new hardcover edition, this is the ultimate way to experience the epic story of the Witcher. Geralt. The witcher whose mission is to protect ordinary people from the monsters created with magic. A mutant who has the task of killing unnatural beings. He uses a magical sign, potions and the pride of every witcher - two swords, steel and silver. But what would happen if Geralt lost his weapons? Andrzej Sapkowski returns to his phenomenal world of the Witcher in a stand-alone novel where Geralt fights, travels and loves again, Dandelion sings and flies from trouble to trouble, sorcerers are scheming ... and across the whole world clouds are gathering. The season of storms is coming... Translated by David French.	English	Collector's Hardback Edition	Hardcover	https://images.isbndb.com/covers/8479443482686.jpg	0.00	f	2025-09-18 12:02:28.015143+00	http://books.google.com/books/content?id=Un5PzwEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	\N
345	The Fury of the Gods	9780356514284	0356514285	225	2024-10-22	560	Varg has overcome the trials of his past and become an accepted member of the Bloodsworn, but now he and his newfound comrades face their biggest challenge yet: slaying a dragon. Elvar is struggling to consolidate her power in Snakavik, where she faces threats from within and without. As she fights to assert her authority in readiness for the coming conflict, she faces a surely insurmountable task: reining in the ferocity of a wolf god. As Biorr and his warband make their way north, eager for blood, Gudvarr pursues a mission of his own, hoping to win Lik-Rifa's favour and further his own ambitions. All paths lead to Snakavik, where the lines are being drawn for the final battle - a titanic clash that will shake the foundations of the world, and bear witness to the true fury of the gods.	English	\N	Book	https://images.isbndb.com/covers/25424783482314.jpg	0.00	f	2025-09-18 12:02:28.535386+00	http://books.google.com/books/content?id=xiwNzgEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	\N
346	The Hunger of the Gods	9780356514246	0356514242	225	2022-04-12	656	The Hunger of the Gods continues John Gwynne's acclaimed Norse-inspired epic fantasy series, packed with myth, magic and bloody vengeance Lik-Rifa, the dragon god of legend, has been freed from her eternal prison. Now she plots a new age of blood and conquest. As Orka continues the hunt for her missing son, the Bloodsworn sweep south in a desperate race to save one of their own - and Varg takes the first steps on the path of vengeance. Elvar has sworn to fulfil her blood oath and rescue a prisoner from the clutches of Lik-Rifa and her dragonborn followers, but first she must persuade the Battle-Grim to follow her. Yet even the might of the Bloodsworn and Battle-Grim cannot stand alone against a dragon god. Their hope lies within the mad writings of a chained god. A book of forbidden magic with the power to raise the wolf god Ulfrir from the dead . . .and bring about a battle that will shake the foundations of the earth. Praise for The Bloodsworn series: 'A masterfully crafted, brutally compelling Norse-inspired epic' Anthony Ryan 'Visceral, heart-breaking and unputdownable' Jay Kristoff 'A satisfying and riveting read. The well-realised characters move against a backdrop of a world stunning in its immensity. It's everything I've come to expect from a John Gwynne book' Robin Hobb.	English	\N	Book	https://images.isbndb.com/covers/25424403482314.jpg	0.00	f	2025-09-18 12:02:29.052021+00	http://books.google.com/books/content?id=FG4MzgEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	\N
347	The Martian	9780804139021	0804139024	226	2014-02-11	384	#1 NEW YORK TIMES BESTSELLER • NOW A MAJOR MOTION PICTURE A mission to Mars. A freak accident. One man’s struggle to survive. From the author of Project Hail Mary comes “a hugely entertaining novel that reads like a rocket ship afire” (Chicago Tribune). “Brilliant . . . a celebration of human ingenuity [and] the purest example of real-science sci-fi for many years . . . utterly compelling.”—The Wall Street Journal Six days ago, astronaut Mark Watney became one of the first people to walk on Mars. Now, he’s sure he’ll be the first person to die there. After a dust storm nearly kills him and forces his crew to evacuate while thinking him dead, Mark finds himself stranded and completely alone with no way to even signal Earth that he’s alive—and even if he could get word out, his supplies would be gone long before a rescue could arrive. Chances are, though, he won’t have time to starve to death. The damaged machinery, unforgiving environment, or plain-old “human error” are much more likely to kill him first. But Mark isn’t ready to give up yet. Drawing on his ingenuity, his engineering skills—and a relentless, dogged refusal to quit—he steadfastly confronts one seemingly insurmountable obstacle after the next. Will his resourcefulness be enough to overcome the impossible odds against him? NAMED ONE OF PASTE’S BEST NOVELS OF THE DECADE “A hugely entertaining novel [that] reads like a rocket ship afire . . . Weir has fashioned in Mark Watney one of the most appealing, funny, and resourceful characters in recent fiction.”—Chicago Tribune “As gripping as they come . . . You’ll be rooting for Watney the whole way, groaning at every setback and laughing at his pitchblack humor. Utterly nail-biting and memorable.”—Financial Times	English	A Novel	Hardcover	https://images.isbndb.com/covers/7945753482474.jpg	27.00	f	2025-09-18 12:02:29.065645+00	http://books.google.com/books/content?id=oGGMDQAAQBAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	\N
348	The Shadow of the Gods	9780316539883	0316539880	221	2021-05-04	528	Set in a Norse-inspired world, and packed with myth, magic, and vengeance, The Shadow of the Gods begins John Gwynne's New York Times bestselling Bloodsworn Saga. The story continues in book two, The Hunger of the Gods, available now! "A masterfully crafted, brutally compelling Norse-inspired epic." --Anthony Ryan, author of The Pariah THE GREATEST SAGAS ARE WRITTEN IN BLOOD. A century has passed since the gods fought and drove themselves to extinction. Now only their bones remain, promising great power to those brave enough to seek them out. As whispers of war echo across the land of Vigrid, fate follows in the footsteps of three warriors: a huntress on a dangerous quest, a noblewoman pursuing battle fame, and a thrall seeking vengeance among the mercenaries known as the Bloodsworn. All three will shape the fate of the world as it once more falls under the shadow of the gods. LOOK OUT FOR THE DELUXE LIMITED HARDCOVER EDITION OF THE SHADOW OF THE GODS―featuring stenciled sprayed edges, a new introduction from the author, a foil stamped case, and custom endpapers. Only available on a limited first print run while supplies last. For more from John Gwynne, check out: The Bloodsworn The Shadow of the Gods The Hunger of the Gods The Fury of the Gods Of Blood and Bone A Time of Dread A Time of Blood A Time of Courage The Faithful and the Fallen Malice Valor Ruin Wrath	English	\N	Paperback	https://images.isbndb.com/covers/18881833482300.jpg	0.00	f	2025-09-18 12:02:29.568815+00	http://books.google.com/books/content?id=iDHYzQEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	160	1
349	The Toll	9781406385670	1406385670	223	2019-11-07	528	In the finale to the Arc of a Scythe trilogy, dictators, prophets, and tensions rise. In a world that's conquered death, will humanity finally be torn asunder by the immortal beings it created? Citra and Rowan have disappeared. Endura is gone. It seems like nothing stands between Scythe Goddard and absolute dominion over the world scythedom. With the silence of the Thunderhead and the reverberations of the Great Resonance still shaking the earth to its core, the question remains: Is there anyone left who can stop him? The answer lies in the Tone, the Toll, and the Thunder.--adapted from publisher's description.	English	\N	Paperback	https://images.isbndb.com/covers/20053183482688.jpg	0.00	f	2025-09-18 12:02:29.597536+00	http://books.google.com/books/content?id=ZpQpwQEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	159	\N
350	The Trouble with Peace	9780316187183	0316187186	221	2020-09-15	512	A fragile peace gives way to conspiracy, betrayal, and rebellion in this sequel to the New York Times bestselling A Little Hatred from epic fantasy master Joe Abercrombie. Peace is just another kind of battlefield . . . Savine dan Glokta, once Adua's most powerful investor, finds her judgement, fortune and reputation in tatters. But she still has all her ambitions, and no scruple will be permitted to stand in her way. For heroes like Leo dan Brock and Stour Nightfall, only happy with swords drawn, peace is an ordeal to end as soon as possible. But grievances must be nursed, power seized, and allies gathered first, while Rikke must master the power of the Long Eye . . . before it kills her. Unrest worms into every layer of society. The Breakers still lurk in the shadows, plotting to free the common man from his shackles, while noblemen bicker for their own advantage. Orso struggles to find a safe path through the maze of knives that is politics, only for his enemies, and his debts, to multiply. The old ways are swept aside, and the old leaders with them, but those who would seize the reins of power will find no alliance, no friendship, and no peace lasts forever. For more from Joe Abercrombie, check out: The Age of Madness A Little Hatred The Trouble With Peace The First Law Trilogy The Blade Itself Before They Are Hanged Last Argument of Kings Best Served Cold The Heroes Red Country The Shattered Sea Trilogy Half a King Half a World Half a War "A master of his craft." --Forbes "No one writes with the seismic scope or primal intensity of Joe Abercrombie." --Pierce Brown	English	\N	Hardcover	https://images.isbndb.com/covers/15354833482300.jpg	0.00	f	2025-09-18 12:02:29.631629+00	http://books.google.com/books/content?id=RGkrzQEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	157	2
351	The Will of the Many	9781982141172	1982141174	227	2023-05-23	640	At the elite Catenan Academy, a young fugitive uncovers layered mysteries and world-changing secrets in this new fantasy series by internationally bestselling author of The Licanius Trilogy, James Islington. AUDI. VIDE. TACE. The Catenan Republic—the Hierarchy—may rule the world now, but they do not know everything. I tell them my name is Vis Telimus. I tell them I was orphaned after a tragic accident three years ago, and that good fortune alone has led to my acceptance into their most prestigious school. I tell them that once I graduate, I will gladly join the rest of civilised society in allowing my strength, my drive and my focus—what they call Will—to be leeched away and added to the power of those above me, as millions already do. As all must eventually do. I tell them that I belong, and they believe me. But the truth is that I have been sent to the Academy to find answers. To solve a murder. To search for an ancient weapon. To uncover secrets that may tear the Republic apart. And that I will never, ever cede my Will to the empire that executed my family. To survive, though, I will still have to rise through the Academy’s ranks. I will have to smile, and make friends, and pretend to be one of them and win. Because if I cannot, then those who want to control me, who know my real name, will no longer have any use for me. And if the Hierarchy finds out who I truly am, they will kill me.	English	\N	Hardcover	https://images.isbndb.com/covers/20021253482893.jpg	0.00	f	2025-09-18 12:02:29.644126+00	http://books.google.com/books/content?id=53u7EAAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api	161	1
352	The Wisdom of Crowds	9780316187244	0316187240	221	2021-09-14	528	The New York Times bestselling finale to the Age of Madness trilogyfinds the world in an unstoppable revolution where heroes have nothing left to lose as darkness and destruction overtake everything. Chaos. Fury. Destruction. The Great Change is upon us . . . Some say that to change the world you must first burn it down. Now that belief will be tested in the crucible of revolution: the Breakers and Burners have seized the levers of power, the smoke of riots has replaced the smog of industry, and all must submit to the wisdom of crowds. With nothing left to lose, Citizen Brock is determined to become a new hero for the new age, while Citizeness Savine must turn her talents from profit to survival before she can claw her way to redemption. Orso will find that when the world is turned upside down, no one is lower than a monarch. And in the bloody North, Rikke and her fragile Protectorate are running out of allies . . . while Black Calder gathers his forces and plots his vengeance. The banks have fallen, the sun of the Union has been torn down, and in the darkness behind the scenes, the threads of the Weaver's ruthless plan are slowly being drawn together . . . "No one writes with the seismic scope or primal intensity of Joe Abercrombie." --Pierce Brown For more from Joe Abercrombie, check out: The Age of Madness A Little Hatred The Trouble With Peace The Wisdom of Crowds The First Law Trilogy The Blade Itself Before They Are Hanged Last Argument of Kings Best Served Cold The Heroes Red Country The Shattered Sea Trilogy Half a King Half a World Half a War	English	\N	Hardcover	https://images.isbndb.com/covers/15355443482300.jpg	0.00	f	2025-09-18 12:02:30.148139+00	http://books.google.com/books/content?id=M94lzgEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	157	3
353	Thunderhead	9781406379532	1406379530	223	2018-05-01	515	The dark and thrilling sequel to Scythe, the New York Times sci-fi bestseller. The stakes are high in this chilling sci-fi thriller, in which professional scythes control who dies. Everything else is out of human control, managed by the Thunderhead. It's a perfect system - until it isn't. It's been a year since Rowan went off-grid. Hunted by the Scythedom, he has become an urban legend, a vigilante snuffing out corrupt scythes in a trial by fire. Citra, meanwhile, is forging her path as Scythe Anastasia, gleaning with compassion. However, conflict within the Scythedom is growing by the day, and when Citra's life is threatened, it becomes clear that there is a truly terrifying plot afoot. The Thunderhead observes everything, and it does not like what it sees. Will it intervene? Or will it simply watch as this perfect world begins to unravel? The sequel to New York Times bestseller Scythe, which was a Publishers Weekly Best Book of 2016. Scythe has received five starred reviews in the US and is also the winner of a Michael L. Printz Honor. Universal has optioned the film rights. Thunderhead has already received three starred reviews in the US, with Booklist praising Neal Shusterman for achieving "that most difficult of feats: a sequel that surpasses its predecessor". "[...] a genuinely original and chilling dystopian thriller posing some big, thought-provoking questions" - The Bookseller on Scythe.The dark and thrilling sequel to Scythe, the New York Times sci-fi bestseller. The stakes are high in this chilling sci-fi thriller, in which professional scythes control who dies. Everything else is out of human control, managed by the Thunderhead. It's a perfect system - until it isn't. It's been a year since Rowan went off-grid. Hunted by the Scythedom, he has become an urban legend, a vigilante snuffing out corrupt scythes in a trial by fire. Citra, meanwhile, is forging her path as Scythe Anastasia, gleaning with compassion. However, conflict within the Scythedom is growing by the day, and when Citra's life is threatened, it becomes clear that there is a truly terrifying plot afoot. The Thunderhead observes everything, and it does not like what it sees. Will it intervene? Or will it simply watch as this perfect world begins to unravel? The sequel to New York Times bestseller Scythe, which was a Publishers Weekly Best Book of 2016. Scythe has received five starred reviews in the US and is also the winner of a Michael L. Printz Honor. Universal has optioned the film rights. Thunderhead has already received three starred reviews in the US, with Booklist praising Neal Shusterman for achieving "that most difficult of feats: a sequel that surpasses its predecessor". "[...] a genuinely original and chilling dystopian thriller posing some big, thought-provoking questions" - The Bookseller on Scythe.	English	\N	Paperback	https://images.isbndb.com/covers/19991803482688.jpg	0.00	f	2025-09-18 12:02:30.163144+00	http://books.google.com/books/content?id=ul0-tAEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	\N
355	Meditations	9780008425029	0008425027	229	2020-09-02	176	<p>HarperCollins is proud to present its incredible range of best-loved, essential classics.</p> <br> <br> <p>Tranquility is nothing else than the good ordering of the mind</p> <p>The extraordinary writings of Marcus Aurelius (CE 121-180), the only Roman emperor to have also been a Stoic philosopher, have for centuries been praised for their wisdom, insight and guidance by leaders and great thinkers alike. Never intended for publication, Meditations are the personal notes of a warrior-king studying his unique position of power, surrounded by loss and human fragility.</p> <p>Boldly confronting many of our greatest questions, Meditations wrestles with the complexities of human nature, rationality and moral virtue. This timeless and significant text will provide both inspiration and comfort to the twenty-first-century reader.</p>	English	\N	Paperback	https://images.isbndb.com/covers/27170193482190.jpg	0.00	f	2026-01-07 16:01:56.449872+00	http://books.google.com/books/content?id=4WeYzQEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	\N
\.


--
-- Data for Name: books_authors; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.books_authors (book_id, author_id, date_created) FROM stdin;
338	252	2025-09-18 12:02:25.821847+00
339	253	2025-09-18 12:02:26.277722+00
340	254	2025-09-18 12:02:26.893163+00
341	254	2025-09-18 12:02:27.406942+00
342	254	2025-09-18 12:02:27.439464+00
343	255	2025-09-18 12:02:27.464361+00
344	256	2025-09-18 12:02:28.015143+00
345	257	2025-09-18 12:02:28.535386+00
346	257	2025-09-18 12:02:29.052021+00
347	258	2025-09-18 12:02:29.065645+00
348	257	2025-09-18 12:02:29.568815+00
349	255	2025-09-18 12:02:29.597536+00
350	252	2025-09-18 12:02:29.631629+00
351	259	2025-09-18 12:02:29.644126+00
352	252	2025-09-18 12:02:30.148139+00
353	255	2025-09-18 12:02:30.163144+00
355	261	2026-01-07 16:01:56.449872+00
\.


--
-- Data for Name: books_subjects; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.books_subjects (book_id, subject_id, date_created) FROM stdin;
338	979	2025-09-18 12:02:25.821847+00
338	980	2025-09-18 12:02:25.821847+00
338	981	2025-09-18 12:02:25.821847+00
338	982	2025-09-18 12:02:25.821847+00
338	983	2025-09-18 12:02:25.821847+00
338	984	2025-09-18 12:02:25.821847+00
339	979	2025-09-18 12:02:26.277722+00
339	985	2025-09-18 12:02:26.277722+00
339	986	2025-09-18 12:02:26.277722+00
339	987	2025-09-18 12:02:26.277722+00
339	980	2025-09-18 12:02:26.277722+00
339	981	2025-09-18 12:02:26.277722+00
340	979	2025-09-18 12:02:26.893163+00
340	980	2025-09-18 12:02:26.893163+00
340	981	2025-09-18 12:02:26.893163+00
340	984	2025-09-18 12:02:26.893163+00
340	988	2025-09-18 12:02:26.893163+00
340	989	2025-09-18 12:02:26.893163+00
340	990	2025-09-18 12:02:26.893163+00
341	991	2025-09-18 12:02:27.406942+00
341	984	2025-09-18 12:02:27.406942+00
341	980	2025-09-18 12:02:27.406942+00
341	992	2025-09-18 12:02:27.406942+00
341	981	2025-09-18 12:02:27.406942+00
341	988	2025-09-18 12:02:27.406942+00
341	989	2025-09-18 12:02:27.406942+00
341	993	2025-09-18 12:02:27.406942+00
341	994	2025-09-18 12:02:27.406942+00
342	979	2025-09-18 12:02:27.439464+00
342	980	2025-09-18 12:02:27.439464+00
342	981	2025-09-18 12:02:27.439464+00
342	984	2025-09-18 12:02:27.439464+00
342	988	2025-09-18 12:02:27.439464+00
342	989	2025-09-18 12:02:27.439464+00
342	990	2025-09-18 12:02:27.439464+00
343	995	2025-09-18 12:02:27.464361+00
343	985	2025-09-18 12:02:27.464361+00
343	996	2025-09-18 12:02:27.464361+00
344	979	2025-09-18 12:02:28.015143+00
344	980	2025-09-18 12:02:28.015143+00
344	992	2025-09-18 12:02:28.015143+00
344	981	2025-09-18 12:02:28.015143+00
344	997	2025-09-18 12:02:28.015143+00
345	979	2025-09-18 12:02:28.535386+00
345	980	2025-09-18 12:02:28.535386+00
345	981	2025-09-18 12:02:28.535386+00
347	979	2025-09-18 12:02:29.065645+00
347	985	2025-09-18 12:02:29.065645+00
347	984	2025-09-18 12:02:29.065645+00
347	998	2025-09-18 12:02:29.065645+00
347	988	2025-09-18 12:02:29.065645+00
347	999	2025-09-18 12:02:29.065645+00
348	979	2025-09-18 12:02:29.568815+00
348	980	2025-09-18 12:02:29.568815+00
348	981	2025-09-18 12:02:29.568815+00
348	984	2025-09-18 12:02:29.568815+00
348	1000	2025-09-18 12:02:29.568815+00
349	1001	2025-09-18 12:02:29.597536+00
349	1002	2025-09-18 12:02:29.597536+00
349	1003	2025-09-18 12:02:29.597536+00
349	1004	2025-09-18 12:02:29.597536+00
349	985	2025-09-18 12:02:29.597536+00
349	992	2025-09-18 12:02:29.597536+00
349	995	2025-09-18 12:02:29.597536+00
349	1005	2025-09-18 12:02:29.597536+00
349	1006	2025-09-18 12:02:29.597536+00
349	1007	2025-09-18 12:02:29.597536+00
350	979	2025-09-18 12:02:29.631629+00
350	980	2025-09-18 12:02:29.631629+00
350	981	2025-09-18 12:02:29.631629+00
350	983	2025-09-18 12:02:29.631629+00
350	982	2025-09-18 12:02:29.631629+00
351	979	2025-09-18 12:02:29.644126+00
351	984	2025-09-18 12:02:29.644126+00
351	980	2025-09-18 12:02:29.644126+00
351	992	2025-09-18 12:02:29.644126+00
351	981	2025-09-18 12:02:29.644126+00
351	983	2025-09-18 12:02:29.644126+00
352	979	2025-09-18 12:02:30.148139+00
352	980	2025-09-18 12:02:30.148139+00
352	981	2025-09-18 12:02:30.148139+00
352	983	2025-09-18 12:02:30.148139+00
352	982	2025-09-18 12:02:30.148139+00
352	984	2025-09-18 12:02:30.148139+00
352	1008	2025-09-18 12:02:30.148139+00
353	1009	2025-09-18 12:02:30.163144+00
355	1023	2026-01-07 16:01:56.449872+00
355	1024	2026-01-07 16:01:56.449872+00
355	1025	2026-01-07 16:01:56.449872+00
355	1026	2026-01-07 16:01:56.449872+00
355	1027	2026-01-07 16:01:56.449872+00
355	1028	2026-01-07 16:01:56.449872+00
355	1029	2026-01-07 16:01:56.449872+00
355	1030	2026-01-07 16:01:56.449872+00
355	992	2026-01-07 16:01:56.449872+00
355	1031	2026-01-07 16:01:56.449872+00
355	1032	2026-01-07 16:01:56.449872+00
355	1033	2026-01-07 16:01:56.449872+00
355	1034	2026-01-07 16:01:56.449872+00
355	1035	2026-01-07 16:01:56.449872+00
\.


--
-- Data for Name: dewey_decimals; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.dewey_decimals (book_id, code) FROM stdin;
338	823.92
339	813.54
340	813.6
341	813.6
342	813.6
347	813.6
348	823.92
350	823.92
351	823.92
352	823.92
355	188
\.


--
-- Data for Name: publishers; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.publishers (id, name, date_created) FROM stdin;
221	Orbit	2025-09-18 12:02:25.821847+00
222	Ace	2025-09-18 12:02:26.277722+00
223	Walker Books	2025-09-18 12:02:27.464361+00
224	Gollancz	2025-09-18 12:02:28.015143+00
225	Little Brown Book Group Limited	2025-09-18 12:02:28.535386+00
226	Ballantine Books	2025-09-18 12:02:29.065645+00
227	Gallery Saga Press	2025-09-18 12:02:29.644126+00
229	Harpercollins Publishers Limited	2026-01-07 15:36:42.984912+00
\.


--
-- Data for Name: series; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.series (id, name, date_created) FROM stdin;
157	The Age of Madness	2025-09-18 12:02:25.821847+00
158	The Green Bone Saga	2025-09-18 12:02:26.893163+00
159	Arc of a Scythe	2025-09-18 12:02:27.464361+00
160	The Bloodsworn Trilogy	2025-09-18 12:02:29.568815+00
161	Hierarchy	2025-09-18 12:02:29.644126+00
\.


--
-- Data for Name: subjects; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.subjects (id, genre, date_created) FROM stdin;
979	Fiction	2025-09-18 12:02:25.821847+00
980	Fantasy	2025-09-18 12:02:25.821847+00
981	Epic	2025-09-18 12:02:25.821847+00
982	Dark Fantasy	2025-09-18 12:02:25.821847+00
983	Historical	2025-09-18 12:02:25.821847+00
984	Action & Adventure	2025-09-18 12:02:25.821847+00
985	Science Fiction	2025-09-18 12:02:26.277722+00
986	Space Opera	2025-09-18 12:02:26.277722+00
987	Classics	2025-09-18 12:02:26.277722+00
988	Thrillers	2025-09-18 12:02:26.893163+00
989	Crime	2025-09-18 12:02:26.893163+00
990	Sagas	2025-09-18 12:02:26.893163+00
991	FICTION	2025-09-18 12:02:27.406942+00
992	General	2025-09-18 12:02:27.406942+00
993	Family Life	2025-09-18 12:02:27.406942+00
994	Siblings	2025-09-18 12:02:27.406942+00
995	Young Adult Fiction	2025-09-18 12:02:27.464361+00
996	Apocalyptic & Post-Apocalyptic	2025-09-18 12:02:27.464361+00
997	Media Tie-In	2025-09-18 12:02:28.015143+00
998	Hard Science Fiction	2025-09-18 12:02:29.065645+00
999	Suspense	2025-09-18 12:02:29.065645+00
1000	Dragons & Mythical Creatures	2025-09-18 12:02:29.568815+00
1001	Juvenile Fiction	2025-09-18 12:02:29.597536+00
1002	Mysteries & Detective Stories	2025-09-18 12:02:29.597536+00
1003	Social Themes	2025-09-18 12:02:29.597536+00
1004	Death, Grief, Bereavement	2025-09-18 12:02:29.597536+00
1005	Emotions & Feelings	2025-09-18 12:02:29.597536+00
1006	Violence	2025-09-18 12:02:29.597536+00
1007	Thrillers & Suspense	2025-09-18 12:02:29.597536+00
1008	Military	2025-09-18 12:02:30.148139+00
1009	Subjects	2025-09-18 12:02:30.163144+00
1023	Biography & Autobiography	2026-01-07 15:36:42.984912+00
1024	Presidents & Heads Of State	2026-01-07 15:36:42.984912+00
1025	History	2026-01-07 15:36:42.984912+00
1026	Ancient	2026-01-07 15:36:42.984912+00
1027	Rome	2026-01-07 15:36:42.984912+00
1028	Literary Collections	2026-01-07 15:36:42.984912+00
1029	Ancient & Classical	2026-01-07 15:36:42.984912+00
1030	Body, Mind & Spirit	2026-01-07 15:36:42.984912+00
1031	Philosophy	2026-01-07 15:36:42.984912+00
1032	History & Surveys	2026-01-07 15:36:42.984912+00
1033	Ethics & Moral Philosophy	2026-01-07 15:36:42.984912+00
1034	Mind & Body	2026-01-07 15:36:42.984912+00
1035	Essays	2026-01-07 15:36:42.984912+00
\.


--
-- Name: author_name_variants_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.author_name_variants_id_seq', 2594, true);


--
-- Name: authors_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.authors_id_seq', 261, true);


--
-- Name: books_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.books_id_seq', 355, true);


--
-- Name: publishers_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.publishers_id_seq', 229, true);


--
-- Name: series_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.series_id_seq', 161, true);


--
-- Name: subjects_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.subjects_id_seq', 1035, true);


--
-- Name: author_name_variants author_name_variants_author_id_name_variant_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.author_name_variants
    ADD CONSTRAINT author_name_variants_author_id_name_variant_key UNIQUE (author_id, name_variant);


--
-- Name: author_name_variants author_name_variants_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.author_name_variants
    ADD CONSTRAINT author_name_variants_pkey PRIMARY KEY (id);


--
-- Name: authors authors_normalized_name_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.authors
    ADD CONSTRAINT authors_normalized_name_key UNIQUE (normalized_name);


--
-- Name: authors authors_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.authors
    ADD CONSTRAINT authors_pkey PRIMARY KEY (id);


--
-- Name: book_dimensions book_dimensions_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.book_dimensions
    ADD CONSTRAINT book_dimensions_pkey PRIMARY KEY (book_id);


--
-- Name: books_authors books_authors_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.books_authors
    ADD CONSTRAINT books_authors_pkey PRIMARY KEY (book_id, author_id);


--
-- Name: books books_isbn10_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.books
    ADD CONSTRAINT books_isbn10_key UNIQUE (isbn10);


--
-- Name: books books_isbn13_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.books
    ADD CONSTRAINT books_isbn13_key UNIQUE (isbn13);


--
-- Name: books books_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.books
    ADD CONSTRAINT books_pkey PRIMARY KEY (id);


--
-- Name: books_subjects books_subjects_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.books_subjects
    ADD CONSTRAINT books_subjects_pkey PRIMARY KEY (book_id, subject_id);


--
-- Name: dewey_decimals dewey_decimals_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.dewey_decimals
    ADD CONSTRAINT dewey_decimals_pkey PRIMARY KEY (book_id, code);


--
-- Name: publishers publishers_name_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.publishers
    ADD CONSTRAINT publishers_name_key UNIQUE (name);


--
-- Name: publishers publishers_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.publishers
    ADD CONSTRAINT publishers_pkey PRIMARY KEY (id);


--
-- Name: series series_name_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.series
    ADD CONSTRAINT series_name_key UNIQUE (name);


--
-- Name: series series_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.series
    ADD CONSTRAINT series_pkey PRIMARY KEY (id);


--
-- Name: subjects subjects_name_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.subjects
    ADD CONSTRAINT subjects_name_key UNIQUE (genre);


--
-- Name: subjects subjects_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.subjects
    ADD CONSTRAINT subjects_pkey PRIMARY KEY (id);


--
-- Name: idx_author_name_variants; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_author_name_variants ON public.author_name_variants USING btree (name_variant);


--
-- Name: idx_books_authors_author_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_books_authors_author_id ON public.books_authors USING btree (author_id);


--
-- Name: idx_books_authors_book_author; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_books_authors_book_author ON public.books_authors USING btree (book_id, author_id);


--
-- Name: idx_books_authors_book_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_books_authors_book_id ON public.books_authors USING btree (book_id);


--
-- Name: idx_books_isbn10; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_books_isbn10 ON public.books USING btree (isbn10);


--
-- Name: idx_books_isbn13; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_books_isbn13 ON public.books USING btree (isbn13);


--
-- Name: idx_books_publisher_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_books_publisher_id ON public.books USING btree (publisher_id);


--
-- Name: idx_books_series_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_books_series_id ON public.books USING btree (series_id);


--
-- Name: idx_books_subjects_book_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_books_subjects_book_id ON public.books_subjects USING btree (book_id);


--
-- Name: idx_books_subjects_subject_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_books_subjects_subject_id ON public.books_subjects USING btree (subject_id);


--
-- Name: idx_books_title; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_books_title ON public.books USING btree (title);


--
-- Name: idx_dewey_decimals_code; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_dewey_decimals_code ON public.dewey_decimals USING btree (code);


--
-- Name: idx_publishers_name; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_publishers_name ON public.publishers USING btree (name);


--
-- Name: idx_series_name; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_series_name ON public.series USING btree (name);


--
-- Name: idx_subjects_name; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_subjects_name ON public.subjects USING btree (genre);


--
-- Name: author_name_variants author_name_variants_author_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.author_name_variants
    ADD CONSTRAINT author_name_variants_author_id_fkey FOREIGN KEY (author_id) REFERENCES public.authors(id) ON DELETE CASCADE;


--
-- Name: book_dimensions book_dimensions_book_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.book_dimensions
    ADD CONSTRAINT book_dimensions_book_id_fkey FOREIGN KEY (book_id) REFERENCES public.books(id) ON DELETE CASCADE;


--
-- Name: books_authors books_authors_author_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.books_authors
    ADD CONSTRAINT books_authors_author_id_fkey FOREIGN KEY (author_id) REFERENCES public.authors(id) ON DELETE CASCADE;


--
-- Name: books_authors books_authors_book_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.books_authors
    ADD CONSTRAINT books_authors_book_id_fkey FOREIGN KEY (book_id) REFERENCES public.books(id) ON DELETE CASCADE;


--
-- Name: books books_publisher_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.books
    ADD CONSTRAINT books_publisher_id_fkey FOREIGN KEY (publisher_id) REFERENCES public.publishers(id) ON DELETE SET NULL;


--
-- Name: books books_series_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.books
    ADD CONSTRAINT books_series_id_fkey FOREIGN KEY (series_id) REFERENCES public.series(id);


--
-- Name: books_subjects books_subjects_book_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.books_subjects
    ADD CONSTRAINT books_subjects_book_id_fkey FOREIGN KEY (book_id) REFERENCES public.books(id) ON DELETE CASCADE;


--
-- Name: books_subjects books_subjects_subject_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.books_subjects
    ADD CONSTRAINT books_subjects_subject_id_fkey FOREIGN KEY (subject_id) REFERENCES public.subjects(id) ON DELETE CASCADE;


--
-- Name: dewey_decimals dewey_decimals_book_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.dewey_decimals
    ADD CONSTRAINT dewey_decimals_book_id_fkey FOREIGN KEY (book_id) REFERENCES public.books(id) ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict kA6LZwa3nxeH4xPZSNCLypeBwiXbxaWI3ma28MnsxfuRN8xaX9hK5GEYvBmp2sr

