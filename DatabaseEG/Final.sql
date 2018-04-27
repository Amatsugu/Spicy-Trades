
-- object: esteban | type: ROLE --
-- DROP ROLE IF EXISTS esteban;
CREATE ROLE esteban WITH 
	INHERIT
	LOGIN
	ENCRYPTED PASSWORD '********';
-- ddl-end --

-- Database creation must be done outside an multicommand file.
-- These commands were put in this file only for convenience.
-- -- object: "Final" | type: DATABASE --
-- -- DROP DATABASE IF EXISTS "Final";
-- CREATE DATABASE "Final"
-- 	ENCODING = 'UTF8'
-- 	LC_COLLATE = 'English_United States.1252'
-- 	LC_CTYPE = 'English_United States.1252'
-- 	TABLESPACE = pg_default
-- 	OWNER = postgres
-- ;
-- -- ddl-end --
-- 

-- object: public.players | type: TABLE --
-- DROP TABLE IF EXISTS public.players CASCADE;
CREATE TABLE public.players(
	player_id integer NOT NULL DEFAULT nextval('serialgen'::regclass),
	username text NOT NULL,
	password text NOT NULL,
	email text NOT NULL,
	CONSTRAINT players_pkey PRIMARY KEY (player_id),
	CONSTRAINT players_email_key UNIQUE (email)

);
-- ddl-end --
ALTER TABLE public.players OWNER TO postgres;
-- ddl-end --

-- object: public.friendslist | type: TABLE --
-- DROP TABLE IF EXISTS public.friendslist CASCADE;
CREATE TABLE public.friendslist(
	player_id integer NOT NULL,
	flist integer[],
	CONSTRAINT friendslist_pkey PRIMARY KEY (player_id)

);
-- ddl-end --
ALTER TABLE public.friendslist OWNER TO postgres;
-- ddl-end --

-- object: fk_players | type: CONSTRAINT --
-- ALTER TABLE public.friendslist DROP CONSTRAINT IF EXISTS fk_players CASCADE;
ALTER TABLE public.friendslist ADD CONSTRAINT fk_players FOREIGN KEY (player_id)
REFERENCES public.players (player_id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --


