using Pure.Collections.Generic;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Guid;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record IndexProjection : IRow
{
    public IndexProjection(IIndex entity)
        : this(entity.IsUnique) { }

    public IndexProjection(IBool isUnique)
        : this(isUnique, new IndexesTable().Columns) { }

    public IndexProjection(IBool isUnique, IEnumerable<IColumn> columns)
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
                            new IsUniqueColumn(),
                            new Cell(new String(isUnique))
                        ),
                    ],
                    x => new ColumnHash(x)
                ),
                column => new ColumnHash(column)
            )
        )
    { }

    private IndexProjection(IReadOnlyDictionary<IColumn, ICell> cells)
    {
        Cells = cells;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells { get; }
}
