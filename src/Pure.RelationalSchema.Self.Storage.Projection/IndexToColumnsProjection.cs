using Pure.Collections.Generic;
using Pure.Primitives.Abstractions.Guid;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record IndexToColumnsProjection : IRow
{
    public IndexToColumnsProjection(IGuid referenceToIndex, IGuid referenceToColumn)
        : this(new String(referenceToIndex), new String(referenceToColumn)) { }

    public IndexToColumnsProjection(IString referenceToIndex, IString referenceToColumn)
        : this(referenceToIndex, referenceToColumn, new IndexesToColumnsTable().Columns)
    { }

    public IndexToColumnsProjection(
        IString referenceToIndex,
        IString referenceToColumn,
        IEnumerable<IColumn> columns
    )
        : this(
            new Dictionary<IColumn, IColumn, ICell>(
                columns,
                column => column,
                column => new CellSwitch<IColumn>(
                    column,
                    [
                        new KeyValuePair<IColumn, ICell>(
                            new ReferenceToIndexColumn(),
                            new Cell(referenceToIndex)
                        ),
                        new KeyValuePair<IColumn, ICell>(
                            new ReferenceToColumnColumn(),
                            new Cell(referenceToColumn)
                        ),
                    ],
                    column => new ColumnHash(column)
                ),
                column => new ColumnHash(column)
            )
        )
    { }

    private IndexToColumnsProjection(IReadOnlyDictionary<IColumn, ICell> cells)
    {
        Cells = cells;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells { get; }
}
