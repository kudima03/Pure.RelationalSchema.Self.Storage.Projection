using Pure.Collections.Generic;
using Pure.HashCodes;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests.Models;

public sealed record IndexExpectedRow : IRow
{
    private readonly IIndex _source;

    public IndexExpectedRow(IIndex source)
    {
        _source = source;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells =>
        new Dictionary<KeyValuePair<IColumn, ICell>, IColumn, ICell>(
            [
                new KeyValuePair<IColumn, ICell>(
                    new IsUniqueColumn(),
                    new Cell(new String(_source.IsUnique))
                ),
                new KeyValuePair<IColumn, ICell>(
                    new CompositionHashColumn(),
                    new Cell(
                        new HexString(
                            new DeterminedHash(
                                _source
                                    .Columns.Select(x => new ColumnExpectedRow(x))
                                    .Select(x => new RowHash(x))
                            )
                        )
                    )
                ),
            ],
            x => x.Key,
            x => x.Value,
            x => new ColumnHash(x)
        );
}
