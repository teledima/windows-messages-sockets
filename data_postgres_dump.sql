--
-- PostgreSQL database dump
--

-- Dumped from database version 13.3
-- Dumped by pg_dump version 13.3

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Data for Name: games; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.games VALUES (2, 'Cities: Skylines');
INSERT INTO public.games VALUES (1, 'The Witcher');
INSERT INTO public.games VALUES (3, 'Forza Horizon 4');
INSERT INTO public.games VALUES (4, 'PAYDAY 2');
INSERT INTO public.games VALUES (5, 'DARK SOULS II');
INSERT INTO public.games VALUES (7, 'The Witcher 2: Assassins of Kings');
INSERT INTO public.games VALUES (8, '????????her 2: Assassins of Kings');


--
-- Data for Name: achievements; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.achievements VALUES (2, 'Pioner', 2);
INSERT INTO public.achievements VALUES (1, 'Triss'' romance card', 1);
INSERT INTO public.achievements VALUES (5, 'No Turninig Back', 4);
INSERT INTO public.achievements VALUES (14, 'Green Energy', 2);
INSERT INTO public.achievements VALUES (15, 'Climbing the Social Ladder', 2);
INSERT INTO public.achievements VALUES (16, 'Unpopular Mayor', 2);
INSERT INTO public.achievements VALUES (17, 'Self Recollection', 5);
INSERT INTO public.achievements VALUES (19, 'Optional Safe', 3);
INSERT INTO public.achievements VALUES (20, 'First Love', 3);
INSERT INTO public.achievements VALUES (21, 'Stay Safe', 3);
INSERT INTO public.achievements VALUES (22, 'Last Giant', 5);
INSERT INTO public.achievements VALUES (23, 'Covenant of the Meek', 5);
INSERT INTO public.achievements VALUES (24, 'Taster', 7);
INSERT INTO public.achievements VALUES (25, 'Intimidator', 7);
INSERT INTO public.achievements VALUES (26, 'Craftsman', 7);
INSERT INTO public.achievements VALUES (27, 'Great Potion!', 7);
INSERT INTO public.achievements VALUES (28, 'Journeyman', 7);
INSERT INTO public.achievements VALUES (29, 'Apprentice', 7);
INSERT INTO public.achievements VALUES (30, 'Gambler', 7);
INSERT INTO public.achievements VALUES (31, 'To Aedirn!', 7);


--
-- Data for Name: categories; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.categories VALUES (2, 'Urban planning');
INSERT INTO public.categories VALUES (1, 'Role game');
INSERT INTO public.categories VALUES (3, 'Racing');
INSERT INTO public.categories VALUES (4, 'Open World');
INSERT INTO public.categories VALUES (5, 'Multiplayer');
INSERT INTO public.categories VALUES (6, 'Singleplayer');
INSERT INTO public.categories VALUES (7, 'Simulation');
INSERT INTO public.categories VALUES (8, 'Management');
INSERT INTO public.categories VALUES (9, 'Realistic');
INSERT INTO public.categories VALUES (10, 'Classic');
INSERT INTO public.categories VALUES (11, 'Dark Fantasy');
INSERT INTO public.categories VALUES (12, 'Arcade');
INSERT INTO public.categories VALUES (13, 'Action');
INSERT INTO public.categories VALUES (14, 'RPG');
INSERT INTO public.categories VALUES (15, 'Souls-like');


--
-- Data for Name: downloadable_contents; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.downloadable_contents VALUES (2, 'Cities: Skylines - After Dark', 2);
INSERT INTO public.downloadable_contents VALUES (1, 'The Witcher: Enhanced Edition Soundtrack', 1);
INSERT INTO public.downloadable_contents VALUES (4, 'Forza Horizon 4: Welcome Pack', 3);
INSERT INTO public.downloadable_contents VALUES (5, 'Cities: Skylines - Snowfall', 2);
INSERT INTO public.downloadable_contents VALUES (6, 'Cities: Skylines - Mass Transit', 2);
INSERT INTO public.downloadable_contents VALUES (8, 'Dark Souls II: Scholar of the First Sin', 5);


--
-- Data for Name: games_categories; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.games_categories VALUES (2, 2);
INSERT INTO public.games_categories VALUES (1, 1);
INSERT INTO public.games_categories VALUES (3, 3);
INSERT INTO public.games_categories VALUES (3, 4);
INSERT INTO public.games_categories VALUES (4, 5);
INSERT INTO public.games_categories VALUES (1, 6);
INSERT INTO public.games_categories VALUES (2, 7);
INSERT INTO public.games_categories VALUES (2, 8);
INSERT INTO public.games_categories VALUES (2, 9);
INSERT INTO public.games_categories VALUES (5, 11);
INSERT INTO public.games_categories VALUES (1, 11);
INSERT INTO public.games_categories VALUES (3, 12);
INSERT INTO public.games_categories VALUES (5, 13);
INSERT INTO public.games_categories VALUES (5, 14);
INSERT INTO public.games_categories VALUES (5, 15);


--
-- Name: achievements_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.achievements_id_seq', 31, true);


--
-- Name: categories_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.categories_id_seq', 15, true);


--
-- Name: downloadable_contents_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.downloadable_contents_id_seq', 8, true);


--
-- Name: games_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.games_id_seq', 8, true);


--
-- PostgreSQL database dump complete
--

