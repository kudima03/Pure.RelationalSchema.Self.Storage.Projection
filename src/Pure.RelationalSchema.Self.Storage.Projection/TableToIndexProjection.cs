using Pure.Collections.Generic;
using Pure.HashCodes;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record TableToIndexProjection : IRow
{
    private readonly IDeterminedHash _tableHash;

    private readonly IDeterminedHash _indexHash;

    private readonly IEnumerable<IColumn> _columns;

    public TableToIndexProjection(
        IDeterminedHash tableHash,
        IDeterminedHash indexHash,
        IEnumerable<IColumn> columns
    )
    {
        _tableHash = tableHash;
        _indexHash = indexHash;
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
                        new ReferenceToIndexColumn(),
                        new Cell(new HexString(_indexHash))
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
