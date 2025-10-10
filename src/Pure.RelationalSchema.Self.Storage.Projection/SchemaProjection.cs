using System.Collections;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ColumnType;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Self.Storage.Projection.Mappings;
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
        yield return new Grouping(
            new ColumnTypesTable(),
            new ProjectionOnRows<IColumnType>(
                new ColumnTypesTable(),
                _schema.Tables.SelectMany(x => x.Columns.Select(c => c.Type)),
                (column, entity) => new ColumnTypeMapping(entity, column)
            )
        );

        yield return new Grouping(
            new ColumnsTable(),
            new ProjectionOnRows<IColumn>(
                new ColumnsTable(),
                _schema.Tables.SelectMany(x => x.Columns),
                (column, entity) => new ColumnMapping(entity, column)
            )
        );

        yield return new Grouping(
            new TablesTable(),
            new ProjectionOnRows<ITable>(
                new TablesTable(),
                _schema.Tables,
                (column, entity) => new TableMapping(entity, column)
            )
        );

        yield return new Grouping(
            new TablesToColumnsTable(),
            new ProjectionOnRows<(ITable, IColumn)>(
                new TablesToColumnsTable(),
                _schema.Tables.SelectMany(
                    table => table.Columns,
                    (table, column) => (table, column)
                ),
                (column, entity) => new TableToColumnMapping(entity, column)
            )
        );

        yield return new Grouping(
            new IndexesTable(),
            new ProjectionOnRows<IIndex>(
                new IndexesTable(),
                _schema.Tables.SelectMany(x => x.Indexes),
                (column, entity) => new IndexesMapping(entity, column)
            )
        );

        yield return new Grouping(
            new TablesToIndexesTable(),
            new ProjectionOnRows<(ITable, IIndex)>(
                new TablesToIndexesTable(),
                _schema.Tables.SelectMany(
                    table => table.Indexes,
                    (table, index) => (table, index)
                ),
                (column, entity) => new TablesToIndexesMapping(entity, column)
            )
        );

        yield return new Grouping(
            new ForeignKeysTable(),
            new ProjectionOnRows<IForeignKey>(
                new ForeignKeysTable(),
                _schema.ForeignKeys,
                (column, entity) => new ForeignKeyMapping(entity, column)
            )
        );

        yield return new Grouping(
            new ForeignKeysToReferencingColumnsTable(),
            new ProjectionOnRows<(IForeignKey, IColumn)>(
                new ForeignKeysToReferencingColumnsTable(),
                _schema.ForeignKeys.SelectMany(
                    table => table.ReferencingColumns,
                    (table, index) => (table, index)
                ),
                (column, entity) =>
                    new ForeignKeysToReferencingColumnsMapping(entity, column)
            )
        );

        yield return new Grouping(
            new ForeignKeysToReferencedColumnsTable(),
            new ProjectionOnRows<(IForeignKey, IColumn)>(
                new ForeignKeysToReferencedColumnsTable(),
                _schema.ForeignKeys.SelectMany(
                    table => table.ReferencedColumns,
                    (table, index) => (table, index)
                ),
                (column, entity) =>
                    new ForeignKeysToReferencingColumnsMapping(entity, column)
            )
        );

        yield return new Grouping(
            new SchemasTable(),
            new ProjectionOnRows<ISchema>(
                new SchemasTable(),
                [_schema],
                (column, entity) => new SchemasMapping(entity, column)
            )
        );

        yield return new Grouping(
            new SchemasToTablesTable(),
            new ProjectionOnRows<(ISchema, ITable)>(
                new SchemasToTablesTable(),
                _schema.Tables.Select(table => (_schema, table)),
                (column, entity) => new SchemasToTablesMapping(entity, column)
            )
        );
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
