using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection.Mappings;

internal sealed record TableMapping : ICell
{
    private readonly ITable _table;

    private readonly IColumn _column;

    public TableMapping(ITable table, IColumn column)
    {
        _table = table;
        _column = column;
    }

    public IString Value =>
        new CellMapping<IColumn>(
            _column,
            [new KeyValuePair<IColumn, ICell>(new NameColumn(), new Cell(_table.Name))],
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
