CREATE DATABASE project_tracker_db;
CREATE USER kc_db_user WITH PASSWORD 'kc_db_secret';
CREATE DATABASE keycloak_db OWNER kc_db_user;
