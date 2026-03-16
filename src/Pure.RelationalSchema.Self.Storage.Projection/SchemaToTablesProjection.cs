using Pure.Collections.Generic;
using Pure.Primitives.Abstractions.Guid;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record SchemaToTablesProjection : IRow
{
    public SchemaToTablesProjection(IGuid referenceToSchema, IGuid referenceToTable)
        : this(referenceToSchema, referenceToTable, new SchemasToTablesTable().Columns)
    { }

    public SchemaToTablesProjection(
        IGuid referenceToSchema,
        IGuid referenceToTable,
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
                            new Cell(new String(referenceToSchema))
                        ),
                        new KeyValuePair<IColumn, ICell>(
                            new ReferenceToTableColumn(),
                            new Cell(new String(referenceToTable))
                        ),
                    ],
                    column => new ColumnHash(column)
                ),
                column => new ColumnHash(column)
            )
        )
    { }

    private SchemaToTablesProjection(IReadOnlyDictionary<IColumn, ICell> cells)
    {
        Cells = cells;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells { get; }
}
