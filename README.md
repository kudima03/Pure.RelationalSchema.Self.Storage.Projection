# Pure.RelationalSchema.Self.Storage.Projection

Projects an `ISchema` into normalized storage rows for the **Pure** relational schema meta-model.

[![.NET build & test](https://github.com/kudima03/Pure.RelationalSchema.Self.Storage.Projection/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/kudima03/Pure.RelationalSchema.Self.Storage.Projection/actions/workflows/build-and-test.yml)
[![Build and Deploy](https://github.com/kudima03/Pure.RelationalSchema.Self.Storage.Projection/actions/workflows/publish-nuget.yml/badge.svg?branch=main)](https://github.com/kudima03/Pure.RelationalSchema.Self.Storage.Projection/actions/workflows/publish-nuget.yml)
[![NuGet](https://img.shields.io/nuget/v/Pure.RelationalSchema.Self.Storage.Projection)](https://www.nuget.org/packages/Pure.RelationalSchema.Self.Storage.Projection)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.txt)

## Overview

`Pure.RelationalSchema.Self.Storage.Projection` takes an `ISchema` and projects it into a set of storage rows grouped by table. The result is an `IEnumerable<IGrouping<ITable, IRow>>` that covers every structural element of the schema — column types, columns, tables, indexes, foreign keys, and the join tables between them — ready to be persisted through the `Pure.RelationalSchema.Storage` layer.

## Public API

| Type | Kind | Description |
|------|------|-------------|
| `SchemaProjection` | `sealed record` | Entry point. Wraps an `ISchema` and enumerates all projection groups. |
| `SchemaEntityProjection` | `sealed record` | Projects a single `ISchema` as an `IRow` into `SchemasTable`. |

### Projection groups

`SchemaProjection` yields one `IGrouping<ITable, IRow>` per logical table in the self-schema:

| Table | Contents |
|-------|----------|
| `ColumnTypesTable` | Distinct column types across all tables |
| `ColumnsTable` | All columns, deduplicated by content hash |
| `TablesTable` | All tables |
| `TablesToColumnsTable` | Table ↔ column membership |
| `IndexesTable` | All indexes |
| `IndexesToColumnsTable` | Index ↔ column membership |
| `TablesToIndexesTable` | Table ↔ index membership |
| `ForeignKeysTable` | All foreign keys with referencing/referenced table UUIDs |
| `ForeignKeysToReferencingColumnsTable` | Foreign key ↔ referencing column |
| `ForeignKeysToReferencedColumnsTable` | Foreign key ↔ referenced column |
| `SchemasTable` | The schema itself as a single row |
| `SchemasToTablesTable` | Schema ↔ table membership |
| `SchemasToForeignKeysTable` | Schema ↔ foreign key membership |

Each entity is identified by a ULID-based UUID cell and deduplicated by a content hash computed over the entity's structural fields.

## Design Principles

- **Self-describing** — the target tables are defined by `Pure.RelationalSchema.Self.Schema`, so the meta-model describes itself.
- **Content-addressed** — every entity row is keyed by a hex-encoded structural hash; duplicates across tables are collapsed automatically.
- **AOT-compatible** — the library carries `IsAotCompatible = true` and avoids reflection.

## Dependencies

- [`Pure.RelationalSchema.Self.Schema`](https://github.com/kudima03/Pure.RelationalSchema.Self.Schema/tree/0.1.0-preview.6.0.0) — target table and column definitions for the self-describing schema
- [`Pure.RelationalSchema.Storage`](https://github.com/kudima03/Pure.RelationalSchema.Storage/tree/0.1.0-preview.7.0.0) — `IRow`, `ICell`, `ITable` abstractions and the `Cell` implementation
- [`Pure.RelationalSchema.HashCodes`](https://github.com/kudima03/Pure.RelationalSchema.HashCodes/tree/3.2.0) — content-addressed hashing for schema entities (`TableHash`, `ColumnHash`, `IndexHash`, `ForeignKeyHash`)
- [`Pure.RelationalSchema.Storage.HashCodes`](https://github.com/kudima03/Pure.RelationalSchema.Storage.HashCodes) — hash codes for storage-layer column objects
- [`Pure.Primitives`](https://github.com/kudima03/Pure.Primitives/tree/3.6.2) — immutable `String` and `Ulid` value types used as cell values
- [`Pure.Primitives.String.Operations`](https://github.com/kudima03/Pure.Primitives.String.Operations/tree/1.4.1) — `HexString` conversion for hash-to-string encoding
- [`Pure.Collections.Generic`](https://github.com/kudima03/Pure.Collections.Generic) — `Dictionary<TKey, TValue, TItem>` with custom key/value selectors

## Target Frameworks

- .NET 8
- .NET 9
- .NET 10

## Installation

```
dotnet add package Pure.RelationalSchema.Self.Storage.Projection
```

## Usage

```csharp
ISchema schema = ...; // your ISchema implementation

SchemaProjection projection = new SchemaProjection(schema);

foreach (IGrouping<ITable, IRow> group in projection)
{
    ITable table = group.Key;
    foreach (IRow row in group)
    {
        foreach ((IColumn column, ICell cell) in row.Cells)
        {
            // persist or process each cell
        }
    }
}
```
