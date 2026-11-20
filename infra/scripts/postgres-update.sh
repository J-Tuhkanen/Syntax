#!/usr/bin/env bash
# Update the Postgres container app without ever running two postmasters on the
# same data directory: stop the app, wait until it is stopped, update it, start
# it again. Terraform ignores the app's template after creation, so this is the
# way to change its image (or CPU, memory, environment variables).
#
# Usage:
#   infra/scripts/postgres-update.sh <resource-group> <app-name> <image> [extra az containerapp update args]
#
# Example (minor version upgrade; push the image first, the pipeline does that):
#   infra/scripts/postgres-update.sh rg-syntax-dev ca-syntax-postgres-dev \
#     acrsyntaxab12.azurecr.io/syntax-postgres:17.13-1
#
# Take a backup first:
#   az containerapp job start -g rg-syntax-dev -n caj-syntax-pgbackup-dev
#
# Major versions (17 -> 18) cannot be done this way: the data directory format
# changes. Restore a dump into a fresh share instead (see infra/README.md).
set -euo pipefail

if [[ $# -lt 3 ]]; then
  sed -n '2,20p' "$0"
  exit 2
fi

rg="$1"
app="$2"
image="$3"
shift 3

api_version="2025-01-01"
subscription="$(az account show --query id -o tsv)"
url="/subscriptions/${subscription}/resourceGroups/${rg}/providers/Microsoft.App/containerApps/${app}"

running_status() {
  az rest --method get --url "${url}?api-version=${api_version}" --query properties.runningStatus -o tsv
}

wait_for() {
  local wanted="$1"
  for _ in $(seq 1 60); do
    if [[ "$(running_status)" == "$wanted" ]]; then
      return 0
    fi
    sleep 10
  done
  echo "Timed out waiting for ${app} to be ${wanted}" >&2
  return 1
}

echo "Stopping ${app}"
az rest --method post --url "${url}/stop?api-version=${api_version}" --output none
wait_for Stopped

echo "Updating ${app} to ${image}"
az containerapp update --resource-group "$rg" --name "$app" --image "$image" "$@" --output none

if [[ "$(running_status)" == "Stopped" ]]; then
  echo "Starting ${app}"
  az rest --method post --url "${url}/start?api-version=${api_version}" --output none
fi
wait_for Running

echo "Done. Latest revision:"
az containerapp revision list --resource-group "$rg" --name "$app" \
  --query "[?properties.active].{name:name, created:properties.createdTime, healthy:properties.healthState}" -o table
