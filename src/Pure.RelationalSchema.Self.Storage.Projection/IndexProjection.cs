using Pure.Collections.Generic;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Self.Storage.Projection.Mappings;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record IndexProjection : IRow
{
    private readonly IIndex _entity;

    private readonly IEnumerable<IColumn> _columns;

    public IndexProjection(IIndex entity)
        : this(entity, new IndexesTable().Columns) { }

    public IndexProjection(IIndex entity, IEnumerable<IColumn> columns)
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
                        new IsUniqueColumn(),
                        new Cell(new String(_entity.IsUnique))
                    ),
                ],
                x => new ColumnHash(x)
            ),
            column => new ColumnHash(column)
        );
}
