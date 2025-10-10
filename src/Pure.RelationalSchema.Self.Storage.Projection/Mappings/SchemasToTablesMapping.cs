using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Guid;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Guid = System.Guid;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Self.Storage.Projection.Mappings;

internal sealed record SchemasToTablesMapping : ICell
{
    private readonly (ISchema schema, ITable table) _tuple;

    private readonly IColumn _column;

    public SchemasToTablesMapping((ISchema schema, ITable table) tuple, IColumn column)
    {
        _tuple = tuple;
        _column = column;
    }

    public IString Value =>
        new CellMapping<IColumn>(
            _column,
            [
                new KeyValuePair<IColumn, ICell>(
                    new GuidColumn(),
                    new Cell(new String(new Ulid(Guid.CreateVersion7())))
                ),
                new KeyValuePair<IColumn, ICell>(
                    new ReferenceToTableColumn(),
                    new Cell(new HexString(new SchemaHash(_tuple.schema)))
                ),
                new KeyValuePair<IColumn, ICell>(
                    new ReferenceToSchemaColumn(),
                    new Cell(new HexString(new TableHash(_tuple.table)))
                ),
            ],
            x => new ColumnHash(x)
        ).Value;

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        throw new NotSupportedException();
    }
}
