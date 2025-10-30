using Pure.Collections.Generic;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests.Models;

public sealed record IndexToColumnExpectedRow : IRow
{
    private readonly (IIndex index, IColumn column) _source;

    public IndexToColumnExpectedRow((IIndex index, IColumn column) source)
    {
        _source = source;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells =>
        new Dictionary<KeyValuePair<IColumn, ICell>, IColumn, ICell>(
            [
                new KeyValuePair<IColumn, ICell>(
                    new ReferenceToColumnColumn(),
                    new Cell(new HexString(new ColumnHash(_source.column)))
                ),
                new KeyValuePair<IColumn, ICell>(
                    new ReferenceToIndexColumn(),
                    new Cell(new HexString(new IndexHash(_source.index)))
                ),
            ],
            x => x.Key,
            x => x.Value,
            x => new ColumnHash(x)
        );
}
