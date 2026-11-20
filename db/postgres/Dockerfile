# syntax=docker/dockerfile:1
#
# Postgres for Azure Container Apps: the official image plus an entrypoint
# wrapper that refuses to start while a previous postmaster still answers.
# Tag it <postgres version>-<wrapper revision>, e.g. 17.12-1 (see deploy.yml).
#
#   docker build --build-arg PG_VERSION=17.12 -t syntax-postgres:17.12-1 db/postgres

ARG PG_VERSION=17.12
FROM postgres:${PG_VERSION}-alpine

COPY --chmod=0755 wait-for-previous.sh /usr/local/bin/wait-for-previous.sh

# STOPSIGNAL SIGINT (fast shutdown) is inherited from the base image.
ENTRYPOINT ["wait-for-previous.sh"]
CMD ["postgres"]
