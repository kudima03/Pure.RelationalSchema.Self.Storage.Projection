using System.Data;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Self.Schema;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.PostgreSQL;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests;

public sealed record SchemaProjectionTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture;

    public SchemaProjectionTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
    }

    [Fact]
    public void CorrectGroupCount()
    {
        Assert.Equal(
            9,
            new SchemaProjection(new RelationalSchemaSchema())
                .Select(x => x.ToArray())
                .Count()
        );
    }

    [Fact]
    public void CorrectCellsCount()
    {
        Assert.Equal(
            289,
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
            _databaseFixture.Connection
        );

        IStoredSchemaDataSet schemaDataSet =
            new PostgreSqlStoredSchemaDataSetWithInsertedRows(
                new PostgreSqlStoredSchemaDataSet(schema, _databaseFixture.Connection),
                new SchemaProjection(new RelationalSchemaSchema())
            );

        Assert.Equal(98, schemaDataSet.SelectMany(x => x.Value).Count());
    }

    [Fact]
    public void CreateMultipleSelfProjection()
    {
        ISchema schema = new PostgreSqlCreatedSchema(
            new RelationalSchemaSchema(),
            _databaseFixture.Connection
        );

        IStoredSchemaDataSet aggregated = Enumerable
            .Range(0, 100)
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

        Assert.Equal(98, aggregated.SelectMany(x => x.Value).Count());
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
