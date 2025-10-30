using Pure.Collections.Generic;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests.Models;

public sealed record TableToIndexExpectedRow : IRow
{
    private readonly (ITable table, IIndex index) _source;

    public TableToIndexExpectedRow((ITable table, IIndex index) source)
    {
        _source = source;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells =>
        new Dictionary<KeyValuePair<IColumn, ICell>, IColumn, ICell>(
            [
                new KeyValuePair<IColumn, ICell>(
                    new ReferenceToIndexColumn(),
                    new Cell(
                        new HexString(new RowHash(new IndexExpectedRow(_source.index)))
                    )
                ),
                new KeyValuePair<IColumn, ICell>(
                    new ReferenceToTableColumn(),
                    new Cell(
                        new HexString(new RowHash(new TableExpectedRow(_source.table)))
                    )
                ),
            ],
            x => x.Key,
            x => x.Value,
            x => new ColumnHash(x)
        );
}
