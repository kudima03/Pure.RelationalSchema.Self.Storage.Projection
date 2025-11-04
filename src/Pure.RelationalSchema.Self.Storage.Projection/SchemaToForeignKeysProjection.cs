using Pure.Collections.Generic;
using Pure.HashCodes.Abstractions;
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
    private readonly IDeterminedHash _schemaHash;

    private readonly IDeterminedHash _foreignKeyHash;

    private readonly IEnumerable<IColumn> _columns;

    public SchemaToForeignKeysProjection((ISchema schema, IForeignKey foreignKey) entity)
        : this(entity, new SchemasToForeignKeysTable().Columns) { }

    public SchemaToForeignKeysProjection(
        (ISchema schema, IForeignKey foreignKey) entity,
        IEnumerable<IColumn> columns
    )
        : this(
            new RowHash(new SchemaEntityProjection(entity.schema)),
            new RowHash(new ForeignKeyProjection(entity.foreignKey)),
            columns
        )
    { }

    public SchemaToForeignKeysProjection(
        IDeterminedHash schemaHash,
        IDeterminedHash foreignKeyHash,
        IEnumerable<IColumn> columns
    )
    {
        _schemaHash = schemaHash;
        _foreignKeyHash = foreignKeyHash;
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
                        new Cell(new HexString(_schemaHash))
                    ),
                    new KeyValuePair<IColumn, ICell>(
                        new ReferenceToForeignKeyColumn(),
                        new Cell(new HexString(_foreignKeyHash))
                    ),
                ],
                column => new ColumnHash(column)
            ),
            column => new ColumnHash(column)
        );
}
