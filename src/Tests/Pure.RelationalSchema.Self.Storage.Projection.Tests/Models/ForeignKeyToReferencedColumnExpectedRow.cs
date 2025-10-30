using Pure.Collections.Generic;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests.Models;

public sealed record ForeignKeyToReferencedColumnExpectedRow : IRow
{
    private readonly (IForeignKey foreignKey, IColumn column) _source;

    public ForeignKeyToReferencedColumnExpectedRow(
        (IForeignKey foreignKey, IColumn column) source
    )
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
                    new ReferenceToColumnColumn(),
                    new Cell(
                        new HexString(new RowHash(new ColumnExpectedRow(_source.column)))
                    )
                ),
            ],
            x => x.Key,
            x => x.Value,
            x => new ColumnHash(x)
        );
}
