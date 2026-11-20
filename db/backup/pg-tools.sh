#!/bin/sh
# Back up the database to Azure Blob storage, or restore it from there.
#
#   pg-tools.sh backup                    pg_dump -> <container>/<db>-<UTC timestamp>.dump
#   pg-tools.sh restore [latest|<name>]   download a dump and pg_restore it
#
# Environment:
#   PGHOST PGPORT PGDATABASE PGUSER PGPASSWORD  libpq connection settings
#   BACKUP_CONTAINER_URL  https://<account>.blob.core.windows.net/<container>
#   AZURE_CLIENT_ID       user-assigned managed identity to use
#   IDENTITY_ENDPOINT, IDENTITY_HEADER  set by Azure Container Apps
#
# Blob access uses the managed identity (Storage Blob Data Contributor on the
# container); there are no storage keys. Old dumps are deleted by the storage
# account's lifecycle rule, not by this script.
set -eu

: "${PGDATABASE:?PGDATABASE is required}"
: "${BACKUP_CONTAINER_URL:?BACKUP_CONTAINER_URL is required}"

storage_api_version="2023-11-03"
work="${TMPDIR:-/tmp}"

token() {
  response="$(curl -fsS -G "$IDENTITY_ENDPOINT" \
    --data-urlencode "resource=https://storage.azure.com/" \
    --data-urlencode "api-version=2019-08-01" \
    --data-urlencode "client_id=${AZURE_CLIENT_ID:-}" \
    -H "X-IDENTITY-HEADER: $IDENTITY_HEADER")"
  printf '%s' "$response" | jq -er .access_token
}

blob() {
  # blob <curl args...> : authenticated request against the backup container.
  # The token is fetched in an assignment so a failure stops the script (set -e).
  access_token="$(token)"
  curl -fsS -H "Authorization: Bearer $access_token" -H "x-ms-version: $storage_api_version" "$@"
}

backup() {
  name="${PGDATABASE}-$(date -u +%Y%m%dT%H%M%SZ).dump"
  file="$work/$name"

  echo "Dumping $PGDATABASE from $PGHOST"
  pg_dump --format=custom --no-owner --file="$file"

  echo "Uploading $name ($(wc -c < "$file") bytes)"
  blob --upload-file "$file" \
    -H "x-ms-blob-type: BlockBlob" \
    -H "Content-Type: application/octet-stream" \
    "$BACKUP_CONTAINER_URL/$name" > /dev/null

  rm -f "$file"
  echo "Backup $name done"
}

latest() {
  # Names sort by their UTC timestamp, so the last one is the newest.
  listing="$(blob "$BACKUP_CONTAINER_URL?restype=container&comp=list&prefix=${PGDATABASE}-")"
  printf '%s' "$listing" | grep -o '<Name>[^<]*</Name>' | sed -e 's/<Name>//' -e 's/<\/Name>//' | sort | tail -n 1
}

restore() {
  name="${1:-latest}"
  if [ "$name" = "latest" ]; then
    name="$(latest)" || exit 1
  fi
  if [ -z "$name" ]; then
    echo "No backups found in $BACKUP_CONTAINER_URL" >&2
    exit 1
  fi

  file="$work/restore.dump"
  echo "Downloading $name"
  blob -o "$file" "$BACKUP_CONTAINER_URL/$name"

  echo "Disconnecting other sessions from $PGDATABASE"
  psql -v ON_ERROR_STOP=1 -qtA -c \
    "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = current_database() AND pid <> pg_backend_pid();" \
    > /dev/null

  echo "Restoring $name into $PGDATABASE"
  pg_restore --clean --if-exists --no-owner --single-transaction --exit-on-error \
    --dbname="$PGDATABASE" "$file"

  rm -f "$file"
  echo "Restore of $name done"
}

case "${1:-backup}" in
  backup)  backup ;;
  restore) restore "${2:-latest}" ;;
  *)
    echo "usage: pg-tools.sh backup | restore [latest|<blob name>]" >&2
    exit 2
    ;;
esac
