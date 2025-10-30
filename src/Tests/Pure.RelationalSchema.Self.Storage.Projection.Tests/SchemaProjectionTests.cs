using System.Data;
using Pure.HashCodes;
using Pure.Primitives.Materialized.String;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema;
using Pure.RelationalSchema.Self.Schema.Tables;
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
            schema
                .Tables.SelectMany(x => x.Columns)
                .Select(x => x.Type)
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
