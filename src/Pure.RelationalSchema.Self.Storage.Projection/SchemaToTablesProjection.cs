using Pure.Collections.Generic;
using Pure.HashCodes.Abstractions;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record SchemaToTablesProjection : IRow
{
    private readonly IDeterminedHash _schemaHash;

    private readonly IDeterminedHash _tableHash;

    private readonly IEnumerable<IColumn> _columns;

    public SchemaToTablesProjection(
        IDeterminedHash schemaHash,
        IDeterminedHash tableHash,
        IEnumerable<IColumn> columns
    )
    {
        _schemaHash = schemaHash;
        _tableHash = tableHash;
        _columns = columns;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells =>
        new Dictionary<IColumn, IColumn, ICell>(
            _columns,
            column => column,
            column => new CellSwitch<IColumn>(
                column,
                [
                    new KeyValuePair<IColumn, ICell>(
                        new ReferenceToSchemaColumn(),
                        new Cell(new HexString(_schemaHash))
                    ),
                    new KeyValuePair<IColumn, ICell>(
                        new ReferenceToTableColumn(),
                        new Cell(new HexString(_tableHash))
                    ),
                ],
                column => new ColumnHash(column)
            ),
            column => new ColumnHash(column)
        );
}
