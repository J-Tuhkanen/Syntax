#!/bin/sh
# Entrypoint wrapper: don't start Postgres while another postmaster may still
# be using the data directory.
#
# On Azure Container Apps the data directory is an NFS share, and the platform
# can start a replacement replica (new revision, rescheduling) before the old
# one has stopped. Two postmasters on one data directory corrupt it, and
# Postgres's own postmaster.pid check can't see a process in another container.
#
# So while postmaster.pid exists we ask the app's own address (PEER_HOST: the
# platform routes it to the replica currently serving traffic) whether a
# postmaster answers. As long as one does (even one shutting down) we wait.
# The pid file is treated as a leftover from a crash only after nobody has
# answered for STALE_AFTER seconds.
set -eu

PGDATA="${PGDATA:-/var/lib/postgresql/data}"
PEER_HOST="${PEER_HOST:-}"
PEER_PORT="${PEER_PORT:-5432}"
STALE_AFTER="${STALE_AFTER:-90}"

pidfile="$PGDATA/postmaster.pid"
silent_for=0

while [ -f "$pidfile" ]; do
  status=2
  if [ -n "$PEER_HOST" ]; then
    # 0 = accepting connections, 1 = rejecting (starting or shutting down),
    # 2 = no response, 3 = no attempt made.
    if pg_isready -q -h "$PEER_HOST" -p "$PEER_PORT" -t 3; then status=0; else status=$?; fi
  fi

  if [ "$status" -le 1 ]; then
    echo "wait-for-previous: a postmaster still answers on $PEER_HOST:$PEER_PORT, waiting" >&2
    silent_for=0
  else
    silent_for=$((silent_for + 5))
    if [ "$silent_for" -ge "$STALE_AFTER" ]; then
      echo "wait-for-previous: no answer for ${STALE_AFTER}s, treating $pidfile as stale" >&2
      break
    fi
  fi

  sleep 5
done

exec docker-entrypoint.sh "$@"
