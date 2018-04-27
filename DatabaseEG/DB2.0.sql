CREATE TABLE public.recipes(
	id_recipe VARCHAR(45) NOT NULL,
	ingredient1 TEXT, /*FK resource*/
	ingredients2 TEXT, /*FK resource*/
	method_ID TEXT, /*Change to enum, will till you the values*/
	id_Resources Serial, /*What is this for?*/
	CONSTRAINT recipes_pkey PRIMARY KEY (id_recipe)

);


ALTER TABLE public.recipes OWNER TO postgres;


CREATE TABLE public.Resources(
	id_Resources uuid,
	price float,
	resource_name TEXT NOT NULL,
	recipe TEXT, /*FK resource*/
	player_ID bigint,
	description TEXT,
	tool_tip TEXT,
	Is_Gatherable boolean,
	tags TEXT[],
	CONSTRAINT Resources_pkey PRIMARY KEY (id_Resources)
);

CREATE TABLE public.session(
	session_ID serial,
	lastLogin TEXT,
	last_active_ID bigint,
	last_active_Date bit varying,
	CONSTRAINT session_pkey PRIMARY KEY (session_ID)

);
ALTER TABLE public.session OWNER TO postgres;


CREATE TABLE public.players(
	player_ID bigint NOT NULL,
	username TEXT NOT NULL,
	password TEXT,
	session_ID bigint,
	CONSTRAINT players_pkey PRIMARY KEY (player_ID)

);
ALTER TABLE public.players OWNER TO postgres;


CREATE INDEX fki_resources_to_players ON public.Resources
	USING btree
	(
	  player_ID
	)
	WITH (FILLFACTOR = 90);

ALTER TABLE public.recipes ADD CONSTRAINT recipes_to_resourceID FOREIGN KEY (id_Resources)
REFERENCES public.Resources (id_Resources) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE public.Resources ADD CONSTRAINT resources_to_players FOREIGN KEY (player_ID)
REFERENCES public.players (player_ID) MATCH SIMPLE
ON DELETE NO ACTION ON UPDATE NO ACTION;


