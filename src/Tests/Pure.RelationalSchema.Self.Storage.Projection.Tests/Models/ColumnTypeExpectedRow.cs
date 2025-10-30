using Pure.Collections.Generic;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ColumnType;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests.Models;

public sealed record ColumnTypeExpectedRow : IRow
{
    private readonly IColumnType _source;

    public ColumnTypeExpectedRow(IColumnType source)
    {
        _source = source;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells =>
        new Dictionary<KeyValuePair<IColumn, ICell>, IColumn, ICell>(
            [new KeyValuePair<IColumn, ICell>(new NameColumn(), new Cell(_source.Name))],
            x => x.Key,
            x => x.Value,
            x => new ColumnHash(x)
        );
}
