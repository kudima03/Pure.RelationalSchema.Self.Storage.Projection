using Pure.Collections.Generic;
using Pure.Primitives.Guid;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Self.Storage.Projection.Mappings;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;
using Guid = System.Guid;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record TableToIndexProjection : IRow
{
    private readonly (ITable table, IIndex index) _entity;

    private readonly IEnumerable<IColumn> _columns;

    public TableToIndexProjection((ITable table, IIndex index) entity)
        : this(entity, new TablesToIndexesTable().Columns) { }

    public TableToIndexProjection(
        (ITable table, IIndex index) entity,
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
                        new GuidColumn(),
                        new Cell(new String(new Ulid(Guid.CreateVersion7())))
                    ),
                    new KeyValuePair<IColumn, ICell>(
                        new ReferenceToIndexColumn(),
                        new Cell(
                            new HexString(new RowHash(new IndexProjection(_entity.index)))
                        )
                    ),
                    new KeyValuePair<IColumn, ICell>(
                        new ReferenceToTableColumn(),
                        new Cell(
                            new HexString(new RowHash(new TableProjection(_entity.table)))
                        )
                    ),
                ],
                column => new ColumnHash(column)
            ),
            column => new ColumnHash(column)
        );
}
