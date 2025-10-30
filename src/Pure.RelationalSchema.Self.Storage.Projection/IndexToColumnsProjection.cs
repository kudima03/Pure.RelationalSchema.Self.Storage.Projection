using Pure.Collections.Generic;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record IndexToColumnsProjection : IRow
{
    private readonly (IIndex index, IColumn column) _entity;

    private readonly IEnumerable<IColumn> _columns;

    public IndexToColumnsProjection((IIndex index, IColumn column) entity)
        : this(entity, new IndexesToColumnsTable().Columns) { }

    public IndexToColumnsProjection(
        (IIndex index, IColumn column) entity,
        IEnumerable<IColumn> columns
    )
    {
        _entity = entity;
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
                        new Cell(
                            new HexString(new RowHash(new IndexProjection(_entity.index)))
                        )
                    ),
                    new KeyValuePair<IColumn, ICell>(
                        new ReferenceToColumnColumn(),
                        new Cell(
                            new HexString(
                                new RowHash(new ColumnProjection(_entity.column))
                            )
                        )
                    ),
                ],
                column => new ColumnHash(column)
            ),
            column => new ColumnHash(column)
        );
}
