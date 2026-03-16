using Pure.Collections.Generic;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record ForeignKeyToReferencingColumnProjection : IRow
{
    public ForeignKeyToReferencingColumnProjection(
        IString referenceToColumn,
        IString referenceToForeignKey,
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
                            new ReferenceToForeignKeyColumn(),
                            new Cell(referenceToForeignKey)
                        ),
                    ],
                    x => new ColumnHash(x)
                ),
                column => new ColumnHash(column)
            )
        )
    { }

    private ForeignKeyToReferencingColumnProjection(
        IReadOnlyDictionary<IColumn, ICell> cells
    )
    {
        Cells = cells;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells { get; }
}
