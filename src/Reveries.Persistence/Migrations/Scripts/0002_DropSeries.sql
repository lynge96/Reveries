ALTER TABLE catalog.works DROP CONSTRAINT IF EXISTS works_series_number_requires_series;

DROP INDEX IF EXISTS catalog.idx_works_series_id;

ALTER TABLE catalog.works DROP COLUMN IF EXISTS series_id;
ALTER TABLE catalog.works DROP COLUMN IF EXISTS series_number;

DROP TABLE IF EXISTS catalog.series;