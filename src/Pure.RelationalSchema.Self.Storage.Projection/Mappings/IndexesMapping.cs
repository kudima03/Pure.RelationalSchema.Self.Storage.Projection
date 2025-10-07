using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Self.Storage.Projection.Mappings;

internal sealed record IndexesMapping : ICell
{
    private readonly IIndex _index;

    private readonly IColumn _column;

    public IndexesMapping(IIndex index, IColumn column)
    {
        _index = index;
        _column = column;
    }

    public IString Value =>
        new CellMapping<IColumn>(
            _column,
            [
                new KeyValuePair<IColumn, ICell>(
                    new IsUniqueColumn(),
                    new Cell(new String(_index.IsUnique))
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
