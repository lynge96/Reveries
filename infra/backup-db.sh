#!/usr/bin/env bash

CONTAINER="reveries-postgres"
DB="reveries_db"
USER="reveries_user"

SCHEMA_FILE="db_schema.sql"
DATA_FILE="db_data.sql"
BACKUP_FILE="db_backup.sql"

echo "Dumping schema..."
docker exec $CONTAINER pg_dump -U $USER -d $DB \
  --schema-only --no-owner --no-privileges > $SCHEMA_FILE

echo "Dumping data..."
docker exec $CONTAINER pg_dump -U $USER -d $DB \
  --data-only --no-owner --no-privileges > $DATA_FILE

echo "Creating full backup..."
docker exec $CONTAINER pg_dump -U $USER -d $DB \
  --no-owner --no-privileges > $BACKUP_FILE

echo "Done."