using Pure.Collections.Generic;
using Pure.HashCodes;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record ForeignKeyToReferencingColumnProjection : IRow
{
    private readonly IDeterminedHash _foreignKeyHash;

    private readonly IDeterminedHash _columnHash;

    private readonly IEnumerable<IColumn> _columns;

    public ForeignKeyToReferencingColumnProjection(
        IDeterminedHash foreignKeyHash,
        IDeterminedHash columnHash,
        IEnumerable<IColumn> columns
    )
    {
        _foreignKeyHash = foreignKeyHash;
        _columnHash = columnHash;
        _columns = columns;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells =>
        new Dictionary<IColumn, IColumn, ICell>(
            _columns,
            column => column,
            column => new CellSwitch<IColumn>(
                column,
                [
                    new KeyValuePair<IColumn, ICell>(
                        new ReferenceToColumnColumn(),
                        new Cell(new HexString(_columnHash))
                    ),
                    new KeyValuePair<IColumn, ICell>(
                        new ReferenceToForeignKeyColumn(),
                        new Cell(new HexString(_foreignKeyHash))
                    ),
                ],
                x => new ColumnHash(x)
            ),
            column => new ColumnHash(column)
        );
}
