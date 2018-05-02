\c postgres postgres

DROP DATABASE IF EXISTS spicy;

CREATE DATABASE spicy WITH OWNER esteban;

\c spicy esteban

CREATE TABLE players (
	player_id integer PRIMARY KEY,
	username text NOT NULL,
	password text NOT NULL,
	email text NOT NULL UNIQUE
);

CREATE TABLE friendslist (
	player_id integer NOT NULL PRIMARY KEY REFERENCES players ON DELETE CASCADE,
	flist integer[]
);