-- Database generated with pgModeler (PostgreSQL Database Modeler).
-- pgModeler  version: 0.9.0
-- PostgreSQL version: 9.6
-- Project Site: pgmodeler.com.br
-- Model Author: ---

-- object: esteban | type: ROLE --
-- DROP ROLE IF EXISTS esteban;
CREATE ROLE esteban WITH 
	INHERIT
	LOGIN
	ENCRYPTED PASSWORD '********';
-- ddl-end --


-- Database creation must be done outside an multicommand file.
-- These commands were put in this file only for convenience.
-- -- object: demo_db | type: DATABASE --
-- -- DROP DATABASE IF EXISTS demo_db;
-- CREATE DATABASE demo_db
-- 	ENCODING = 'UTF8'
-- 	LC_COLLATE = 'English_United States.1252'
-- 	LC_CTYPE = 'English_United States.1252'
-- 	TABLESPACE = pg_default
-- 	OWNER = postgres
-- ;
-- -- ddl-end --
-- 

-- object: public.recipes | type: TABLE --
-- DROP TABLE IF EXISTS public.recipes CASCADE;
CREATE TABLE public.recipes(
	id_recipe character varying(45) NOT NULL,
	ingredient1 character varying,
	ingredients2 character varying,
	"method_ID" character varying,
	"id_Resources" bigint,
	CONSTRAINT recipes_pkey PRIMARY KEY (id_recipe)

);
-- ddl-end --
ALTER TABLE public.recipes OWNER TO postgres;
-- ddl-end --

-- object: public."Resources" | type: TABLE --
-- DROP TABLE IF EXISTS public."Resources" CASCADE;
CREATE TABLE public."Resources"(
	"id_Resources" bigint NOT NULL,
	price bigint,
	name character varying NOT NULL,
	recipe character varying,
	"player_ID" bigint,
	CONSTRAINT "Resources_pkey" PRIMARY KEY ("id_Resources")

);
-- ddl-end --
ALTER TABLE public."Resources" OWNER TO postgres;
-- ddl-end --

-- object: public.session | type: TABLE --
-- DROP TABLE IF EXISTS public.session CASCADE;
CREATE TABLE public.session(
	"session_ID" bigint NOT NULL,
	"lastLogin" character varying,
	"last_active_ID" bigint,
	"last_active_Date" bit varying,
	CONSTRAINT session_pkey PRIMARY KEY ("session_ID")

);
-- ddl-end --
ALTER TABLE public.session OWNER TO postgres;
-- ddl-end --

-- object: public.players | type: TABLE --
-- DROP TABLE IF EXISTS public.players CASCADE;
CREATE TABLE public.players(
	"player_ID" bigint NOT NULL,
	username character varying NOT NULL,
	password character varying,
	"session_ID" bigint,
	CONSTRAINT players_pkey PRIMARY KEY ("player_ID")

);
-- ddl-end --
ALTER TABLE public.players OWNER TO postgres;
-- ddl-end --

-- object: fki_resources_to_players | type: INDEX --
-- DROP INDEX IF EXISTS public.fki_resources_to_players CASCADE;
CREATE INDEX fki_resources_to_players ON public."Resources"
	USING btree
	(
	  "player_ID"
	)
	WITH (FILLFACTOR = 90);
-- ddl-end --

-- object: "recipes_to_resourceID" | type: CONSTRAINT --
-- ALTER TABLE public.recipes DROP CONSTRAINT IF EXISTS "recipes_to_resourceID" CASCADE;
ALTER TABLE public.recipes ADD CONSTRAINT "recipes_to_resourceID" FOREIGN KEY ("id_Resources")
REFERENCES public."Resources" ("id_Resources") MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: resources_to_players | type: CONSTRAINT --
-- ALTER TABLE public."Resources" DROP CONSTRAINT IF EXISTS resources_to_players CASCADE;
ALTER TABLE public."Resources" ADD CONSTRAINT resources_to_players FOREIGN KEY ("player_ID")
REFERENCES public.players ("player_ID") MATCH SIMPLE
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --


