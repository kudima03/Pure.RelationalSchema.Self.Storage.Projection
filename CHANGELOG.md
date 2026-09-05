# Changelog

All notable changes to Pure.RelationalSchema.Self.Storage.Projection are documented here.

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

---

## [0.1.0-preview.6.0.5] — 2026-08-06

- Maintenance release: dependency and build updates.

## [0.1.0-preview.6.0.4] — 2026-08-04

- Maintenance release: dependency and build updates.

## [0.1.0-preview.6.0.2] — 2026-06-25

- Maintenance release: dependency and build updates.

## [0.1.0-preview.6.0.1] — 2026-06-07

- Maintenance release: dependency and build updates.

## [0.1.0-preview.6.0.0] — 2026-05-28

- Maintenance release: dependency and build updates.

## [0.1.0-preview.5.0.2] — 2026-05-20

- Maintenance release: dependency and build updates.

## [0.1.0-preview.5.0.1] — 2026-03-16

- Maintenance release: dependency and build updates.

## [0.1.0-preview.5.0.0] — 2026-03-16

### Added

- Multi-targeting: the package now also targets `net8.0` and `net10.0`, in
  addition to `net9.0`.
- `SchemaEntityProjection` gained two new public constructors —
  `SchemaEntityProjection(IString name)` and
  `SchemaEntityProjection(IString name, IEnumerable<IColumn> columns)` — for
  building a schema row directly from a name, alongside the existing
  `SchemaEntityProjection(ISchema schema)`.

### Fixed

- Projected rows for columns, column types, indexes, foreign keys, and tables
  now include a UUID primary-key cell, matching the identity cell already
  produced for the schema row.

## [0.1.0-preview.4.0.0] — 2025-11-15

### Removed

- **Breaking:** `CellSwitch<TSelector>` is now internal (previously public).
- **Breaking:** the public constructor
  `SchemaEntityProjection(ISchema entity, IEnumerable<IColumn> columns)` was
  removed; only `SchemaEntityProjection(ISchema entity)` remains public.

## [0.1.0-preview.3.1.0] — 2025-11-10

### Added

- `SchemaEntityProjection` is now public (previously internal), so schema
  rows can be constructed and consumed directly.

## [0.1.0-preview.3.0.0] — 2025-11-04

- Maintenance release: dependency and build updates.

## [0.1.0-preview.2.0.0] — 2025-11-03

- Maintenance release: dependency and build updates.

## [0.1.0-preview.1.0.1] — 2025-11-03

- Maintenance release: dependency and build updates.

## [0.1.0-preview.1.0.0] — 2025-10-30

### Added

- `SchemaProjection` now also emits `IndexesToColumnsTable` and
  `SchemasToForeignKeysTable` groupings, completing coverage of the schema's
  join tables (13 table groupings in total).

## [0.1.0-preview.0.1.0] — 2025-10-21

### Added

- Initial release. `SchemaProjection(ISchema)` projects a relational schema
  into `IEnumerable<IGrouping<ITable, IRow>>`, covering columns, column
  types, tables, indexes, foreign keys, schemas, and their join tables, so
  the schema can be inserted into storage defined by
  `Pure.RelationalSchema.Self.Schema`.
