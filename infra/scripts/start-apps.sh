#!/usr/bin/env bash
# Start every stopped container app in an environment's resource group,
# Postgres first. Undoes the budget shutdown runbook; the deploy pipeline runs
# it on every deployment.
#
# Usage: infra/scripts/start-apps.sh [resource-group]   (default: rg-syntax-dev)
set -euo pipefail

rg="${1:-rg-syntax-dev}"
api_version="2025-01-01"
subscription="$(az account show --query id -o tsv)"

names="$(az containerapp list --resource-group "$rg" --query "[].name" -o tsv)"
ordered="$(grep -- '-postgres-' <<<"$names" || true; grep -v -- '-postgres-' <<<"$names" || true)"

for app in $ordered; do
  url="/subscriptions/${subscription}/resourceGroups/${rg}/providers/Microsoft.App/containerApps/${app}"
  status="$(az rest --method get --url "${url}?api-version=${api_version}" --query properties.runningStatus -o tsv)"

  if [[ "$status" == "Stopped" ]]; then
    echo "Starting ${app}"
    az rest --method post --url "${url}/start?api-version=${api_version}" --output none
  else
    echo "${app}: ${status:-Running}"
  fi
done
