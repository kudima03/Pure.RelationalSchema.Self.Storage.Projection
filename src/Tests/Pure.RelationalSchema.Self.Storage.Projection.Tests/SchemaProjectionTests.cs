using System.Data;
using Pure.HashCodes;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema;

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
