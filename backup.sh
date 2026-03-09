#!/usr/bin/env bash
# Database backup / restore helper
# Usage:
#   ./backup.sh backup           — create a backup now
#   ./backup.sh restore <file>   — restore from a .sql.gz file
#   ./backup.sh list             — list available backups

set -euo pipefail

BACKUP_DIR="${BACKUP_DIR:-$HOME/backups/hiringprocess}"
CONTAINER="${PG_CONTAINER:-hp_postgres}"
PG_USER="${PG_USER:-postgres}"
PG_DB="${PG_DB:-hiringprocess}"
KEEP="${KEEP:-7}"

cmd="${1:-backup}"

case "$cmd" in
  backup)
    mkdir -p "$BACKUP_DIR"
    TIMESTAMP=$(date +%Y%m%d_%H%M%S)
    FILE="$BACKUP_DIR/backup_${TIMESTAMP}.sql.gz"
    docker exec "$CONTAINER" pg_dump -U "$PG_USER" "$PG_DB" | gzip > "$FILE"
    echo "Backup saved: $FILE"
    # Prune old backups
    ls -t "$BACKUP_DIR"/backup_*.sql.gz 2>/dev/null | tail -n +"$((KEEP + 1))" | xargs -r rm --
    echo "Kept last $KEEP backups."
    ;;

  restore)
    FILE="${2:-}"
    if [[ -z "$FILE" ]]; then
      echo "Usage: $0 restore <backup_file.sql.gz>"
      exit 1
    fi
    if [[ ! -f "$FILE" ]]; then
      echo "File not found: $FILE"
      exit 1
    fi
    echo "Restoring $FILE into $PG_DB ..."
    gunzip -c "$FILE" | docker exec -i "$CONTAINER" psql -U "$PG_USER" -d "$PG_DB"
    echo "Restore complete."
    ;;

  list)
    echo "Available backups in $BACKUP_DIR:"
    ls -lht "$BACKUP_DIR"/backup_*.sql.gz 2>/dev/null || echo "(none)"
    ;;

  *)
    echo "Unknown command: $cmd"
    echo "Usage: $0 {backup|restore <file>|list}"
    exit 1
    ;;
esac
