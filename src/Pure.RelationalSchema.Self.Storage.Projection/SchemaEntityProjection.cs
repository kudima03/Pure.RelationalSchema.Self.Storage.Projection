using Pure.Collections.Generic;
using Pure.HashCodes;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection;

public sealed record SchemaEntityProjection : IRow
{
    private readonly ISchema _entity;

    private readonly IEnumerable<IColumn> _columns;

    public SchemaEntityProjection(ISchema entity)
        : this(entity, new SchemasTable().Columns) { }

    public SchemaEntityProjection(ISchema entity, IEnumerable<IColumn> columns)
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
                        new NameColumn(),
                        new Cell(_entity.Name)
                    ),
                    new KeyValuePair<IColumn, ICell>(
                        new CompositionHashColumn(),
                        new Cell(
                            new HexString(
                                new DeterminedHash(
                                    [
                                        new DeterminedHash(
                                            _entity.Tables.Select(x => new RowHash(
                                                new TableProjection(x)
                                            ))
                                        ),
                                        new DeterminedHash(
                                            _entity.ForeignKeys.Select(x => new RowHash(
                                                new ForeignKeyProjection(x)
                                            ))
                                        ),
                                    ]
                                )
                            )
                        )
                    ),
                ],
                column => new ColumnHash(column)
            ),
            column => new ColumnHash(column)
        );
}
