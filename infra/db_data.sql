--
-- PostgreSQL database dump
--

\restrict YShvlQm9ZRafKaI0Xbecimz8iSPmCWMPMeyYwdcaLPk2NKVDflsbrNQi9AbEhWi

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
-- Data for Name: author_name_variants; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.author_name_variants (id, name_variant, is_primary, author_id) FROM stdin;
50	jamesislington	t	4a3787ed-5dd3-4e6f-9d36-b879ab9d86ed
51	islingtonjames	f	4a3787ed-5dd3-4e6f-9d36-b879ab9d86ed
52	ofislingtonfredericjamespost	f	4a3787ed-5dd3-4e6f-9d36-b879ab9d86ed
53	fredericjamespostofislington	f	4a3787ed-5dd3-4e6f-9d36-b879ab9d86ed
54	frank herbert	t	355185c3-90ca-4a93-be79-2b1d3a2d4633
55	herbertfrank	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
56	frankherbert	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
57	herbert frank	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
58	frank. herbert	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
59	frenk herbert	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
60	frank herbert slade	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
61	herbert frank milne	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
62	frank barnes herbert	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
63	frank herbert sweet	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
64	herbert frank fisher	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
65	frank herbert cunningham	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
66	frank herbert chase	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
67	frank herbert norcross	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
68	frank herbert. lachacz	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
69	frank herbert matthews	f	355185c3-90ca-4a93-be79-2b1d3a2d4633
70	andrzej sapkowski	t	ad17b5b5-37e7-42f4-8ead-469087228da2
71	andrzej. sapkowski	f	ad17b5b5-37e7-42f4-8ead-469087228da2
72	sapkowski andrzej	f	ad17b5b5-37e7-42f4-8ead-469087228da2
73	sapkowski andrzej bere stanisaw	f	ad17b5b5-37e7-42f4-8ead-469087228da2
74	david french andrzej sapkowski	f	ad17b5b5-37e7-42f4-8ead-469087228da2
75	sapkowski andrzej baniewicz artur orbitowski ukasz	f	ad17b5b5-37e7-42f4-8ead-469087228da2
76	andrzej sapkowski eugeniusz dębski	f	ad17b5b5-37e7-42f4-8ead-469087228da2
77	andy weir	t	fd16400e-97aa-4e1b-98be-45bda89a211e
78	weir andy	f	fd16400e-97aa-4e1b-98be-45bda89a211e
79	andy weir bush	f	fd16400e-97aa-4e1b-98be-45bda89a211e
80	radoslaw madejski andy weir	f	fd16400e-97aa-4e1b-98be-45bda89a211e
81	marcin ring andy weir	f	fd16400e-97aa-4e1b-98be-45bda89a211e
82	эндзі ўір andy weir	f	fd16400e-97aa-4e1b-98be-45bda89a211e
83	앤디 위어	f	fd16400e-97aa-4e1b-98be-45bda89a211e
84	安迪威爾	f	fd16400e-97aa-4e1b-98be-45bda89a211e
85	安迪威爾andy weir	f	fd16400e-97aa-4e1b-98be-45bda89a211e
86	安迪威爾 andy weir	f	fd16400e-97aa-4e1b-98be-45bda89a211e
87	drew. goddard	f	fd16400e-97aa-4e1b-98be-45bda89a211e
88	fonda lee	t	09657f26-612b-4799-b240-36ec2d0c2a9d
89	fonda. lee	f	09657f26-612b-4799-b240-36ec2d0c2a9d
90	lee lee fonda shannon	f	09657f26-612b-4799-b240-36ec2d0c2a9d
91	john gwynne	t	03dde1d8-76c8-41c4-aab2-03ea66659272
92	john. gwynne	f	03dde1d8-76c8-41c4-aab2-03ea66659272
93	johnharoldgwynne	f	03dde1d8-76c8-41c4-aab2-03ea66659272
94	john gwynne brockis	f	03dde1d8-76c8-41c4-aab2-03ea66659272
95	john gwynnetimothy	f	03dde1d8-76c8-41c4-aab2-03ea66659272
96	john gwynne evans	f	03dde1d8-76c8-41c4-aab2-03ea66659272
97	john stelling gwynne	f	03dde1d8-76c8-41c4-aab2-03ea66659272
98	george john gwynne	f	03dde1d8-76c8-41c4-aab2-03ea66659272
99	john a. gwynne	f	03dde1d8-76c8-41c4-aab2-03ea66659272
100	john a. gwynne jr.	f	03dde1d8-76c8-41c4-aab2-03ea66659272
101	john gwynne prosser ii	f	03dde1d8-76c8-41c4-aab2-03ea66659272
102	david john gwynnejames	f	03dde1d8-76c8-41c4-aab2-03ea66659272
103	john fl gwynne	f	03dde1d8-76c8-41c4-aab2-03ea66659272
104	john r. w. gwynnetimothy	f	03dde1d8-76c8-41c4-aab2-03ea66659272
105	gwynnetimothy john r. w.	f	03dde1d8-76c8-41c4-aab2-03ea66659272
106	john hillgwynne pomfrettchristiane hermann	f	03dde1d8-76c8-41c4-aab2-03ea66659272
107	neal shusterman	t	7d319db0-e498-4b10-98ca-120378080229
108	shusterman neal	f	7d319db0-e498-4b10-98ca-120378080229
109	neal. shusterman	f	7d319db0-e498-4b10-98ca-120378080229
110	neal shusterman jarrod shusterman	f	7d319db0-e498-4b10-98ca-120378080229
111	jarrod shusterman neal shusterman	f	7d319db0-e498-4b10-98ca-120378080229
112	neal shusterman e jarrod shusterman	f	7d319db0-e498-4b10-98ca-120378080229
113	michelle knowlden neal shusterman	f	7d319db0-e498-4b10-98ca-120378080229
114	닐 셔스터먼	f	7d319db0-e498-4b10-98ca-120378080229
115	shusterman neal elfman eric	f	7d319db0-e498-4b10-98ca-120378080229
116	eric elfman neal shusterman	f	7d319db0-e498-4b10-98ca-120378080229
117	joe abercrombie	t	9f537022-d4bb-46dd-b20f-a0420c6b7086
118	abercrombie joe abercrombie	f	9f537022-d4bb-46dd-b20f-a0420c6b7086
119	abercrombie joe	f	9f537022-d4bb-46dd-b20f-a0420c6b7086
120	joe. abercrombie	f	9f537022-d4bb-46dd-b20f-a0420c6b7086
121	joe abercrombie ba	f	9f537022-d4bb-46dd-b20f-a0420c6b7086
122	dave senior joe abercrombie	f	9f537022-d4bb-46dd-b20f-a0420c6b7086
123	andreas steno	t	d992239a-4857-47db-9ae3-cae5d8d6edc4
124	jesper ravnborg	t	47a5bb2c-f5b0-4aa2-8100-b0dfd0a18191
\.


