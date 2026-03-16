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

internal sealed record ColumnProjection : IRow
{
    public ColumnProjection(IColumn column, IGuid referenceToColumnType)
        : this(column.Name, referenceToColumnType) { }

    public ColumnProjection(IColumn column, IString referenceToColumnType)
        : this(column.Name, referenceToColumnType) { }

    public ColumnProjection(IString name, IGuid referenceToColumnType)
        : this(name, new String(referenceToColumnType)) { }

    public ColumnProjection(IString name, IString referenceToColumnType)
        : this(name, referenceToColumnType, new ColumnsTable().Columns) { }

    public ColumnProjection(
        IString name,
        IString referenceToColumnType,
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
                            new NameColumn(),
                            new Cell(name)
                        ),
                        new KeyValuePair<IColumn, ICell>(
                            new ReferenceToColumnTypeColumn(),
                            new Cell(referenceToColumnType)
                        ),
                    ],
                    column => new ColumnHash(column)
                ),
                column => new ColumnHash(column)
            )
        )
    { }

    private ColumnProjection(IReadOnlyDictionary<IColumn, ICell> cells)
    {
        Cells = cells;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells { get; }
}
