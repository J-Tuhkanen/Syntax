#!/bin/sh
# Apply the migrations in this folder with Flyway. Used by db/docker-compose.yml
# and by the migration job in Azure (image built from db/Dockerfile.migrate).
#
# Configuration comes from the environment:
#   DB_HOST          database host (required)
#   DB_PORT          default 5432
#   DB_NAME          database name (required)
#   FLYWAY_USER      read by Flyway (required)
#   FLYWAY_PASSWORD  read by Flyway (required)
set -eu

: "${DB_HOST:?DB_HOST is required}"
: "${DB_NAME:?DB_NAME is required}"
: "${FLYWAY_USER:?FLYWAY_USER is required}"
: "${FLYWAY_PASSWORD:?FLYWAY_PASSWORD is required}"

export FLYWAY_URL="jdbc:postgresql://${DB_HOST}:${DB_PORT:-5432}/${DB_NAME}"
export FLYWAY_LOCATIONS="${FLYWAY_LOCATIONS:-filesystem:/flyway/sql}"

# Databases created from the old init.sql already have the tables but no
# history table: baseline them below V1 (which only uses IF NOT EXISTS).
export FLYWAY_BASELINE_ON_MIGRATE=true
export FLYWAY_BASELINE_VERSION=0

# Postgres may still be starting (first deploy, or after a budget shutdown).
export FLYWAY_CONNECT_RETRIES="${FLYWAY_CONNECT_RETRIES:-30}"

exec flyway migrate
