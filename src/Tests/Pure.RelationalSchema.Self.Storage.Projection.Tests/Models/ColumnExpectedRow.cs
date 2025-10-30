using Pure.Collections.Generic;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests.Models;

public sealed record ColumnExpectedRow : IRow
{
    private readonly IColumn _source;

    public ColumnExpectedRow(IColumn source)
    {
        _source = source;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells =>
        new Dictionary<KeyValuePair<IColumn, ICell>, IColumn, ICell>(
            [
                new KeyValuePair<IColumn, ICell>(
                    new NameColumn(),
                    new Cell(_source.Name)
                ),
                new KeyValuePair<IColumn, ICell>(
                    new ReferenceToColumnTypeColumn(),
                    new Cell(
                        new HexString(
                            new RowHash(new ColumnTypeExpectedRow(_source.Type))
                        )
                    )
                ),
            ],
            x => x.Key,
            x => x.Value,
            x => new ColumnHash(x)
        );
}