--
-- Data for Name: authors; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.authors (id, normalized_name, first_name, last_name, date_created) FROM stdin;
4a3787ed-5dd3-4e6f-9d36-b879ab9d86ed	james islington	James	Islington	2026-02-12 17:55:40.690028
355185c3-90ca-4a93-be79-2b1d3a2d4633	frank herbert	Frank	Herbert	2026-02-12 19:10:55.531748
ad17b5b5-37e7-42f4-8ead-469087228da2	andrzej sapkowski	Andrzej	Sapkowski	2026-02-12 19:12:10.973471
fd16400e-97aa-4e1b-98be-45bda89a211e	andy weir	Andy	Weir	2026-02-12 19:13:06.631366
09657f26-612b-4799-b240-36ec2d0c2a9d	fonda lee	Fonda	Lee	2026-02-12 19:13:21.821302
03dde1d8-76c8-41c4-aab2-03ea66659272	john gwynne	John	Gwynne	2026-02-12 19:13:29.993659
7d319db0-e498-4b10-98ca-120378080229	neal shusterman	Neal	Shusterman	2026-02-12 19:13:54.414107
9f537022-d4bb-46dd-b20f-a0420c6b7086	joe abercrombie	Joe	Abercrombie	2026-02-12 19:14:35.157782
d992239a-4857-47db-9ae3-cae5d8d6edc4	andreas steno	Andreas	Steno	2026-03-27 12:40:15.29416
47a5bb2c-f5b0-4aa2-8100-b0dfd0a18191	jesper ravnborg	Jesper	Ravnborg	2026-03-27 12:40:15.29416
\.


--
-- Data for Name: books; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.books (id, title, isbn13, isbn10, series_number, publication_date, page_count, synopsis, language, edition, binding, image_url, image_thumbnail, msrp, is_read, height_cm, width_cm, thickness_cm, weight_g, date_created, publisher_id, series_id) FROM stdin;
bd52a9ea-a983-463b-92cb-5a56c78cd359	Dune	9780593099322	059309932X	\N	2019-10-01	688	Frank Herbert’s epic masterpiece—a triumph of the imagination and one of the bestselling science fiction novels of all time. This deluxe hardcover edition of Dune includes: · An iconic new cover · A stamped and foiled case featuring a quote from the Litany Against Fear · Stained edges and fully illustrated endpapers · A beautifully designed poster on the interior of the jacket · A redesigned world map of Dune · An updated Introduction by Brian Herbert Set on the desert planet Arrakis, Dune is the story of Paul Atreides, heir to a noble family tasked with ruling an inhospitable world where the only thing of value is the “spice” melange, a drug capable of extending life and enhancing consciousness. Coveted across the known universe, melange is a prize worth killing for... When House Atreides is betrayed, the destruction of Paul’s family will set the boy on a journey toward a destiny greater than he could ever have imagined. And as he evolves into the mysterious man known as Muad’Dib, he will bring to fruition humankind’s most ancient and unattainable dream. A stunning blend of adventure and mysticism, environmentalism and politics, Dune won the first Nebula Award, shared the Hugo Award, and formed the basis of what is undoubtedly the grandest epic in science fiction. • DUNE: PART TWO • THE MAJOR MOTION PICTURE Directed by Denis Villeneuve, screenplay by Denis Villeneuve and Jon Spaihts, based on the novel Dune by Frank Herbert • Starring Timothée Chalamet, Zendaya, Rebecca Ferguson, Josh Brolin, Austin Butler, Florence Pugh, Dave Bautista, Christopher Walken, Stephen McKinley Henderson, Léa Seydoux, with Stellan Skarsgård, with Charlotte Rampling, and Javier Bardem	English	Deluxe Edition	Hardback	https://images.isbndb.com/covers/3983013482399.jpg	http://books.google.com/books/content?id=-hmtDwAAQBAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	40.0	f	23.9	16.3	5.3	998	2026-02-12 19:10:55.531748	ff86f049-1578-4c53-8ae5-215df9c48d61	\N
2485bf0e-3207-4543-8df1-5131764719b9	The Martian	9780804139021	0804139024	\N	2014-02-11	384	<b>#1 <i>NEW YORK TIMES </i>BESTSELLER • NOW A MAJOR MOTION PICTURE</b><br><br><b>A mission to Mars. A freak accident. One man’s struggle to survive. From the author of <i>Project Hail Mary</i> comes “a hugely entertaining novel that reads like a rocket ship afire” (<i>Chicago Tribune</i>).</b><br><br><b>“Brilliant . . . a celebration of human ingenuity [and] the purest example of real-science sci-fi for many years . . . utterly compelling.”—<i>The Wall Street Journal</i></b><br><br>Six days ago, astronaut Mark Watney became one of the first people to walk on Mars. <br><br>Now, he’s sure he’ll be the first person to die there.<br><br>After a dust storm nearly kills him and forces his crew to evacuate while thinking him dead, Mark finds himself stranded and completely alone with no way to even signal Earth that he’s alive—and even if he could get word out, his supplies would be gone long before a rescue could arrive. <br><br>Chances are, though, he won’t have time to starve to death. The damaged machinery, unforgiving environment, or plain-old “human error” are much more likely to kill him first. <br><br>But Mark isn’t ready to give up yet. Drawing on his ingenuity, his engineering skills—and a relentless, dogged refusal to quit—he steadfastly confronts one seemingly insurmountable obstacle after the next. Will his resourcefulness be enough to overcome the impossible odds against him?<br><b> <br><b>NAMED ONE OF <i>PASTE</i>’S BEST NOVELS OF THE DECADE</b></b><br><br>“As gripping as they come . . . You’ll be rooting for Watney the whole way, groaning at every setback and laughing at his pitchblack humor. Utterly nail-biting and memorable.”<b>—<i>Financial Times</i></b>	English	A Novel	Hardback	https://images.isbndb.com/covers/7945753482474.jpg	http://books.google.com/books/content?id=oGGMDQAAQBAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	27.0	f	24.0	16.3	3.3	454	2026-02-12 19:13:06.631366	12c62220-76c3-47d6-af2b-edd4dd077469	\N
2cd52dc7-3cc5-4952-b780-da7ee9615e89	Jade Legacy	9780316440974	0316440973	\N	2021	736	<p><b>WINNER OF THE LOCUS AWARD FOR BEST FANTASY NOVEL, 2022 <br> <br> <br> <br> "Lee's series will stand as a pillar of epic fantasy and family drama."</b> --<i>Library Journal</i> (starred review)<br> <br> <br> <br> <b>The Kaul siblings battle rival clans for honor and control over an East Asia-inspired fantasy metropolis in <i>Jade Legacy</i>, the page-turning conclusion to the Green Bone Saga.</b></p> <p>Jade, the mysterious and magical substance once exclusive to the Green Bone warriors of Kekon, is now coveted throughout the world. Everyone wants access to the supernatural abilities it provides. As the struggle over the control of jade grows ever larger and more deadly, the Kaul family, and the ancient ways of the Kekonese Green Bones, will never be the same.</p> <p>Battered by war and tragedy, the Kauls are plagued by resentments and old wounds as their adversaries are on the ascent and their country is riven by dangerous factions and foreign interference. The clan must discern allies from enemies, set aside bloody rivalries, and make terrible sacrifices . . . but even the unbreakable bonds of blood and loyalty may not be enough to ensure the survival of the Green Bone clans and the nation they are sworn to protect.</p> <p><b>Praise for the Green Bone Saga:</b></p> <p>"<i>Jade City</i> has it all: a beautifully realized setting, a great cast of characters, and dramatic action scenes. What a fun, gripping read!" --Ann Leckie</p> <p>"An instantly absorbing tale of blood, honor, family, and magic, spiced with unexpectedly tender character beats."--<i>NPR</i></p> <p><br> <br> <b>The Green Bone Saga</b></p> <p><i>Jade City<br> <br> Jade War<br> <br> Jade Legacy</i></p>	English	\N	Hardback	https://images.isbndb.com/covers/17892743482300.jpg	http://books.google.com/books/content?id=WGs8zgEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	f	24.3	16.6	6.1	975	2026-02-12 19:13:46.290132	1840fcf6-ffc7-4115-8844-cbe7f67989ea	\N
6113d050-8454-44c3-9e8c-adb30e410b61	The Trouble with Peace	9780316187183	0316187186	\N	2020	512	<b>A fragile peace gives way to conspiracy, betrayal, and rebellion in this sequel to the <i>New York Times</i> bestselling <i>A Little Hatred </i>from epic fantasy master Joe Abercrombie.</b> <p><i>Peace is just another kind of battlefield . . .</i> <p>Savine dan Glokta, once Adua's most powerful investor, finds her judgement, fortune and reputation in tatters. But she still has all her ambitions, and no scruple will be permitted to stand in her way. <p>For heroes like Leo dan Brock and Stour Nightfall, only happy with swords drawn, peace is an ordeal to end as soon as possible. But grievances must be nursed, power seized, and allies gathered first, while Rikke must master the power of the Long Eye . . . before it kills her. <p>Unrest worms into every layer of society. The Breakers still lurk in the shadows, plotting to free the common man from his shackles, while noblemen bicker for their own advantage. Orso struggles to find a safe path through the maze of knives that is politics, only for his enemies, and his debts, to multiply. <p>The old ways are swept aside, and the old leaders with them, but those who would seize the reins of power will find no alliance, no friendship, and no peace lasts forever. <p>For more from Joe Abercrombie, check out: <p><b>The Age of Madness</b> <br><i>A Little Hatred</i> <br><i>The Trouble With Peace</i> <p><b>The First Law Trilogy</b> <br><i>The Blade Itself</i> <br><i>Before They Are Hanged</i> <br><i>Last Argument of Kings</i> <br><i>Best Served Cold</i> <br><i>The Heroes</i> <br><i>Red Country</i> <p><b>The Shattered Sea Trilogy</b> <br><i>Half a King</i> <br><i>Half a World</i> <br><i>Half a War</i> <p><b>"A master of his craft." --<i>Forbes</i></b> <p><b>"No one writes with the seismic scope or primal intensity of Joe Abercrombie." --Pierce Brown</b>	English	\N	Hardback	https://images.isbndb.com/covers/15354833482300.jpg	http://books.google.com/books/content?id=RGkrzQEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	f	25.7	16.8	4.7	753	2026-02-12 19:14:42.613739	1840fcf6-ffc7-4115-8844-cbe7f67989ea	\N
c6f3085f-5e9c-4115-8ec1-176cb7c69401	The Will of the Many	9781982141172	1982141174	\N	2023-05-23	630	<b>Featuring a reversible book cover with the original hardcover art design.</b><br><br><b>At the elite Catenan Academy, a young fugitive uncovers layered mysteries and world-changing secrets in this new fantasy series by internationally bestselling author of The Licanius Trilogy, James Islington.</b><br><br><b><i>AUDI. VIDE. TACE.</i></b><br> <br>The Catenan Republic—the Hierarchy—may rule the world now, but they do not know everything.<br> <br>I tell them my name is Vis Telimus. I tell them I was orphaned after a tragic accident three years ago, and that good fortune alone has led to my acceptance into their most prestigious school. I tell them that once I graduate, I will gladly join the rest of civilised society in allowing my strength, my drive and my focus—what they call Will—to be leeched away and added to the power of those above me, as millions already do. As all must eventually do.<br> <br>I tell them that I belong, and they believe me.<br> <br>But the truth is that I have been sent to the Academy to find answers. To solve a murder. To search for an ancient weapon. To uncover secrets that may tear the Republic apart.<br> <br>And that I will never, ever cede my Will to the empire that executed my family.<br> <br>To survive, though, I will still have to rise through the Academy’s ranks. I will have to smile, and make friends, and pretend to be one of them and <i>win</i>. Because if I cannot, then those who want to control me, who know my real name, will no longer have any use for me.<br> <br>And if the Hierarchy finds out who I truly am, they will kill me.	English	\N	Hardback	https://images.isbndb.com/covers/20021253482893.jpg	http://books.google.com/books/content?id=53u7EAAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api	\N	f	22.9	15.2	4.3	706	2026-02-12 19:10:31.840438	4925c6f9-3a14-4cfc-8a2e-abb60f8245d0	\N
ecf063e1-8a29-4460-94fe-27ce3e69d81a	Season of Storms	9781399611138	1399611135	\N	2023-01-05	421	<p><b>Before he was Ciri's guardian, Geralt of Rivia was a legendary swordsman. <i>Season of Storms</i> is an adventure set in the world of the Witcher, the book series that inspired the hit Netflix show and bestselling video games.</b> <b>Now in a brand-new hardcover edition, this is the ultimate way to experience the epic story of the Witcher.</b><br> <br> Geralt. The witcher whose mission is to protect ordinary people from the monsters created with magic. A mutant who has the task of killing unnatural beings. He uses a magical sign, potions and the pride of every witcher - two swords, steel and silver.<br> <br> But what would happen if Geralt lost his weapons?<br> <br> Andrzej Sapkowski returns to his phenomenal world of the Witcher in a stand-alone novel where Geralt fights, travels and loves again, Dandelion sings and flies from trouble to trouble, sorcerers are scheming ... and across the whole world clouds are gathering.<br> <br> The season of storms is coming...<br> <br> Translated by David French.</p>	English	Collector's Hardback Edition	Hardback	https://images.isbndb.com/covers/8479443482686.jpg	http://books.google.com/books/content?id=Un5PzwEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	f	24	15.4	4.4	\N	2026-02-12 19:12:10.973471	be97dc00-a0d0-4d37-a0e6-cde563299f8c	\N
321ab573-02da-458d-be58-ed4de7a34e37	The Wisdom of Crowds	9780316187244	0316187240	\N	2021	528	<p><b>The <i>New York Times</i> bestselling finale to the Age of Madness trilogyfinds the world in an unstoppable revolution where heroes have nothing left to lose as darkness and destruction overtake everything.</b></p> Chaos. Fury. Destruction.<br> <br> <br> <br> The Great Change is upon us . . .<br> <br> <br> <br> Some say that to change the world you must first burn it down. Now that belief will be tested in the crucible of revolution: the Breakers and Burners have seized the levers of power, the smoke of riots has replaced the smog of industry, and all must submit to the wisdom of crowds.<br> <br> <br> <br> With nothing left to lose, Citizen Brock is determined to become a new hero for the new age, while Citizeness Savine must turn her talents from profit to survival before she can claw her way to redemption. Orso will find that when the world is turned upside down, no one is lower than a monarch. And in the bloody North, Rikke and her fragile Protectorate are running out of allies . . . while Black Calder gathers his forces and plots his vengeance.<br> <br> <br> <br> The banks have fallen, the sun of the Union has been torn down, and in the darkness behind the scenes, the threads of the Weaver's ruthless plan are slowly being drawn together . . .<br> <br> <br> <br> <b>"No one writes with the seismic scope or primal intensity of Joe Abercrombie." --Pierce Brown</b><br> <br> <br> <br> For more from Joe Abercrombie, check out:<br> <br> <br> <br> <i><b>The Age of Madness</b><br> <br> A Little Hatred<br> <br> The Trouble With Peace<br> <br> The Wisdom of Crowds<br> <br> <br> <br> <b>The First Law Trilogy</b><br> <br> The Blade Itself<br> <br> Before They Are Hanged<br> <br> Last Argument of Kings<br> <br> <br> <br> Best Served Cold<br> <br> The Heroes<br> <br> Red Country<br> <br> <br> <br> <b>The Shattered Sea Trilogy</b><br> <br> Half a King<br> <br> Half a World<br> <br> Half a War</i>	English	\N	Hardback	https://images.isbndb.com/covers/15355443482300.jpg	http://books.google.com/books/content?id=M94lzgEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	f	26.9	16.4	4.7	771	2026-02-12 19:14:50.127283	1840fcf6-ffc7-4115-8844-cbe7f67989ea	\N
b4c92622-6c9d-4c51-9409-3edc05420ccf	A Little Hatred	9780316187169	031618716X	\N	2019-09-17	480	<b>From <i>New York Times </i>bestselling author Joe Abercrombie comes the first book in a new blockbuster fantasy trilogy where the age of the machine dawns, but the age of magic refuses to die.</b><br>The chimneys of industry rise over Adua and the world seethes with new opportunities. But old scores run deep as ever.<br>On the blood-soaked borders of Angland, Leo dan Brock struggles to win fame on the battlefield, and defeat the marauding armies of Stour Nightfall. He hopes for help from the crown. But King Jezal's son, the feckless Prince Orso, is a man who specializes in disappointments.<br>Savine dan Glokta - socialite, investor, and daughter of the most feared man in the Union - plans to claw her way to the top of the slag-heap of society by any means necessary. But the slums boil over with a rage that all the money in the world cannot control.<br>The age of the machine dawns, but the age of magic refuses to die. With the help of the mad hillwoman Isern-i-Phail, Rikke struggles to control the blessing, or the curse, of the Long Eye. Glimpsing the future is one thing, but with the guiding hand of the First of the Magi still pulling the strings, changing it will be quite another...<b><br></b>For more from Joe Abercrombie, check out:<b><br></b><b>The First Law Trilogy</b><i>The Blade Itself</i><i>Before They Are Hanged</i><i>Last Argument of Kings</i><i><br></i><i>Best Served Cold</i><i>The Heroes</i><i>Red Country</i><b><i><br></i></b><b>The Shattered Sea Trilogy</b><i>Half a King</i><i>Half a World</i><i>Half a War</i>	English	First Edition	Hardback	https://images.isbndb.com/covers/15354693482300.jpg	http://books.google.com/books/content?id=VWJCtAEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	27.0	f	24.3	16.3	4.5	726	2026-02-12 19:14:35.157782	1840fcf6-ffc7-4115-8844-cbe7f67989ea	\N
bea3d501-8e93-4faf-8938-73c166193104	The Shadow of the Gods	9780316539883	0316539880	\N	2021	514	<b>LOOK OUT FOR THE <i>DELUXE LIMITED HARDCOVER EDITIONS </i>OF <i>THE BLOODSWORN SAGA </i></b>― featuring stenciled sprayed edges, a new introduction from the author, a foil stamped case, and custom endpapers. Only available on a limited first print run while supplies last.<br> <br> <br> <br> <b>Set in a Norse-inspired world, and packed with myth, magic, and vengeance, <i>The Shadow of the Gods </i>begins an epic fantasy saga from bestselling author John Gwynne. </b><br> <br> <br> <br> A century has passed since the gods fought and drove themselves to extinction. Now only their bones remain, promising great power to those brave enough to seek them out.<br> <br> <br> <br> As whispers of war echo across the land of Vigrid, fate follows in the footsteps of three warriors: a huntress on a dangerous quest, a noblewoman pursuing battle fame, and a thrall seeking vengeance among the mercenaries known as the Bloodsworn.<br> <br> <br> <br> All three will shape the fate of the world as it once more falls under the shadow of the gods.<br> <br> <br> <br> "A masterfully crafted, brutally compelling Norse-inspired epic." --Anthony Ryan <i>The Greatest Sagas Are Written in Blood</i><br> <br> <br> <br> For more from John Gwynne, check out: <br> <br> <br> <br> The Bloodsworn Trilogy<br> <br> <i>The Shadow of the Gods <br> <br> The Hunger of the Gods <br> <br> The Fury of the Gods </i><br> <br> <br> <br> Of Blood and Bone<br> <br> <i>A Time of Dread <br> <br> A Time of Blood <br> <br> A Time of Courage </i><br> <br> <br> <br> The Faithful and the Fallen<br> <br> <i>Malice <br> <br> Valor <br> <br> Ruin <br> <br> Wrath</i>	English	\N	Paperback	https://images.isbndb.com/covers/18881833482300.jpg	http://books.google.com/books/content?id=iDHYzQEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	f	23.4	15.2	4.5	553	2026-02-12 19:14:17.641953	1840fcf6-ffc7-4115-8844-cbe7f67989ea	\N
5054f51d-1d2b-4a80-89c8-d846a099ecaf	Jade War	9780316440929	0316440922	\N	2019-07-23	608	<b>In <i>Jade War</i>, the sequel to the World Fantasy Award-winning novel <i>Jade City</i>, the Kaul siblings battle rival clans for honor and control over an Asia-inspired fantasy metropolis.</b><br>On the island of Kekon, the Kaul family is locked in a violent feud for control of the capital city and the supply of magical jade that endows trained Green Bone warriors with supernatural powers they alone have possessed for hundreds of years. <br>Beyond Kekon's borders, war is brewing. Powerful foreign governments and mercenary criminal kingpins alike turn their eyes on the island nation. Jade, Kekon's most prized resource, could make them rich - or give them the edge they'd need to topple their rivals. <br>Faced with threats on all sides, the Kaul family is forced to form new and dangerous alliances, confront enemies in the darkest streets and the tallest office towers, and put honor aside in order to do whatever it takes to ensure their own survival - and that of all the Green Bones of Kekon. <br><b><i>Jade War</i> is the second book of the Green Bone Saga, an epic trilogy about family, honor, and those who live and die by the ancient laws of blood and jade.</b><b><br></b><b><br></b><b>The Green Bone Saga</b><i>Jade City</i><i>Jade War</i>	English	1	Hardback	https://images.isbndb.com/covers/17892293482300.jpg	http://books.google.com/books/content?id=E99cuwEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	26.0	f	24.6	17.0	5.7	839	2026-02-12 19:13:37.348701	1840fcf6-ffc7-4115-8844-cbe7f67989ea	\N
620afc88-54b0-4d12-ac77-1ae3f4eabfa1	Jade City	9780316440868	0316440868	\N	2017-11-07	512	<b>In this epic saga of magic and kungfu, four siblings battle rival clans for honor and power in an Asia-inspired fantasy metropolis.</b><br>* World Fantasy Award for Best Novel, winner* Aurora Award for Best Novel, winner* Nebula Award for Best Novel, nominee* Locus Award for Best Fantasy Novel, finalist<br>Jade is the lifeblood of the island of Kekon. It has been mined, traded, stolen, and killed for -- and for centuries, honorable Green Bone warriors like the Kaul family have used it to enhance their magical abilities and defend the island from foreign invasion. <br>Now, the war is over and a new generation of Kauls vies for control of Kekon's bustling capital city. They care about nothing but protecting their own, cornering the jade market, and defending the districts under their protection. Ancient tradition has little place in this rapidly changing nation.<br>When a powerful new drug emerges that lets anyone -- even foreigners -- wield jade, the simmering tension between the Kauls and the rival Ayt family erupts into open violence. The outcome of this clan war will determine the fate of all Green Bones -- from their grandest patriarch to the lowliest motorcycle runner on the streets -- and of Kekon itself. <b><br></b><b><i>Jade City</i> is the first novel in an epic trilogy about family, honor, and those who live and die by the ancient laws of blood and jade.</b>	English	1	Hardback	https://images.isbndb.com/covers/17891683482300.jpg	http://books.google.com/books/content?id=N7XYAQAACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	26.0	f	24.5	15.9	4.1	739	2026-02-12 19:13:21.821302	1840fcf6-ffc7-4115-8844-cbe7f67989ea	\N
dae78a50-a220-4167-9873-a3b9d8edf130	The Hunger of the Gods	9780356514246	0356514242	\N	2022	636	The Hunger of the Gods continues John Gwynne's acclaimed Norse-inspired epic fantasy series, packed with myth, magic and bloody vengeance Lik-Rifa, the dragon god of legend, has been freed from her eternal prison. Now she plots a new age of blood and conquest. As Orka continues the hunt for her missing son, the Bloodsworn sweep south in a desperate race to save one of their own - and Varg takes the first steps on the path of vengeance. Elvar has sworn to fulfil her blood oath and rescue a prisoner from the clutches of Lik-Rifa and her dragonborn followers, but first she must persuade the Battle-Grim to follow her. Yet even the might of the Bloodsworn and Battle-Grim cannot stand alone against a dragon god. Their hope lies within the mad writings of a chained god. A book of forbidden magic with the power to raise the wolf god Ulfrir from the dead . . .and bring about a battle that will shake the foundations of the earth. Praise for The Bloodsworn series: 'A masterfully crafted, brutally compelling Norse-inspired epic' Anthony Ryan 'Visceral, heart-breaking and unputdownable' Jay Kristoff 'A satisfying and riveting read. The well-realised characters move against a backdrop of a world stunning in its immensity. It's everything I've come to expect from a John Gwynne book' Robin Hobb.	English	\N	Unknown	https://images.isbndb.com/covers/25424403482314.jpg	http://books.google.com/books/content?id=FG4MzgEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	f	24.0	15.3	\N	\N	2026-02-12 19:13:29.993659	7b252a8a-83af-47ae-84da-200ce945ed86	\N
2c041218-157a-4ad9-bc95-4c2d1edc2701	The Toll	9781406385670	1406385670	\N	2019	528	In the finale to the Arc of a Scythe trilogy, dictators, prophets, and tensions rise. In a world that's conquered death, will humanity finally be torn asunder by the immortal beings it created? Citra and Rowan have disappeared. Endura is gone. It seems like nothing stands between Scythe Goddard and absolute dominion over the world scythedom. With the silence of the Thunderhead and the reverberations of the Great Resonance still shaking the earth to its core, the question remains: Is there anyone left who can stop him? The answer lies in the Tone, the Toll, and the Thunder.--adapted from publisher's description.	English	\N	Paperback	https://images.isbndb.com/covers/20053183482688.jpg	http://books.google.com/books/content?id=ZpQpwQEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	f	20	13.2	4.1	437	2026-02-12 19:14:10.277806	3edf4b2b-809d-4385-bd04-72e9bfb2506b	\N
dd1badc8-82f3-4ad1-b331-a4e8ce87a7b9	Thunderhead	9781406379532	1406379530	\N	2018-05	504	The dark and thrilling sequel to Scythe, the New York Times sci-fi bestseller. The stakes are high in this chilling sci-fi thriller, in which professional scythes control who dies. Everything else is out of human control, managed by the Thunderhead. It's a perfect system - until it isn't. It's been a year since Rowan went off-grid. Hunted by the Scythedom, he has become an urban legend, a vigilante snuffing out corrupt scythes in a trial by fire. Citra, meanwhile, is forging her path as Scythe Anastasia, gleaning with compassion. However, conflict within the Scythedom is growing by the day, and when Citra's life is threatened, it becomes clear that there is a truly terrifying plot afoot. The Thunderhead observes everything, and it does not like what it sees. Will it intervene? Or will it simply watch as this perfect world begins to unravel? The sequel to New York Times bestseller Scythe, which was a Publishers Weekly Best Book of 2016. Scythe has received five starred reviews in the US and is also the winner of a Michael L. Printz Honor. Universal has optioned the film rights. Thunderhead has already received three starred reviews in the US, with Booklist praising Neal Shusterman for achieving "that most difficult of feats: a sequel that surpasses its predecessor". "[...] a genuinely original and chilling dystopian thriller posing some big, thought-provoking questions" - The Bookseller on Scythe.The dark and thrilling sequel to Scythe, the New York Times sci-fi bestseller. The stakes are high in this chilling sci-fi thriller, in which professional scythes control who dies. Everything else is out of human control, managed by the Thunderhead. It's a perfect system - until it isn't. It's been a year since Rowan went off-grid. Hunted by the Scythedom, he has become an urban legend, a vigilante snuffing out corrupt scythes in a trial by fire. Citra, meanwhile, is forging her path as Scythe Anastasia, gleaning with compassion. However, conflict within the Scythedom is growing by the day, and when Citra's life is threatened, it becomes clear that there is a truly terrifying plot afoot. The Thunderhead observes everything, and it does not like what it sees. Will it intervene? Or will it simply watch as this perfect world begins to unravel? The sequel to New York Times bestseller Scythe, which was a Publishers Weekly Best Book of 2016. Scythe has received five starred reviews in the US and is also the winner of a Michael L. Printz Honor. Universal has optioned the film rights. Thunderhead has already received three starred reviews in the US, with Booklist praising Neal Shusterman for achieving "that most difficult of feats: a sequel that surpasses its predecessor". "[...] a genuinely original and chilling dystopian thriller posing some big, thought-provoking questions" - The Bookseller on Scythe.	English	\N	Paperback	https://images.isbndb.com/covers/19991803482688.jpg	http://books.google.com/books/content?id=ul0-tAEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	f	19.8	12.9	2.8	330	2026-02-12 19:13:54.414107	3edf4b2b-809d-4385-bd04-72e9bfb2506b	\N
67f8d7a7-fb8e-4458-a790-29eefdae860a	Scythe	9781406379242	1406379247	\N	2018	433	A dark, gripping and witty thriller in which the only thing humanity has control over is death. In a world where disease, war and crime have been eliminated, the only way to die is to be randomly killed ("gleaned") by professional scythes. Citra and Rowan are teenagers who have been selected to be scythes' apprentices, and despite wanting nothing to do with the vocation, they must learn the art of killing and understand the necessity of what they do. Only one of them will be chosen as a scythe's apprentice and as Citra and Rowan come up against a terrifyingly corrupt Scythedom, it becomes clear that the winning apprentice's first task will be to glean the loser. "Pretty much a perfect teen adventure novel [...] Over the years, I've heard many books touted as the successor to Hunger Games, but Scythe is the first one that I would really, truly stand behind, as it offers teens a complementary reading experience to that series rather than a duplicate one. Like Hunger Games, Scythe invites readers to both turn pages quickly but also furrow their brows over the ethical questions it asks." Maggie Stiefvater, author of The Shiver Trilogy and The Raven Cycle Scythe has been on the New York Times bestseller list for 11 weeks and is a Publishers Weekly Best Book of 2016. It's received five starred reviews in the US and is also the winner of a Michael L. Printz Honor. Universal has optioned the film rights to Scythe.	English	\N	Paperback	https://images.isbndb.com/covers/19988903482688.jpg	http://books.google.com/books/content?id=waxTswEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	f	21.6	13.8	2.7	290	2026-02-12 19:14:01.64641	cbe01d90-be61-4742-8cd8-7437abe4a0b6	\N
27cc4939-e6a5-4565-b315-5b0b15b074c1	The Fury of the Gods	9780356514284	0356514285	\N	2024-10-22	560	<i><b>The Fury of the Gods</b></i> <b>is the earth-shattering final book in John Gwynne's bestselling Norse-inspired epic fantasy series, packed with myth, magic and bloody vengeance</b>THE FINAL BATTLE FOR THE FATE OF VIGRI APPROACHES Varg has overcome the trials of his past and become an accepted member of the Bloodsworn, but now he and his newfound comrades face their biggest challenge yet: slaying a dragon. Elvar is struggling to consolidate her power in Snakavik, where she faces threats from within and without. As she fights to assert her authority in readiness for the coming conflict, she faces a surely insurmountable task: reining in the ferocity of a wolf god. As Biorr and his warband make their way north, eager for blood, Gudvarr pursues a mission of his own, hoping to win Lik-Rifa's favour and further his own ambitions. All paths lead to Snakavik, where the lines are being drawn for the final battle - a titanic clash that will shake the foundations of the world, and bear witness to the true fury of the gods. <i>Praise for The Bloodsworn series:</i><b>'A masterfully crafted, brutally compelling Norse-inspired epic'</b> Anthony Ryan<b>'Visceral, heart-breaking and unputdownable'</b> Jay Kristoff<b>'A satisfying and riveting read. The well-realised characters move against a backdrop of a world stunning in its immensity. It's everything I've come to expect from a John Gwynne book'</b> Robin HobbThe Bloodsworn series <i>The Shadow of the Gods</i><i>The Hunger of the Gods</i> <i>The Fury of the Gods</i>	English	\N	Unknown	https://images.isbndb.com/covers/25424783482314.jpg	http://books.google.com/books/content?id=xiwNzgEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api	\N	f	24.0	15.3	\N	\N	2026-02-12 19:14:25.554524	ca9fdb85-91ab-409b-915d-f6a5d48fc114	\N
\.


