using Pure.Collections.Generic;
using Pure.HashCodes.Abstractions;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record TableToColumnProjection : IRow
{
    private readonly IDeterminedHash _tableHash;

    private readonly IDeterminedHash _columnHash;

    private readonly IEnumerable<IColumn> _columns;

    public TableToColumnProjection(
        IDeterminedHash tableHash,
        IDeterminedHash columnHash,
        IEnumerable<IColumn> columns
    )
    {
        _tableHash = tableHash;
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
                        new ReferenceToTableColumn(),
                        new Cell(new HexString(_tableHash))
                    ),
                ],
                column => new ColumnHash(column)
            ),
            column => new ColumnHash(column)
        );
}
