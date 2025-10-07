using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection.Mappings;

internal sealed record ColumnMapping : ICell
{
    private readonly IColumn _columnEntity;

    private readonly IColumn _column;

    public ColumnMapping(IColumn columnEntity, IColumn column)
    {
        _columnEntity = columnEntity;
        _column = column;
    }

    public IString Value =>
        new CellMapping<IColumn>(
            _column,
            [
                new KeyValuePair<IColumn, ICell>(
                    new NameColumn(),
                    new Cell(_columnEntity.Name)
                ),
                new KeyValuePair<IColumn, ICell>(
                    new ReferenceToColumnTypeColumn(),
                    new Cell(new HexString(new ColumnTypeHash(_columnEntity.Type)))
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
