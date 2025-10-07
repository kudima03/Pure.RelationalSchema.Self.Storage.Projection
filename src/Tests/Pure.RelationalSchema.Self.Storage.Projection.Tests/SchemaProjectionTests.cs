using Pure.RelationalSchema.Random;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests;

public sealed record SchemaProjectionTests
{
    [Fact]
    public void Enumerates()
    {
        Assert.NotEmpty(
            new SchemaProjection(new RandomSchema()).Select(x => x.ToArray())
        );
    }
}
