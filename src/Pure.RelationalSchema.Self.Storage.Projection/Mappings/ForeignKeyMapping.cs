using Pure.Primitives.Abstractions.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection.Mappings;

internal sealed record ForeignKeyMapping : ICell
{
    private readonly IForeignKey _foreignKey;

    private readonly IColumn _column;

    public ForeignKeyMapping(IForeignKey foreignKey, IColumn column)
    {
        _foreignKey = foreignKey;
        _column = column;
    }

    public IString Value =>
        new CellMapping<IColumn>(
            _column,
            [
                new KeyValuePair<IColumn, ICell>(
                    new ReferencingTableColumn(),
                    new Cell(new HexString(new TableHash(_foreignKey.ReferencingTable)))
                ),
                new KeyValuePair<IColumn, ICell>(
                    new ReferencedTableColumn(),
                    new Cell(new HexString(new TableHash(_foreignKey.ReferencedTable)))
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
