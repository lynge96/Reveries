#!/usr/bin/env bash

CONTAINER="reveries-postgres"
DB="reveries_db"
USER="reveries_user"

DATA_FILE="db_data.sql"

echo "Dumping data..."
docker exec $CONTAINER pg_dump -U $USER -d $DB \
  --data-only --no-owner --no-privileges > $DATA_FILE

echo "Done."