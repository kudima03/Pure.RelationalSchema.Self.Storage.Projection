using Pure.Collections.Generic;
using Pure.HashCodes;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests.Models;

public sealed record TableExpectedRow : IRow
{
    private readonly ITable _source;

    public TableExpectedRow(ITable source)
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
                    new CompositionHashColumn(),
                    new Cell(
                        new HexString(
                            new DeterminedHash(
                                [
                                    new DeterminedHash(
                                        _source
                                            .Columns.Select(x => new ColumnExpectedRow(x))
                                            .Select(x => new RowHash(x))
                                    ),
                                    new DeterminedHash(
                                        _source
                                            .Indexes.Select(x => new IndexExpectedRow(x))
                                            .Select(x => new RowHash(x))
                                    ),
                                ]
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
