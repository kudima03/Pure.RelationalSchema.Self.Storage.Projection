using System.Collections;
using System.Linq.Expressions;
using Pure.HashCodes;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool;
using Pure.Primitives.Materialized.String;
using Pure.Primitives.Switches.Bool;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ColumnType;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection;

public sealed record SchemasFromRows : IQueryable<ISchema>
{
    private readonly IQueryable<ISchema> _rows;

    public SchemasFromRows(IStoredSchemaDataSet dataset)
    {
        _rows = dataset[new SchemasTable()]
            .Select(x => new SchemaFromRow(new RowHash(x), dataset));
    }

    public Type ElementType => _rows.ElementType;

    public Expression Expression => _rows.Expression;

    public IQueryProvider Provider => _rows.Provider;

    public IEnumerator<ISchema> GetEnumerator()
    {
        return _rows.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal sealed record SchemaFromRow : ISchema
{
    private readonly IRow _row;

    private readonly IStoredSchemaDataSet _schemaDataset;

    public SchemaFromRow(IDeterminedHash rowHash, IStoredSchemaDataSet schemaDataset)
    {
        _row = schemaDataset[new SchemasTable()]
            .Single(x => new RowHash(x).SequenceEqual(rowHash));
        _schemaDataset = schemaDataset;
    }

    public IString Name => _row.Cells[new NameColumn()].Value;

    public IEnumerable<ITable> Tables =>
        new SchemaTables(new RowHash(_row), _schemaDataset).Select(x => new TableFromRow(
            x,
            _schemaDataset
        ));

    public IEnumerable<IForeignKey> ForeignKeys =>
        new SchemaForeignKeys(new RowHash(_row), _schemaDataset).Select(
            x => new ForeignKeyFromRow(x, _schemaDataset)
        );
}

internal sealed record ForeignKeyFromRow : IForeignKey
{
    private readonly IRow _row;

    private readonly IStoredSchemaDataSet _schemaDataset;

    public ForeignKeyFromRow(IDeterminedHash rowHash, IStoredSchemaDataSet schemaDataset)
    {
        _row = schemaDataset[new ForeignKeysTable()]
            .Single(x => new RowHash(x).SequenceEqual(rowHash));
        _schemaDataset = schemaDataset;
    }

    public ITable ReferencingTable =>
        new TableFromRow(
            new HashFromString(_row.Cells[new ReferencingTableColumn()].Value),
            _schemaDataset
        );

    public IEnumerable<IColumn> ReferencingColumns =>
        new ForeignKeyReferencingColumns(new RowHash(_row), _schemaDataset).Select(
            x => new ColumnFromRow(x, _schemaDataset)
        );

    public ITable ReferencedTable =>
        new TableFromRow(
            new HashFromString(_row.Cells[new ReferencedTableColumn()].Value),
            _schemaDataset
        );

    public IEnumerable<IColumn> ReferencedColumns =>
        new ForeignKeyReferencedColumns(new RowHash(_row), _schemaDataset).Select(
            x => new ColumnFromRow(x, _schemaDataset)
        );
}

internal sealed record ForeignKeyReferencedColumns : IEnumerable<IDeterminedHash>
{
    private readonly IEnumerable<IRow> _rows;

    public ForeignKeyReferencedColumns(
        IDeterminedHash foreignKeyHash,
        IStoredSchemaDataSet schemaDataset
    )
    {
        _rows = schemaDataset[new ForeignKeysToReferencedColumnsTable()]
            .Where(x =>
                new HashFromString(
                    x.Cells[new ReferenceToForeignKeyColumn()].Value
                ).SequenceEqual(foreignKeyHash)
            );
    }

    public IEnumerator<IDeterminedHash> GetEnumerator()
    {
        return _rows
            .Select(row => new HashFromString(
                row.Cells[new ReferenceToColumnColumn()].Value
            ))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal sealed record ForeignKeyReferencingColumns : IEnumerable<IDeterminedHash>
{
    private readonly IEnumerable<IRow> _rows;

    public ForeignKeyReferencingColumns(
        IDeterminedHash foreignKeyHash,
        IStoredSchemaDataSet schemaDataset
    )
    {
        _rows = schemaDataset[new ForeignKeysToReferencingColumnsTable()]
            .Where(x =>
                new HashFromString(
                    x.Cells[new ReferenceToForeignKeyColumn()].Value
                ).SequenceEqual(foreignKeyHash)
            );
    }

    public IEnumerator<IDeterminedHash> GetEnumerator()
    {
        return _rows
            .Select(row => new HashFromString(
                row.Cells[new ReferenceToColumnColumn()].Value
            ))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal sealed record SchemaForeignKeys : IEnumerable<IDeterminedHash>
{
    private readonly IEnumerable<IRow> _rows;

    public SchemaForeignKeys(
        IDeterminedHash schemaHash,
        IStoredSchemaDataSet schemaDataset
    )
    {
        _rows = schemaDataset[new SchemasToForeignKeysTable()]
            .Where(x =>
                new HashFromString(
                    x.Cells[new ReferenceToSchemaColumn()].Value
                ).SequenceEqual(schemaHash)
            );
    }

    public IEnumerator<IDeterminedHash> GetEnumerator()
    {
        return _rows
            .Select(row => new HashFromString(
                row.Cells[new ReferenceToForeignKeyColumn()].Value
            ))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal sealed record SchemaTables : IEnumerable<IDeterminedHash>
{
    private readonly IEnumerable<IRow> _rows;

    public SchemaTables(IDeterminedHash schemaHash, IStoredSchemaDataSet schemaDataset)
    {
        _rows = schemaDataset[new SchemasToTablesTable()]
            .Where(x =>
                new HashFromString(
                    x.Cells[new ReferenceToSchemaColumn()].Value
                ).SequenceEqual(schemaHash)
            );
    }

    public IEnumerator<IDeterminedHash> GetEnumerator()
    {
        return _rows
            .Select(row => new HashFromString(
                row.Cells[new ReferenceToTableColumn()].Value
            ))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal sealed record TableFromRow : ITable
{
    private readonly IRow _row;

    private readonly IStoredSchemaDataSet _schemaDataset;

    public TableFromRow(IDeterminedHash rowHash, IStoredSchemaDataSet schemaDataset)
    {
        _row = schemaDataset[new TablesTable()]
            .Single(x => new RowHash(x).SequenceEqual(rowHash));
        _schemaDataset = schemaDataset;
    }

    public IString Name => _row.Cells[new NameColumn()].Value;

    public IEnumerable<IColumn> Columns =>
        new TableColumns(new RowHash(_row), _schemaDataset).Select(x => new ColumnFromRow(
            x,
            _schemaDataset
        ));

    public IEnumerable<IIndex> Indexes =>
        new TableIndexes(new RowHash(_row), _schemaDataset).Select(x => new IndexFromRow(
            x,
            _schemaDataset
        ));
}

internal sealed record IndexFromRow : IIndex
{
    private readonly IRow _row;

    private readonly IStoredSchemaDataSet _schemaDataset;

    public IndexFromRow(IDeterminedHash rowHash, IStoredSchemaDataSet schemaDataset)
    {
        _row = schemaDataset[new IndexesTable()]
            .Single(x => new RowHash(x).SequenceEqual(rowHash));
        _schemaDataset = schemaDataset;
    }

    public IBool IsUnique =>
        new BoolFromString(_row.Cells[new IsUniqueColumn()].Value).Value;

    public IEnumerable<IColumn> Columns =>
        new IndexColumns(new RowHash(_row), _schemaDataset).Select(x => new ColumnFromRow(
            x,
            _schemaDataset
        ));
}

internal sealed record IndexColumns : IEnumerable<IDeterminedHash>
{
    private readonly IEnumerable<IRow> _rows;

    public IndexColumns(IDeterminedHash indexHash, IStoredSchemaDataSet schemaDataset)
    {
        _rows = schemaDataset[new IndexesToColumnsTable()]
            .Where(x =>
                new HashFromString(
                    x.Cells[new ReferenceToIndexColumn()].Value
                ).SequenceEqual(indexHash)
            );
    }

    public IEnumerator<IDeterminedHash> GetEnumerator()
    {
        return _rows
            .Select(row => new HashFromString(
                row.Cells[new ReferenceToColumnColumn()].Value
            ))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal sealed record TableIndexes : IEnumerable<IDeterminedHash>
{
    private readonly IEnumerable<IRow> _rows;

    public TableIndexes(IDeterminedHash tableHash, IStoredSchemaDataSet schemaDataset)
    {
        _rows = schemaDataset[new TablesToIndexesTable()]
            .Where(x =>
                new HashFromString(
                    x.Cells[new ReferenceToTableColumn()].Value
                ).SequenceEqual(tableHash)
            );
    }

    public IEnumerator<IDeterminedHash> GetEnumerator()
    {
        return _rows
            .Select(row => new HashFromString(
                row.Cells[new ReferenceToIndexColumn()].Value
            ))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal sealed record TableColumns : IEnumerable<IDeterminedHash>
{
    private readonly IEnumerable<IRow> _rows;

    public TableColumns(IDeterminedHash tableHash, IStoredSchemaDataSet schemaDataset)
    {
        _rows = schemaDataset[new TablesToColumnsTable()]
            .Where(x =>
                new HashFromString(
                    x.Cells[new ReferenceToTableColumn()].Value
                ).SequenceEqual(tableHash)
            );
    }

    public IEnumerator<IDeterminedHash> GetEnumerator()
    {
        return _rows
            .Select(row => new HashFromString(
                row.Cells[new ReferenceToColumnColumn()].Value
            ))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal sealed record ColumnFromRow : IColumn
{
    private readonly IRow _row;

    private readonly IStoredSchemaDataSet _schemaDataset;

    public ColumnFromRow(IDeterminedHash rowHash, IStoredSchemaDataSet schemaDataset)
    {
        _row = schemaDataset[new ColumnsTable()]
            .Single(x => new RowHash(x).SequenceEqual(rowHash));
        _schemaDataset = schemaDataset;
    }

    public IString Name => _row.Cells[new NameColumn()].Value;

    public IColumnType Type =>
        new ColumnTypeFromRow(
            new HashFromString(_row.Cells[new ReferenceToColumnTypeColumn()].Value),
            _schemaDataset
        );
}

internal sealed record ColumnTypeFromRow : IColumnType
{
    private readonly IRow _row;

    public ColumnTypeFromRow(IDeterminedHash rowHash, IStoredSchemaDataSet schemaDataset)
        : this(
            schemaDataset[new ColumnTypesTable()]
                .Single(x => new RowHash(x).SequenceEqual(rowHash))
        )
    { }

    public ColumnTypeFromRow(IRow row)
    {
        _row = row;
    }

    public IString Name => _row.Cells[new NameColumn()].Value;
}

internal sealed record HashFromString : IDeterminedHash
{
    private readonly IString _hash;

    public HashFromString(IString hash)
    {
        _hash = hash;
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return Convert
            .FromHexString(new MaterializedString(_hash).Value)
            .AsEnumerable()
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal sealed record BoolFromString
{
    private readonly IString _source;

    public BoolFromString(IString source)
    {
        _source = source;
    }

    public IBool Value =>
        new BoolSwitch<IString>(
            _source,
            [
                new KeyValuePair<IString, IBool>(
                    new Primitives.String.String(new True()),
                    new True()
                ),
                new KeyValuePair<IString, IBool>(
                    new Primitives.String.String(new False()),
                    new False()
                ),
            ],
            x => new DeterminedHash(x)
        );
}
