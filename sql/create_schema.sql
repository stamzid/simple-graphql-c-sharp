CREATE SCHEMA IF NOT EXISTS notifi AUTHORIZATION notifi;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

SELECT uuid_generate_v4();

SET TIME ZONE 'UTC';

CREATE TABLE notifi.api_hits (
    id serial PRIMARY KEY,
    endpoint varchar,
    time timestamp without time zone NOT NULL
);
