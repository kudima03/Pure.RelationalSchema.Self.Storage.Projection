using Pure.Collections.Generic;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ColumnType;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using String = Pure.Primitives.String.String;
using Ulid = Pure.Primitives.Guid.Ulid;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record ColumnTypeProjection : IRow
{
    public ColumnTypeProjection(IColumnType entity)
        : this(entity.Name, new ColumnTypesTable().Columns) { }

    public ColumnTypeProjection(IString name, IEnumerable<IColumn> columns)
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
                    ],
                    column => new ColumnHash(column)
                ),
                column => new ColumnHash(column)
            )
        )
    { }

    private ColumnTypeProjection(IReadOnlyDictionary<IColumn, ICell> cells)
    {
        Cells = cells;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells { get; }
}
