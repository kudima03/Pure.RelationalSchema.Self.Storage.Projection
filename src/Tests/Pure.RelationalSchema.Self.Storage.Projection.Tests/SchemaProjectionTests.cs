using Pure.Primitives.Materialized.String;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.PostgreSQL;
using Pure.RelationalSchema.Storage.PostgreSQL.Tests;

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
            11,
            new SchemaProjection(new RelationalSchemaSchema())
                .Select(x => x.ToArray())
                .Count()
        );
    }

    [Fact]
    public void CorrectCellsCount()
    {
        Assert.Equal(
            287,
            new SchemaProjection(new RelationalSchemaSchema())
                .SelectMany(x =>
                    x.SelectMany(c => c.Cells.Values.Select(v => v.Value)).ToArray()
                )
                .Count()
        );
    }

    [Fact]
    public void DatabaseCreatedBySelfProjectionSchema()
    {
        ISchema schema = new PostgreSqlCreatedSchema(
            new RelationalSchemaSchema(),
            _databaseFixture.Connection
        );

        IStoredSchemaDataSet schemaDataSet =
            new PostgreSqlStoredSchemaDataSetWithInsertedRows(
                schema,
                new PostgreSqlStoredSchemaDataSet(schema, _databaseFixture.Connection),
                new SchemaProjection(new RelationalSchemaSchema())
            );

        Assert.Equal(
            200,
            schemaDataSet
                .TablesDatasets.OrderBy(x =>
                    new SchemaProjection(new RelationalSchemaSchema())
                        .Select(c => c.Key)
                        .Select(c => Convert.ToHexString(new TableHash(c).ToArray()))
                        .Index()
                        .First(c => c.Item == Convert.ToHexString(new TableHash(x.Key).ToArray())).Index
                )
                .SelectMany(x => x.Value)
                .Count()
        );
    }
}
