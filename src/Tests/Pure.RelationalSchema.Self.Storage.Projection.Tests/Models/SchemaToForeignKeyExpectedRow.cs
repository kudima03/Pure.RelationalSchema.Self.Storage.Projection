using Pure.Collections.Generic;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests.Models;

public sealed record SchemaToForeignKeyExpectedRow : IRow
{
    private readonly (ISchema schema, IForeignKey foreignKey) _source;

    public SchemaToForeignKeyExpectedRow((ISchema schema, IForeignKey foreignKey) source)
    {
        _source = source;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells =>
        new Dictionary<KeyValuePair<IColumn, ICell>, IColumn, ICell>(
            [
                new KeyValuePair<IColumn, ICell>(
                    new ReferenceToForeignKeyColumn(),
                    new Cell(
                        new HexString(
                            new RowHash(new ForeignKeyExpectedRow(_source.foreignKey))
                        )
                    )
                ),
                new KeyValuePair<IColumn, ICell>(
                    new ReferenceToSchemaColumn(),
                    new Cell(
                        new HexString(new RowHash(new SchemaExpectedRow(_source.schema)))
                    )
                ),
            ],
            x => x.Key,
            x => x.Value,
            x => new ColumnHash(x)
        );
}
