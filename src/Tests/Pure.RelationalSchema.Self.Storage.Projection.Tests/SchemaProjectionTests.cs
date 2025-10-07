using Pure.RelationalSchema.Self.Schema;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests;

public sealed record SchemaProjectionTests
{
    [Fact]
    public void CorrectGroupCount()
    {
        Assert.Equal(
            10,
            new SchemaProjection(new RelationalSchemaSchema()).Select(x => x.ToArray()).Count()
        );
    }

    [Fact]
    public void CorrectCellsCount()
    {
        Assert.Equal(254, new SchemaProjection(new RelationalSchemaSchema())
            .SelectMany(x =>
                x.SelectMany(c => c.Cells.Values.Select(v => v.Value)).ToArray()
            )
            .Count());
    }
}
