using Pure.Collections.Generic;
using Pure.HashCodes;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests.Models;

public sealed record SchemaExpectedRow : IRow
{
    private readonly ISchema _source;

    public SchemaExpectedRow(ISchema source)
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
                                            .Tables.Select(x => new TableExpectedRow(x))
                                            .Select(x => new RowHash(x))
                                    ),
                                    new DeterminedHash(
                                        _source
                                            .ForeignKeys.Select(
                                                x => new ForeignKeyExpectedRow(x)
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
