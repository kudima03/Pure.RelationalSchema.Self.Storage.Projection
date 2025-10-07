using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Guid;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Guid = System.Guid;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Self.Storage.Projection.Mappings;

internal sealed record TableToColumnMapping : ICell
{
    private readonly (ITable table, IColumn column) _tuple;

    private readonly IColumn _column;

    public TableToColumnMapping((ITable table, IColumn column) tuple, IColumn column)
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
                    new ReferenceToColumnColumn(),
                    new Cell(new HexString(new ColumnHash(_tuple.column)))
                ),
                new KeyValuePair<IColumn, ICell>(
                    new ReferenceToTableColumn(),
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
