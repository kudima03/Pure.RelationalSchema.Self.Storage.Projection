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
        yield return new Grouping(
            new ColumnTypesTable(),
            _schema
                .Tables.SelectMany(x => x.Columns.Select(c => c.Type))
                .Select(x => new ColumnTypeProjection(x))
        );

        yield return new Grouping(
            new ColumnsTable(),
            _schema
                .Tables.SelectMany(x => x.Columns)
                .Prepend(new RowDeterminedHashColumn())
                .Select(x => new ColumnProjection(x))
        );

        yield return new Grouping(
            new TablesTable(),
            _schema.Tables.Select(x => new TableProjection(x))
        );

        yield return new Grouping(
            new TablesToColumnsTable(),
            _schema.Tables.SelectMany(x => new TablesToColumnsProjections(x))
        );

        yield return new Grouping(
            new IndexesTable(),
            _schema.Tables.SelectMany(x => x.Indexes).Select(x => new IndexProjection(x))
        );

        yield return new Grouping(
            new IndexesToColumnsTable(),
            _schema.Tables.SelectMany(table =>
                table.Indexes.SelectMany(x => new IndexesToColumnsProjections(x))
            )
        );

        yield return new Grouping(
            new TablesToIndexesTable(),
            _schema.Tables.SelectMany(x => new TablesToIndexesProjections(x))
        );

        yield return new Grouping(
            new ForeignKeysTable(),
            _schema.ForeignKeys.Select(x => new ForeignKeyProjection(x))
        );

        yield return new Grouping(
            new ForeignKeysToReferencingColumnsTable(),
            _schema.ForeignKeys.SelectMany(
                fk => new ForeignKeysToReferencingColumnsProjections(fk)
            )
        );

        yield return new Grouping(
            new ForeignKeysToReferencedColumnsTable(),
            _schema.ForeignKeys.SelectMany(
                fk => new ForeignKeysToReferencedColumnsProjections(fk)
            )
        );

        yield return new Grouping(
            new SchemasTable(),
            [new SchemaEntityProjection(_schema)]
        );

        yield return new Grouping(
            new SchemasToTablesTable(),
            new SchemasToTablesProjections(_schema)
        );

        yield return new Grouping(
            new SchemasToForeignKeysTable(),
            new SchemaToForeignKeysProjections(_schema)
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
