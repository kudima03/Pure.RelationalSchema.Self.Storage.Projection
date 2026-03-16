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
using Ulid = Pure.Primitives.Guid.Ulid;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record ForeignKeyProjection : IRow
{
    public ForeignKeyProjection(
        IGuid referenceToReferencingTable,
        IGuid referenceToReferencedTable
    )
        : this(
            new String(referenceToReferencingTable),
            new String(referenceToReferencedTable),
            new ForeignKeysTable().Columns
        )
    { }

    public ForeignKeyProjection(
        IString referenceToReferencingTable,
        IString referenceToReferencedTable
    )
        : this(
            referenceToReferencingTable,
            referenceToReferencedTable,
            new ForeignKeysTable().Columns
        )
    { }

    public ForeignKeyProjection(
        IString referenceToReferencingTable,
        IString referenceToReferencedTable,
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
                            new UuidColumn(),
                            new Cell(new String(new Ulid()))
                        ),
                        new KeyValuePair<IColumn, ICell>(
                            new ReferencingTableColumn(),
                            new Cell(referenceToReferencingTable)
                        ),
                        new KeyValuePair<IColumn, ICell>(
                            new ReferencedTableColumn(),
                            new Cell(referenceToReferencedTable)
                        ),
                    ],
                    column => new ColumnHash(column)
                ),
                column => new ColumnHash(column)
            )
        )
    { }

    private ForeignKeyProjection(IReadOnlyDictionary<IColumn, ICell> cells)
    {
        Cells = cells;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells { get; }
}
