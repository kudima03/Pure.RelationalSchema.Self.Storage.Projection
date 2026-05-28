# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All `dotnet` commands must be run from the `./src` directory.

```bash
dotnet restore
dotnet build --no-restore -warnaserror
dotnet format --verify-no-changes             # check code style (CI enforces this)
dotnet format                                 # auto-fix code style
dotnet test --no-build --verbosity normal     # run unit tests
dotnet pack --configuration Release -p:PackageVersion=<version> --output .
```

## Architecture

This is a **projection NuGet library** — no abstractions defined here, no CLI surface. The single responsibility is mapping an `ISchema` (from `Pure.RelationalSchema.Abstractions`) into a set of `IRow` groups ready for storage.

**Public surface:**
- `SchemaProjection(ISchema)` — implements `IEnumerable<IGrouping<ITable, IRow>>`. Enumerates 13 distinct table groupings covering the full schema graph.
- `SchemaEntityProjection(ISchema)` — implements `IRow`. Projects a schema name and a ULID UUID as cells into `SchemasTable`.

**Internal projection classes** (one per entity kind and join table):
`ColumnTypeProjection`, `ColumnProjection`, `TableProjection`, `IndexProjection`, `ForeignKeyProjection`, `TableToColumnProjection`, `IndexToColumnsProjection`, `TableToIndexProjection`, `ForeignKeyToReferencingColumnProjection`, `SchemaToTablesProjection`, `SchemaToForeignKeysProjection`.
All implement `IRow`. Utility: `CellSwitch<T>` dispatches on column identity to select the right cell value; `Grouping` bundles an `ITable` key with its rows.

**Deduplication:** every entity is hashed via `Pure.RelationalSchema.HashCodes` (`TableHash`, `ColumnHash`, `IndexHash`, `ForeignKeyHash`) and stored in a dictionary keyed by `HexString`. Entities with the same structural hash share one row. UUIDs are generated as `Pure.Primitives.Guid.Ulid` at projection time.

**Self-schema dependency:** the table and column definitions that rows are projected into come from `Pure.RelationalSchema.Self.Schema` (e.g. `ColumnsTable`, `TablesTable`, `UuidColumn`, `NameColumn`). Changing that package's schema changes what cells are emitted here.

**Multi-targeting:** net8.0, net9.0, net10.0. All code must remain AOT-compatible (`IsAotCompatible = true`).

**Package validation:** `EnablePackageValidation = true` with baseline `0.1.0-preview.4.0.0`. Any breaking API change fails the build.

**Publishing:** triggered by pushing a semver tag (pattern `*.*.*`). The tag value becomes `PackageVersion`.

## Code Style

Enforced via `.editorconfig` and `dotnet format --verify-no-changes` in CI:

- No `var` — always explicit types.
- No expression-bodied methods or constructors; expression-bodied properties and indexers are required.
- File-scoped namespace declarations.
- Private fields: `_camelCase`.
- No public or non-private instance fields.
- Braces always required, even for single-line blocks.
- Max line length: 90 characters.
- `using` directives outside namespace; system directives sorted first.
- Explicit `new` — never target-typed `new()` when the type is not apparent from context.

## Commit Messages

Do not mention Claude or AI assistance in commit messages.