--
-- Data for Name: books_authors; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.books_authors (book_id, author_id) FROM stdin;
c6f3085f-5e9c-4115-8ec1-176cb7c69401	4a3787ed-5dd3-4e6f-9d36-b879ab9d86ed
bd52a9ea-a983-463b-92cb-5a56c78cd359	355185c3-90ca-4a93-be79-2b1d3a2d4633
ecf063e1-8a29-4460-94fe-27ce3e69d81a	ad17b5b5-37e7-42f4-8ead-469087228da2
2485bf0e-3207-4543-8df1-5131764719b9	fd16400e-97aa-4e1b-98be-45bda89a211e
620afc88-54b0-4d12-ac77-1ae3f4eabfa1	09657f26-612b-4799-b240-36ec2d0c2a9d
dae78a50-a220-4167-9873-a3b9d8edf130	03dde1d8-76c8-41c4-aab2-03ea66659272
5054f51d-1d2b-4a80-89c8-d846a099ecaf	09657f26-612b-4799-b240-36ec2d0c2a9d
2cd52dc7-3cc5-4952-b780-da7ee9615e89	09657f26-612b-4799-b240-36ec2d0c2a9d
dd1badc8-82f3-4ad1-b331-a4e8ce87a7b9	7d319db0-e498-4b10-98ca-120378080229
67f8d7a7-fb8e-4458-a790-29eefdae860a	7d319db0-e498-4b10-98ca-120378080229
2c041218-157a-4ad9-bc95-4c2d1edc2701	7d319db0-e498-4b10-98ca-120378080229
bea3d501-8e93-4faf-8938-73c166193104	03dde1d8-76c8-41c4-aab2-03ea66659272
27cc4939-e6a5-4565-b315-5b0b15b074c1	03dde1d8-76c8-41c4-aab2-03ea66659272
b4c92622-6c9d-4c51-9409-3edc05420ccf	9f537022-d4bb-46dd-b20f-a0420c6b7086
6113d050-8454-44c3-9e8c-adb30e410b61	9f537022-d4bb-46dd-b20f-a0420c6b7086
321ab573-02da-458d-be58-ed4de7a34e37	9f537022-d4bb-46dd-b20f-a0420c6b7086
\.


