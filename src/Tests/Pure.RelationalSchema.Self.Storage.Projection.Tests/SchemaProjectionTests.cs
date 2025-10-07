using Pure.RelationalSchema.Random;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests;

public sealed record SchemaProjectionTests
{
    [Fact]
    public void CorrectGroupCount()
    {
        Assert.Equal(
            10,
            new SchemaProjection(new RandomSchema()).Select(x => x.ToArray()).Count()
        );
    }

    [Fact]
    public void EnumerateCells()
    {
        int count = new SchemaProjection(new RandomSchema())
            .SelectMany(x => x.SelectMany(c => c.Cells.Values.Select(v => v.Value)).ToArray()).Count();
        Assert.NotEqual(0, count);
    }
}
