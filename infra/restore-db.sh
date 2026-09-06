#!/usr/bin/env bash

CONTAINER="reveries-postgres"
DB="reveries_db"
USER="reveries_user"

echo "Restoring data..."
docker exec -i $CONTAINER psql -U $USER -d $DB < db_data.sql

echo "Done."