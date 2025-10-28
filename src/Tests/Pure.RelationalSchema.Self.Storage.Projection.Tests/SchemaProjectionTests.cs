using System.Data;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.PostgreSQL;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests;

public sealed record SchemaProjectionTests : IAsyncLifetime, IDisposable
{
    private DatabaseFixture? _databaseFixture;

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
            288,
            new SchemaProjection(new RelationalSchemaSchema())
                .SelectMany(x =>
                    x.SelectMany(c => c.Cells.Values.Select(v => v.Value)).ToArray()
                )
                .Count()
        );
    }

    [Fact]
    public void CreateSingleSelfProjection()
    {
        ISchema schema = new PostgreSqlCreatedSchema(
            new RelationalSchemaSchema(),
            _databaseFixture!.Connection
        );

        IStoredSchemaDataSet schemaDataSet =
            new PostgreSqlStoredSchemaDataSetWithInsertedRows(
                new PostgreSqlStoredSchemaDataSet(schema, _databaseFixture.Connection),
                new SchemaProjection(new RelationalSchemaSchema())
            );

        Assert.Equal(125, schemaDataSet.SelectMany(x => x.Value).Count());
    }

    [Fact]
    public void CreateMultipleSelfProjection()
    {
        ISchema schema = new PostgreSqlCreatedSchema(
            new RelationalSchemaSchema(),
            _databaseFixture!.Connection
        );

        IStoredSchemaDataSet aggregated = Enumerable
            .Range(0, Random.Shared.Next(5, 10))
            .Aggregate(
                new PostgreSqlStoredSchemaDataSetWithInsertedRows(
                    new PostgreSqlStoredSchemaDataSet(
                        schema,
                        _databaseFixture.Connection
                    ),
                    new SchemaProjection(new RelationalSchemaSchema())
                ),
                (x, _) =>
                    new PostgreSqlStoredSchemaDataSetWithInsertedRows(
                        x,
                        new SchemaProjection(new RelationalSchemaSchema())
                    )
            );

        Assert.Equal(125, aggregated.SelectMany(x => x.Value).Count());

        ISchema a = new SchemasFromRows(aggregated).Single();

        Assert.True(
            new SchemaHash(a).SequenceEqual(new SchemaHash(new RelationalSchemaSchema()))
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

    public void Dispose()
    {
        _databaseFixture?.Dispose();
    }

    public Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        _databaseFixture = new DatabaseFixture();
        return Task.CompletedTask;
    }
}
