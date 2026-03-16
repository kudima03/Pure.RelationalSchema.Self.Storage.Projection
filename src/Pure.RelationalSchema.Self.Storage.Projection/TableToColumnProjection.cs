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

internal sealed record TableToColumnProjection : IRow
{
    public TableToColumnProjection(IGuid referenceToColumn, IGuid referenceToTable)
        : this(new String(referenceToColumn), new String(referenceToTable)) { }

    public TableToColumnProjection(IString referenceToColumn, IString referenceToTable)
        : this(referenceToColumn, referenceToTable, new TablesToColumnsTable().Columns)
    { }

    public TableToColumnProjection(
        IString referenceToColumn,
        IString referenceToTable,
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
                            new ReferenceToColumnColumn(),
                            new Cell(referenceToColumn)
                        ),
                        new KeyValuePair<IColumn, ICell>(
                            new ReferenceToTableColumn(),
                            new Cell(referenceToTable)
                        ),
                    ],
                    column => new ColumnHash(column)
                ),
                column => new ColumnHash(column)
            )
        )
    { }

    private TableToColumnProjection(IReadOnlyDictionary<IColumn, ICell> cells)
    {
        Cells = cells;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells { get; }
}
