using System.Data;
using Pure.HashCodes;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Self.Storage.Projection.Tests.Models;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests;

public sealed record SchemaProjectionTests
{
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
            schema.Tables.SelectMany(x => x.Columns).Select(x => x.Type).Count(),
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
    public void ProduceCorrectCellsInColumnTypesRows()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new ColumnTypesTable()))
        );

        Assert.True(
            new DeterminedHash(
                schema
                    .Tables.SelectMany(x => x.Columns)
                    .Select(x => x.Type)
                    .Select(x => new ColumnTypeExpectedRow(x))
                    .Select(x => new RowHash(x))
            ).SequenceEqual(new DeterminedHash(projection.Select(x => new RowHash(x))))
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
                .Prepend(new RowDeterminedHashColumn()) //Should contain PK column
                .Count(),
            projection.Count()
        );
    }

    [Fact]
    public void ProduceCorrectCellsInColumnsRows()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new ColumnsTable()))
        );

        Assert.True(
            new DeterminedHash(
                schema
                    .Tables.SelectMany(x => x.Columns)
                    .Prepend(new RowDeterminedHashColumn()) //Should contain PK column
                    .Select(x => new ColumnExpectedRow(x))
                    .Select(x => new RowHash(x))
            ).SequenceEqual(new DeterminedHash(projection.Select(x => new RowHash(x))))
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
            schema.Tables.SelectMany(x => x.Indexes).Count(),
            projection.Count()
        );
    }

    [Fact]
    public void ProduceCorrectCellsInIndexesRows()
    {
        ISchema schema = new RelationalSchemaSchema();
        IGrouping<ITable, IRow> projection = new SchemaProjection(schema).Single(x =>
            new TableHash(x.Key).SequenceEqual(new TableHash(new IndexesTable()))
        );

        Assert.True(
            new DeterminedHash(
                schema
                    .Tables.SelectMany(x => x.Indexes)
                    .Select(x => new IndexExpectedRow(x))
                    .Select(x => new RowHash(x))
            ).SequenceEqual(new DeterminedHash(projection.Select(x => new RowHash(x))))
        );
    }

    [Fact]
    public void CorrectGroupCount()
    {
        Assert.Equal(
            13,
            new SchemaProjection(new RelationalSchemaSchema())
                .Select(x => x.ToArray())
                .Count()
        );
    }

    [Fact]
    public void CorrectCellsCount()
    {
        Assert.Equal(
            339,
            new SchemaProjection(new RelationalSchemaSchema())
                .SelectMany(x =>
                    x.SelectMany(c => c.Cells.Values.Select(v => v.Value)).ToArray()
                )
                .Count()
        );
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
