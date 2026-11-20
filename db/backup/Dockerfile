# syntax=docker/dockerfile:1
#
# pg_dump / pg_restore against Blob storage, for the backup and restore jobs.
# PG_VERSION must match the server (pg_dump refuses newer servers).
#
#   docker build --build-arg PG_VERSION=17.12 -t syntax-pg-backup db/backup

ARG PG_VERSION=17.12
FROM postgres:${PG_VERSION}-alpine

RUN apk add --no-cache curl jq

COPY --chmod=0755 pg-tools.sh /usr/local/bin/pg-tools.sh

USER postgres
ENTRYPOINT ["pg-tools.sh"]
CMD ["backup"]
