#!/usr/bin/env bash

CONTAINER="reveries-postgres"
DB="reveries_db"
USER="reveries_user"

echo "Restoring schema..."
docker exec -i $CONTAINER psql -U $USER -d $DB < db_schema.sql

echo "Restoring data..."
docker exec -i $CONTAINER psql -U $USER -d $DB < db_data.sql

echo "Done."