#!/usr/bin/env bash
set -euo pipefail

if [ -x /opt/mssql-tools18/bin/sqlcmd ]; then
  SQLCMD=/opt/mssql-tools18/bin/sqlcmd
elif [ -x /opt/mssql-tools/bin/sqlcmd ]; then
  SQLCMD=/opt/mssql-tools/bin/sqlcmd
else
  echo "sqlcmd not found in the mssql-tools image." >&2
  exit 1
fi

MSSQL_HOST=${MSSQL_HOST:-sqlserver}
MSSQL_PORT=${MSSQL_PORT:-1433}
MSSQL_USER=${MSSQL_USER:-sa}
: "${MSSQL_PASSWORD:?MSSQL_PASSWORD is required}"
MSSQL_BACKUP_DATABASE=${MSSQL_BACKUP_DATABASE:-master}
BACKUP_DIR=${BACKUP_DIR:-/var/opt/mssql/backups}
BACKUP_INTERVAL_SECONDS=${BACKUP_INTERVAL_SECONDS:-3600}
RETENTION_DAYS=${RETENTION_DAYS:-7}

echo "Waiting for SQL Server at ${MSSQL_HOST}:${MSSQL_PORT}..."
ready=0
for _ in $(seq 1 60); do
  if "$SQLCMD" -S "${MSSQL_HOST},${MSSQL_PORT}" -U "$MSSQL_USER" -P "$MSSQL_PASSWORD" -Q "SELECT 1" >/dev/null 2>&1; then
    ready=1
    break
  fi
  sleep 5
done

if [ "$ready" -ne 1 ]; then
  echo "SQL Server did not become ready in time." >&2
  exit 1
fi

while true; do
  timestamp=$(date -u +"%Y%m%dT%H%M%SZ")
  backup_file="${BACKUP_DIR}/${MSSQL_BACKUP_DATABASE}_${timestamp}.bak"
  echo "Backing up ${MSSQL_BACKUP_DATABASE} to ${backup_file}..."
  "$SQLCMD" -S "${MSSQL_HOST},${MSSQL_PORT}" -U "$MSSQL_USER" -P "$MSSQL_PASSWORD" \
    -Q "BACKUP DATABASE [${MSSQL_BACKUP_DATABASE}] TO DISK = N'${backup_file}' WITH INIT, COMPRESSION;"

  find "$BACKUP_DIR" -type f -name "${MSSQL_BACKUP_DATABASE}_*.bak" -mtime +"$RETENTION_DAYS" -print -delete || true
  sleep "$BACKUP_INTERVAL_SECONDS"
done
