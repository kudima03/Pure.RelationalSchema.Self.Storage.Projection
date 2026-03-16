using Pure.Collections.Generic;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Guid;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Self.Storage.Projection;

public sealed record SchemaEntityProjection : IRow
{
    public SchemaEntityProjection(ISchema schema)
        : this(schema.Name) { }

    public SchemaEntityProjection(IString name)
        : this(name, new SchemasTable().Columns) { }

    public SchemaEntityProjection(IString name, IEnumerable<IColumn> columns)
        : this(
            new Dictionary<IColumn, IColumn, ICell>(
                columns,
                column => column,
                column => new CellSwitch<IColumn>(
                    column,
                    [
                        new KeyValuePair<IColumn, ICell>(
                            new UuidColumn(),
                            new Cell(new String(new Ulid()))
                        ),
                        new KeyValuePair<IColumn, ICell>(
                            new NameColumn(),
                            new Cell(name)
                        ),
                    ],
                    column => new ColumnHash(column)
                ),
                column => new ColumnHash(column)
            )
        )
    { }

    private SchemaEntityProjection(IReadOnlyDictionary<IColumn, ICell> cells)
    {
        Cells = cells;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells { get; }
}
