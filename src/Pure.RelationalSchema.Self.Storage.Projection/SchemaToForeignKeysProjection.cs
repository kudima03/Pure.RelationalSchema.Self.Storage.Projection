using Pure.Collections.Generic;
using Pure.Primitives.Abstractions.Guid;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record SchemaToForeignKeysProjection : IRow
{
    public SchemaToForeignKeysProjection(
        IGuid referenceToSchema,
        IGuid referenceToForeignKey
    )
        : this(new String(referenceToSchema), new String(referenceToForeignKey)) { }

    public SchemaToForeignKeysProjection(
        IString referenceToSchema,
        IString referenceToForeignKey
    )
        : this(
            referenceToSchema,
            referenceToForeignKey,
            new SchemasToForeignKeysTable().Columns
        )
    { }

    public SchemaToForeignKeysProjection(
        IString referenceToSchema,
        IString referenceToForeignKey,
        IEnumerable<IColumn> columns
    )
        : this(
            new Dictionary<IColumn, IColumn, ICell>(
                columns,
                column => column,
                column => new CellSwitch<IColumn>(
                    column,
                    [
                        new KeyValuePair<IColumn, ICell>(
                            new ReferenceToSchemaColumn(),
                            new Cell(referenceToSchema)
                        ),
                        new KeyValuePair<IColumn, ICell>(
                            new ReferenceToForeignKeyColumn(),
                            new Cell(referenceToForeignKey)
                        ),
                    ],
                    column => new ColumnHash(column)
                ),
                column => new ColumnHash(column)
            )
        )
    { }

    private SchemaToForeignKeysProjection(IReadOnlyDictionary<IColumn, ICell> cells)
    {
        Cells = cells;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells { get; }
}
