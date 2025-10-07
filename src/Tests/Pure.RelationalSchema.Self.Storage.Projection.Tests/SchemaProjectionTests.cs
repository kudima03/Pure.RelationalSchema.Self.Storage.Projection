using Pure.RelationalSchema.Random;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests;

public sealed record SchemaProjectionTests
{
    [Fact]
    public void Enumerates()
    {
        Assert.Equal(
            10,
            new SchemaProjection(new RandomSchema()).Select(x => x.ToArray()).Count()
        );
    }
}
