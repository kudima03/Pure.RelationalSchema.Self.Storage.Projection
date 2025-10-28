using Pure.Collections.Generic;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record SchemaToForeignKeysProjection : IRow
{
    private readonly (ISchema schema, IForeignKey foreignKey) _entity;

    private readonly IEnumerable<IColumn> _columns;

    public SchemaToForeignKeysProjection((ISchema schema, IForeignKey foreignKey) entity)
        : this(entity, new SchemasToForeignKeysTable().Columns) { }

    public SchemaToForeignKeysProjection(
        (ISchema schema, IForeignKey foreignKey) entity,
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
                        new ReferenceToSchemaColumn(),
                        new Cell(
                            new HexString(
                                new RowHash(new SchemaEntityProjection(_entity.schema))
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
                column => new ColumnHash(column)
            ),
            column => new ColumnHash(column)
        );
}
