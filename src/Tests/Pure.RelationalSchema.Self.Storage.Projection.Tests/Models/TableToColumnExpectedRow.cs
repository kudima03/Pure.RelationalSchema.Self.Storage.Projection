using Pure.Collections.Generic;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests.Models;

public sealed record TableToColumnExpectedRow : IRow
{
    private readonly (ITable table, IColumn column) _source;

    public TableToColumnExpectedRow((ITable table, IColumn column) source)
    {
        _source = source;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells =>
        new Dictionary<KeyValuePair<IColumn, ICell>, IColumn, ICell>(
            [
                new KeyValuePair<IColumn, ICell>(
                    new ReferenceToColumnColumn(),
                    new Cell(
                        new HexString(new RowHash(new ColumnExpectedRow(_source.column)))
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
