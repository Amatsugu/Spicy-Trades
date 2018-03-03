
-- Author Esteban Garcia---

-- object: esteban | type: ROLE --
-- DROP ROLE IF EXISTS esteban;
CREATE ROLE esteban WITH 
	INHERIT
	LOGIN
	ENCRYPTED PASSWORD '********';

CREATE TABLE public.recipes(
	id_recipe character varying(45) NOT NULL,
	ingredient1 character varying,
	ingredients2 character varying,
	"method_ID" character varying,
	"id_Resources" bigint,
	CONSTRAINT recipes_pkey PRIMARY KEY (id_recipe)

);


ALTER TABLE public.recipes OWNER TO postgres;


CREATE TABLE public."Resources"(
	"id_Resources" bigint NOT NULL,
	price bigint,
	name character varying NOT NULL,
	recipe character varying,
	"player_ID" bigint,
	CONSTRAINT "Resources_pkey" PRIMARY KEY ("id_Resources")


);
ALTER TABLE public."Resources" OWNER TO postgres;


CREATE TABLE public.session(
	"session_ID" bigint NOT NULL,
	"lastLogin" character varying,
	"last_active_ID" bigint,
	"last_active_Date" bit varying,
	CONSTRAINT session_pkey PRIMARY KEY ("session_ID")

);
ALTER TABLE public.session OWNER TO postgres;


CREATE TABLE public.players(
	"player_ID" bigint NOT NULL,
	username character varying NOT NULL,
	password character varying,
	"session_ID" bigint,
	CONSTRAINT players_pkey PRIMARY KEY ("player_ID")

);
ALTER TABLE public.players OWNER TO postgres;


CREATE INDEX fki_resources_to_players ON public."Resources"
	USING btree
	(
	  "player_ID"
	)
	WITH (FILLFACTOR = 90);

ALTER TABLE public.recipes ADD CONSTRAINT "recipes_to_resourceID" FOREIGN KEY ("id_Resources")
REFERENCES public."Resources" ("id_Resources") MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE public."Resources" ADD CONSTRAINT resources_to_players FOREIGN KEY ("player_ID")
REFERENCES public.players ("player_ID") MATCH SIMPLE
ON DELETE NO ACTION ON UPDATE NO ACTION;