--
-- Data for Name: dewey_decimals; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.dewey_decimals (id, code, date_created) FROM stdin;
9	823.92	2026-02-12 17:55:40.690028
10	813.54	2026-02-12 19:10:55.531748
11	813.6	2026-02-12 19:13:06.631366
\.


--
-- Data for Name: books_dewey_decimals; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.books_dewey_decimals (dewey_decimal_id, book_id) FROM stdin;
9	c6f3085f-5e9c-4115-8ec1-176cb7c69401
10	bd52a9ea-a983-463b-92cb-5a56c78cd359
11	2485bf0e-3207-4543-8df1-5131764719b9
11	620afc88-54b0-4d12-ac77-1ae3f4eabfa1
11	5054f51d-1d2b-4a80-89c8-d846a099ecaf
11	2cd52dc7-3cc5-4952-b780-da7ee9615e89
9	bea3d501-8e93-4faf-8938-73c166193104
9	b4c92622-6c9d-4c51-9409-3edc05420ccf
9	6113d050-8454-44c3-9e8c-adb30e410b61
9	321ab573-02da-458d-be58-ed4de7a34e37
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
53	Science Fiction	2026-02-12 19:10:55.531748
54	Space Opera	2026-02-12 19:10:55.531748
55	Classics	2026-02-12 19:10:55.531748
56	Media Tie-In	2026-02-12 19:12:10.973471
57	Hard Science Fiction	2026-02-12 19:13:06.631366
58	Thrillers	2026-02-12 19:13:06.631366
59	Suspense	2026-02-12 19:13:06.631366
60	Crime	2026-02-12 19:13:21.821302
61	Sagas	2026-02-12 19:13:21.821302
62	Dragons	2026-02-12 19:13:29.993659
63	Family Life	2026-02-12 19:13:46.290132
64	Siblings	2026-02-12 19:13:46.290132
65	Asian American & Pacific Islander	2026-02-12 19:13:46.290132
66	Death	2026-02-12 19:13:54.414107
67	Young Adult Fiction	2026-02-12 19:14:01.64641
68	Apocalyptic & Post-Apocalyptic	2026-02-12 19:14:01.64641
69	Juvenile Fiction	2026-02-12 19:14:10.277806
70	Mysteries & Detective Stories	2026-02-12 19:14:10.277806
71	Social Themes	2026-02-12 19:14:10.277806
72	Death, Grief, Bereavement	2026-02-12 19:14:10.277806
73	Emotions & Feelings	2026-02-12 19:14:10.277806
74	Violence	2026-02-12 19:14:10.277806
75	Thrillers & Suspense	2026-02-12 19:14:10.277806
76	Dragons & Mythical Creatures	2026-02-12 19:14:17.641953
77	Dark Fantasy	2026-02-12 19:14:35.157782
78	Military	2026-02-12 19:14:50.127283
\.


--
-- Data for Name: books_genres; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.books_genres (genre_id, book_id) FROM stdin;
47	c6f3085f-5e9c-4115-8ec1-176cb7c69401
48	c6f3085f-5e9c-4115-8ec1-176cb7c69401
49	c6f3085f-5e9c-4115-8ec1-176cb7c69401
50	c6f3085f-5e9c-4115-8ec1-176cb7c69401
51	c6f3085f-5e9c-4115-8ec1-176cb7c69401
52	c6f3085f-5e9c-4115-8ec1-176cb7c69401
47	bd52a9ea-a983-463b-92cb-5a56c78cd359
53	bd52a9ea-a983-463b-92cb-5a56c78cd359
54	bd52a9ea-a983-463b-92cb-5a56c78cd359
55	bd52a9ea-a983-463b-92cb-5a56c78cd359
49	bd52a9ea-a983-463b-92cb-5a56c78cd359
51	bd52a9ea-a983-463b-92cb-5a56c78cd359
47	ecf063e1-8a29-4460-94fe-27ce3e69d81a
49	ecf063e1-8a29-4460-94fe-27ce3e69d81a
50	ecf063e1-8a29-4460-94fe-27ce3e69d81a
51	ecf063e1-8a29-4460-94fe-27ce3e69d81a
56	ecf063e1-8a29-4460-94fe-27ce3e69d81a
47	2485bf0e-3207-4543-8df1-5131764719b9
53	2485bf0e-3207-4543-8df1-5131764719b9
48	2485bf0e-3207-4543-8df1-5131764719b9
57	2485bf0e-3207-4543-8df1-5131764719b9
58	2485bf0e-3207-4543-8df1-5131764719b9
59	2485bf0e-3207-4543-8df1-5131764719b9
47	620afc88-54b0-4d12-ac77-1ae3f4eabfa1
49	620afc88-54b0-4d12-ac77-1ae3f4eabfa1
51	620afc88-54b0-4d12-ac77-1ae3f4eabfa1
48	620afc88-54b0-4d12-ac77-1ae3f4eabfa1
58	620afc88-54b0-4d12-ac77-1ae3f4eabfa1
60	620afc88-54b0-4d12-ac77-1ae3f4eabfa1
61	620afc88-54b0-4d12-ac77-1ae3f4eabfa1
62	dae78a50-a220-4167-9873-a3b9d8edf130
47	5054f51d-1d2b-4a80-89c8-d846a099ecaf
49	5054f51d-1d2b-4a80-89c8-d846a099ecaf
51	5054f51d-1d2b-4a80-89c8-d846a099ecaf
48	5054f51d-1d2b-4a80-89c8-d846a099ecaf
58	5054f51d-1d2b-4a80-89c8-d846a099ecaf
60	5054f51d-1d2b-4a80-89c8-d846a099ecaf
61	5054f51d-1d2b-4a80-89c8-d846a099ecaf
47	2cd52dc7-3cc5-4952-b780-da7ee9615e89
48	2cd52dc7-3cc5-4952-b780-da7ee9615e89
49	2cd52dc7-3cc5-4952-b780-da7ee9615e89
50	2cd52dc7-3cc5-4952-b780-da7ee9615e89
51	2cd52dc7-3cc5-4952-b780-da7ee9615e89
58	2cd52dc7-3cc5-4952-b780-da7ee9615e89
60	2cd52dc7-3cc5-4952-b780-da7ee9615e89
63	2cd52dc7-3cc5-4952-b780-da7ee9615e89
64	2cd52dc7-3cc5-4952-b780-da7ee9615e89
65	2cd52dc7-3cc5-4952-b780-da7ee9615e89
66	dd1badc8-82f3-4ad1-b331-a4e8ce87a7b9
67	67f8d7a7-fb8e-4458-a790-29eefdae860a
53	67f8d7a7-fb8e-4458-a790-29eefdae860a
68	67f8d7a7-fb8e-4458-a790-29eefdae860a
69	2c041218-157a-4ad9-bc95-4c2d1edc2701
70	2c041218-157a-4ad9-bc95-4c2d1edc2701
71	2c041218-157a-4ad9-bc95-4c2d1edc2701
72	2c041218-157a-4ad9-bc95-4c2d1edc2701
53	2c041218-157a-4ad9-bc95-4c2d1edc2701
50	2c041218-157a-4ad9-bc95-4c2d1edc2701
67	2c041218-157a-4ad9-bc95-4c2d1edc2701
73	2c041218-157a-4ad9-bc95-4c2d1edc2701
74	2c041218-157a-4ad9-bc95-4c2d1edc2701
75	2c041218-157a-4ad9-bc95-4c2d1edc2701
47	bea3d501-8e93-4faf-8938-73c166193104
49	bea3d501-8e93-4faf-8938-73c166193104
51	bea3d501-8e93-4faf-8938-73c166193104
48	bea3d501-8e93-4faf-8938-73c166193104
76	bea3d501-8e93-4faf-8938-73c166193104
47	27cc4939-e6a5-4565-b315-5b0b15b074c1
49	27cc4939-e6a5-4565-b315-5b0b15b074c1
50	27cc4939-e6a5-4565-b315-5b0b15b074c1
51	27cc4939-e6a5-4565-b315-5b0b15b074c1
48	27cc4939-e6a5-4565-b315-5b0b15b074c1
76	27cc4939-e6a5-4565-b315-5b0b15b074c1
47	b4c92622-6c9d-4c51-9409-3edc05420ccf
49	b4c92622-6c9d-4c51-9409-3edc05420ccf
51	b4c92622-6c9d-4c51-9409-3edc05420ccf
77	b4c92622-6c9d-4c51-9409-3edc05420ccf
52	b4c92622-6c9d-4c51-9409-3edc05420ccf
48	b4c92622-6c9d-4c51-9409-3edc05420ccf
47	6113d050-8454-44c3-9e8c-adb30e410b61
49	6113d050-8454-44c3-9e8c-adb30e410b61
51	6113d050-8454-44c3-9e8c-adb30e410b61
52	6113d050-8454-44c3-9e8c-adb30e410b61
77	6113d050-8454-44c3-9e8c-adb30e410b61
47	321ab573-02da-458d-be58-ed4de7a34e37
49	321ab573-02da-458d-be58-ed4de7a34e37
51	321ab573-02da-458d-be58-ed4de7a34e37
52	321ab573-02da-458d-be58-ed4de7a34e37
77	321ab573-02da-458d-be58-ed4de7a34e37
48	321ab573-02da-458d-be58-ed4de7a34e37
78	321ab573-02da-458d-be58-ed4de7a34e37
\.


--
-- Data for Name: publishers; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.publishers (id, name, date_created) FROM stdin;
4925c6f9-3a14-4cfc-8a2e-abb60f8245d0	Simon And Schuster	2026-02-12 17:55:40.690028
ff86f049-1578-4c53-8ae5-215df9c48d61	Penguin	2026-02-12 19:10:55.531748
be97dc00-a0d0-4d37-a0e6-cde563299f8c	Orion Publishing Group, Limited	2026-02-12 19:12:10.973471
12c62220-76c3-47d6-af2b-edd4dd077469	Random House Publishing Group	2026-02-12 19:13:06.631366
1840fcf6-ffc7-4115-8844-cbe7f67989ea	Orbit	2026-02-12 19:13:21.821302
7b252a8a-83af-47ae-84da-200ce945ed86	Little, Brown Book Group	2026-02-12 19:13:29.993659
3edf4b2b-809d-4385-bd04-72e9bfb2506b	Walker Books	2026-02-12 19:13:54.414107
cbe01d90-be61-4742-8cd8-7437abe4a0b6	Simon & Schuster Bfyr	2026-02-12 19:14:01.64641
ca9fdb85-91ab-409b-915d-f6a5d48fc114	Little, Brown Book Group Limited	2026-02-12 19:14:25.554524
6fce1ddd-19f5-40b6-81c0-aceff14ab767	People's Press	2026-03-27 12:40:15.29416
\.


--
-- Data for Name: series; Type: TABLE DATA; Schema: library; Owner: -
--

COPY library.series (id, name, date_created) FROM stdin;
\.


--
-- Name: author_name_variants_id_seq; Type: SEQUENCE SET; Schema: library; Owner: -
--

SELECT pg_catalog.setval('library.author_name_variants_id_seq', 124, true);


--
-- Name: dewey_decimals_id_seq; Type: SEQUENCE SET; Schema: library; Owner: -
--

SELECT pg_catalog.setval('library.dewey_decimals_id_seq', 11, true);


--
-- Name: genres_id_seq; Type: SEQUENCE SET; Schema: library; Owner: -
--

SELECT pg_catalog.setval('library.genres_id_seq', 78, true);


--
-- PostgreSQL database dump complete
--

\unrestrict YShvlQm9ZRafKaI0Xbecimz8iSPmCWMPMeyYwdcaLPk2NKVDflsbrNQi9AbEhWi

