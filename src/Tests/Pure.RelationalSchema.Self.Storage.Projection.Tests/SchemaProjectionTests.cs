using System.Data;
using Pure.HashCodes;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;
using Pure.RelationalSchema.Storage.PostgreSQL;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests;

public sealed record SchemaProjectionTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public SchemaProjectionTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void CreateInsertableRows()
    {
        PostgreSqlStoredSchemaDataSetWithInsertedRows a =
            new PostgreSqlStoredSchemaDataSetWithInsertedRows(
                new PostgreSqlStoredSchemaDataSet(
                    new PostgreSqlCreatedSchema(
                        new RelationalSchemaSchema(),
                        _fixture.Connection
                    ),
                    _fixture.Connection
                ),
                new SchemaProjection(new RelationalSchemaSchema())
            );

        Assert.Equal(145, a.Sum(x => x.Value.Count()));
    }

    [Fact]
    public void ProduceCorrectGroupingTables()
    {
        ISchema schema = new RelationalSchemaSchema();
        Assert.True(
            new DeterminedHash(schema.Tables.Select(x => new TableHash(x))).SequenceEqual(
                new DeterminedHash(
                    new SchemaProjection(schema)
                        .Select(x => x.Key)
                        .Select(x => new TableHash(x))
                )
            )
        );
    }

    [Fact]
    public void ProduceCorrectColumnTypesRowsCount()
    {
        ISchema schema = new RelationalSchemaSchema();
        Assert.Equal(
            schema
                .Tables.SelectMany(x => x.Columns)
                .Select(x => x.Type)
                .DistinctBy(x => new HexString(new ColumnTypeHash(x)).TextValue)
                .Count(),
            new SchemaProjection(schema)
                .Single(x =>
                    new TableHash(x.Key).SequenceEqual(
                        new TableHash(new ColumnTypesTable())
                    )
                )
                .Select(x => new RowHash(x))
                .Count()
        );
    }

    [Fact]
    public void ProduceCorrectColumnsInColumnTypesRows()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new ColumnTypesTable()))
        );

        Assert.True(
            projection.All(row =>
                new DeterminedHash(
                    row.Cells.Keys.Select(column => new ColumnHash(column))
                ).SequenceEqual(
                    new DeterminedHash(
                        projection.Key.Columns.Select(column => new ColumnHash(column))
                    )
                )
            )
        );
    }

    [Fact]
    public void ProduceCorrectColumnsInColumnsRows()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new ColumnsTable()))
        );

        Assert.True(
            projection.All(row =>
                new DeterminedHash(
                    row.Cells.Keys.Select(column => new ColumnHash(column))
                ).SequenceEqual(
                    new DeterminedHash(
                        projection.Key.Columns.Select(column => new ColumnHash(column))
                    )
                )
            )
        );
    }

    [Fact]
    public void ProduceCorrectColumnsRowsCount()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new ColumnsTable()))
        );

        Assert.Equal(
            schema
                .Tables.SelectMany(x => x.Columns)
                .DistinctBy(x => new HexString(new ColumnHash(x)).TextValue)
                .Count(),
            projection.Count()
        );
    }

    [Fact]
    public void ProduceCorrectColumnsInIndexesRows()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new IndexesTable()))
        );

        Assert.True(
            projection.All(row =>
                new DeterminedHash(
                    row.Cells.Keys.Select(column => new ColumnHash(column))
                ).SequenceEqual(
                    new DeterminedHash(
                        projection.Key.Columns.Select(column => new ColumnHash(column))
                    )
                )
            )
        );
    }

    [Fact]
    public void ProduceCorrectIndexesRowsCount()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new IndexesTable()))
        );

        Assert.Equal(
            schema
                .Tables.SelectMany(x => x.Indexes)
                .DistinctBy(x => new HexString(new IndexHash(x)).TextValue)
                .Count(),
            projection.Count()
        );
    }

    [Fact]
    public void ProduceCorrectColumnsInIndexesToColumnsRows()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new IndexesToColumnsTable()))
        );

        Assert.True(
            projection.All(row =>
                new DeterminedHash(
                    row.Cells.Keys.Select(column => new ColumnHash(column))
                ).SequenceEqual(
                    new DeterminedHash(
                        projection.Key.Columns.Select(column => new ColumnHash(column))
                    )
                )
            )
        );
    }

    [Fact]
    public void ProduceCorrectIndexesToColumnsRowsCount()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new IndexesToColumnsTable()))
        );

        Assert.Equal(
            schema.Tables.SelectMany(x => x.Indexes).Sum(x => x.Columns.Count()),
            projection.Count()
        );
    }

    [Fact]
    public void ProduceCorrectColumnsInTablesRows()
    {
        IGrouping<ITable, IRow> projection = new SchemaProjection(
            new RelationalSchemaSchema()
        ).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new TablesTable()))
        );

        Assert.True(
            projection.All(row =>
                new DeterminedHash(
                    row.Cells.Keys.Select(column => new ColumnHash(column))
                ).SequenceEqual(
                    new DeterminedHash(
                        projection.Key.Columns.Select(column => new ColumnHash(column))
                    )
                )
            )
        );
    }

    [Fact]
    public void ProduceCorrectTablesRowsCount()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new TablesTable()))
        );

        Assert.Equal(schema.Tables.Count(), projection.Count());
    }

    [Fact]
    public void ProduceCorrectColumnsInTablesToColumnsRows()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new TablesToColumnsTable()))
        );

        Assert.True(
            projection.All(row =>
                new DeterminedHash(
                    row.Cells.Keys.Select(column => new ColumnHash(column))
                ).SequenceEqual(
                    new DeterminedHash(
                        projection.Key.Columns.Select(column => new ColumnHash(column))
                    )
                )
            )
        );
    }

    [Fact]
    public void ProduceCorrectTablesToColumnsRowsCount()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new TablesToColumnsTable()))
        );

        Assert.Equal(schema.Tables.Sum(x => x.Columns.Count()), projection.Count());
    }

    [Fact]
    public void ProduceCorrectColumnsInTablesToIndexesRows()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new TablesToIndexesTable()))
        );

        Assert.True(
            projection.All(row =>
                new DeterminedHash(
                    row.Cells.Keys.Select(column => new ColumnHash(column))
                ).SequenceEqual(
                    new DeterminedHash(
                        projection.Key.Columns.Select(column => new ColumnHash(column))
                    )
                )
            )
        );
    }

    [Fact]
    public void ProduceCorrectTablesToIndexesRowsCount()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new TablesToIndexesTable()))
        );

        Assert.Equal(schema.Tables.Sum(x => x.Indexes.Count()), projection.Count());
    }

    [Fact]
    public void ProduceCorrectColumnsInSchemasRows()
    {
        IGrouping<ITable, IRow> projection = new SchemaProjection(
            new RelationalSchemaSchema()
        ).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new SchemasTable()))
        );

        Assert.True(
            projection.All(row =>
                new DeterminedHash(
                    row.Cells.Keys.Select(column => new ColumnHash(column))
                ).SequenceEqual(
                    new DeterminedHash(
                        projection.Key.Columns.Select(column => new ColumnHash(column))
                    )
                )
            )
        );
    }

    [Fact]
    public void ProduceCorrectSchemasRowsCount()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new SchemasTable()))
        );

        _ = Assert.Single(projection);
    }

    [Fact]
    public void ProduceCorrectColumnsInForeignKeysRows()
    {
        IGrouping<ITable, IRow> projection = new SchemaProjection(
            new RelationalSchemaSchema()
        ).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new ForeignKeysTable()))
        );

        Assert.True(
            projection.All(row =>
                new DeterminedHash(
                    row.Cells.Keys.Select(column => new ColumnHash(column))
                ).SequenceEqual(
                    new DeterminedHash(
                        projection.Key.Columns.Select(column => new ColumnHash(column))
                    )
                )
            )
        );
    }

    [Fact]
    public void ProduceCorrectForeignKeysRowsCount()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new ForeignKeysTable()))
        );

        Assert.Equal(schema.ForeignKeys.Count(), projection.Count());
    }

    [Fact]
    public void ProduceCorrectColumnsInForeignKeysToReferencingColumnsRows()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(
                new TableHash(new ForeignKeysToReferencingColumnsTable())
            )
        );

        Assert.True(
            projection.All(row =>
                new DeterminedHash(
                    row.Cells.Keys.Select(column => new ColumnHash(column))
                ).SequenceEqual(
                    new DeterminedHash(
                        projection.Key.Columns.Select(column => new ColumnHash(column))
                    )
                )
            )
        );
    }

    [Fact]
    public void ProduceCorrectForeignKeysToReferencingColumnsRowsCount()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(
                new TableHash(new ForeignKeysToReferencingColumnsTable())
            )
        );

        Assert.Equal(
            schema.ForeignKeys.Sum(x => x.ReferencingColumns.Count()),
            projection.Count()
        );
    }

    [Fact]
    public void ProduceCorrectColumnsInForeignKeysToReferencedColumnsRows()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(
                new TableHash(new ForeignKeysToReferencedColumnsTable())
            )
        );

        Assert.True(
            projection.All(row =>
                new DeterminedHash(
                    row.Cells.Keys.Select(column => new ColumnHash(column))
                ).SequenceEqual(
                    new DeterminedHash(
                        projection.Key.Columns.Select(column => new ColumnHash(column))
                    )
                )
            )
        );
    }

    [Fact]
    public void ProduceCorrectForeignKeysToReferencedColumnsRowsCount()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(
                new TableHash(new ForeignKeysToReferencedColumnsTable())
            )
        );

        Assert.Equal(
            schema.ForeignKeys.Sum(x => x.ReferencedColumns.Count()),
            projection.Count()
        );
    }

    [Fact]
    public void ProduceCorrectColumnsInSchemasToForeignKeysRows()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(
                new TableHash(new SchemasToForeignKeysTable())
            )
        );

        Assert.True(
            projection.All(row =>
                new DeterminedHash(
                    row.Cells.Keys.Select(column => new ColumnHash(column))
                ).SequenceEqual(
                    new DeterminedHash(
                        projection.Key.Columns.Select(column => new ColumnHash(column))
                    )
                )
            )
        );
    }

    [Fact]
    public void ProduceCorrectSchemasToForeignKeysRowsCount()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(
                new TableHash(new SchemasToForeignKeysTable())
            )
        );

        Assert.Equal(schema.ForeignKeys.Count(), projection.Count());
    }

    [Fact]
    public void ThrowsExceptionOnGetHashCode()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new SchemaProjection(new RelationalSchemaSchema()).GetHashCode()
        );
    }

    [Fact]
    public void ThrowsExceptionOnToString()
    {
        _ = Assert.Throws<NotSupportedException>(() =>
            new SchemaProjection(new RelationalSchemaSchema()).ToString()
        );
    }
}
