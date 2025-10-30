using Pure.Collections.Generic;
using Pure.HashCodes;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests.Models;

public sealed record ForeignKeyExpectedRow : IRow
{
    private readonly IForeignKey _source;

    public ForeignKeyExpectedRow(IForeignKey source)
    {
        _source = source;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells =>
        new Dictionary<KeyValuePair<IColumn, ICell>, IColumn, ICell>(
            [
                new KeyValuePair<IColumn, ICell>(
                    new ReferencingTableColumn(),
                    new Cell(
                        new HexString(
                            new RowHash(new TableExpectedRow(_source.ReferencingTable))
                        )
                    )
                ),
                new KeyValuePair<IColumn, ICell>(
                    new ReferencedTableColumn(),
                    new Cell(
                        new HexString(
                            new RowHash(new TableExpectedRow(_source.ReferencedTable))
                        )
                    )
                ),
                new KeyValuePair<IColumn, ICell>(
                    new CompositionHashColumn(),
                    new Cell(
                        new HexString(
                            new DeterminedHash(
                                [
                                    new DeterminedHash(
                                        _source
                                            .ReferencingColumns.Select(
                                                x => new ColumnExpectedRow(x)
                                            )
                                            .Select(x => new RowHash(x))
                                    ),
                                    new DeterminedHash(
                                        _source
                                            .ReferencedColumns.Select(
                                                x => new ColumnExpectedRow(x)
                                            )
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
