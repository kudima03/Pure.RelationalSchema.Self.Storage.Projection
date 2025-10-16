using System.Collections;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.Column;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection;

public sealed record SchemaProjection : IEnumerable<IGrouping<ITable, IRow>>
{
    private readonly ISchema _schema;

    public SchemaProjection(ISchema schema)
    {
        _schema = schema;
    }

    public IEnumerator<IGrouping<ITable, IRow>> GetEnumerator()
    {
        IReadOnlyCollection<IRow> columnTypesRows =
        [
            .. _schema
                .Tables.SelectMany(x => x.Columns.Select(c => c.Type))
                .Select(x => new ColumnTypeProjection(x)),
        ];

        if (columnTypesRows.Count != 0)
        {
            yield return new Grouping(new ColumnTypesTable(), columnTypesRows);
        }

        IReadOnlyCollection<IRow> columnsRows =
        [
            .. _schema
                .Tables.SelectMany(x => x.Columns)
                .Prepend(new RowDeterminedHashColumn())
                .Select(x => new ColumnProjection(x)),
        ];

        if (columnsRows.Count != 0)
        {
            yield return new Grouping(new ColumnsTable(), columnsRows);
        }

        IReadOnlyCollection<IRow> tablesRows =
        [
            .. _schema.Tables.Select(x => new TableProjection(x)),
        ];

        if (tablesRows.Count != 0)
        {
            yield return new Grouping(new TablesTable(), tablesRows);
        }

        IReadOnlyCollection<IRow> tablesToColumnsRows =
        [
            .. _schema
                .Tables.SelectMany(
                    table => table.Columns,
                    (table, column) => (table, column)
                )
                .Select(x => new TableToColumnProjection(x)),
        ];

        if (tablesToColumnsRows.Count != 0)
        {
            yield return new Grouping(new TablesToColumnsTable(), tablesToColumnsRows);
        }

        IReadOnlyCollection<IRow> indexesRows =
        [
            .. _schema
                .Tables.SelectMany(x => x.Indexes)
                .Select(x => new IndexProjection(x)),
        ];

        if (indexesRows.Count != 0)
        {
            yield return new Grouping(new IndexesTable(), indexesRows);
        }

        IReadOnlyCollection<IRow> tablesToIndexesRows =
        [
            .. _schema
                .Tables.SelectMany(
                    table => table.Indexes,
                    (table, index) => (table, index)
                )
                .Select(x => new TableToIndexProjection(x)),
        ];

        if (tablesToIndexesRows.Count != 0)
        {
            yield return new Grouping(new TablesToIndexesTable(), tablesToIndexesRows);
        }

        IReadOnlyCollection<IRow> foreignKeysRows =
        [
            .. _schema.ForeignKeys.Select(x => new ForeignKeyProjection(x)),
        ];

        if (foreignKeysRows.Count != 0)
        {
            yield return new Grouping(new ForeignKeysTable(), foreignKeysRows);
        }

        IReadOnlyCollection<IRow> foreignKeysToReferencingColumnsRows =
        [
            .. _schema
                .ForeignKeys.SelectMany(
                    fk => fk.ReferencingColumns,
                    (fk, col) => (fk, col)
                )
                .Select(x => new ForeignKeyToReferencingColumnProjection(x)),
        ];

        if (foreignKeysToReferencingColumnsRows.Count != 0)
        {
            yield return new Grouping(
                new ForeignKeysToReferencingColumnsTable(),
                foreignKeysToReferencingColumnsRows
            );
        }

        IReadOnlyCollection<IRow> foreignKeysToReferencedColumnsRows =
        [
            .. _schema
                .ForeignKeys.SelectMany(
                    fk => fk.ReferencedColumns,
                    (fk, col) => (fk, col)
                )
                .Select(x => new ForeignKeyToReferencingColumnProjection(x)),
        ];

        if (foreignKeysToReferencedColumnsRows.Count != 0)
        {
            yield return new Grouping(
                new ForeignKeysToReferencedColumnsTable(),
                foreignKeysToReferencedColumnsRows
            );
        }

        IReadOnlyCollection<IRow> schemaRows = [new SchemaEntityProjection(_schema)];

        if (schemaRows.Count != 0)
        {
            yield return new Grouping(new SchemasTable(), schemaRows);
        }

        IReadOnlyCollection<IRow> schemasToTablesRows =
        [
            .. _schema
                .Tables.Select(table => (_schema, table))
                .Select(x => new SchemaToTablesProjection(x)),
        ];

        if (schemasToTablesRows.Count != 0)
        {
            yield return new Grouping(new SchemasToTablesTable(), schemasToTablesRows);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        throw new NotSupportedException();
    }
}
