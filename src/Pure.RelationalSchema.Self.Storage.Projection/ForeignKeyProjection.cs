using Pure.Collections.Generic;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Self.Storage.Projection.Mappings;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record ForeignKeyProjection : IRow
{
    private readonly IForeignKey _entity;

    private readonly IEnumerable<IColumn> _columns;

    public ForeignKeyProjection(IForeignKey entity)
        : this(entity, new ForeignKeysTable().Columns) { }

    public ForeignKeyProjection(IForeignKey entity, IEnumerable<IColumn> columns)
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
                        new ReferencingTableColumn(),
                        new Cell(
                            new HexString(
                                new RowHash(new TableProjection(_entity.ReferencingTable))
                            )
                        )
                    ),
                    new KeyValuePair<IColumn, ICell>(
                        new ReferencedTableColumn(),
                        new Cell(
                            new HexString(
                                new RowHash(new TableProjection(_entity.ReferencedTable))
                            )
                        )
                    ),
                ],
                column => new ColumnHash(column)
            ),
            column => new ColumnHash(column)
        );
}
