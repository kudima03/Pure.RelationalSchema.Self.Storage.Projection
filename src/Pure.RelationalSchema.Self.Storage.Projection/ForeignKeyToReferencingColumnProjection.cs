using Pure.Collections.Generic;
using Pure.Primitives.Guid;
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
using Guid = System.Guid;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record ForeignKeyToReferencingColumnProjection : IRow
{
    private readonly (IForeignKey foreignKey, IColumn column) _entity;

    private readonly IEnumerable<IColumn> _columns;

    public ForeignKeyToReferencingColumnProjection(
        (IForeignKey foreignKey, IColumn column) entity
    )
        : this(entity, new ForeignKeysToReferencingColumnsTable().Columns) { }

    public ForeignKeyToReferencingColumnProjection(
        (IForeignKey foreignKey, IColumn column) entity,
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
                        new ReferenceToColumnColumn(),
                        new Cell(
                            new HexString(
                                new RowHash(new ColumnProjection(_entity.column))
                            )
                        )
                    ),
                    new KeyValuePair<IColumn, ICell>(
                        new ReferenceToForeignKeyColumn(),
                        new Cell(
                            new HexString(
                                new RowHash(new ForeignKeyProjection(_entity.foreignKey))
                            )
                        )
                    ),
                ],
                x => new ColumnHash(x)
            ),
            column => new ColumnHash(column)
        );
}
