using System.Data;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Pure.RelationalSchema.Self.Storage.Projection.Tests;

public sealed record DatabaseFixture : IDisposable
{
    private readonly PostgreSqlContainer _postgres;

    public IDbConnection Connection { get; }

    public DatabaseFixture()
    {
        _postgres = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        _postgres.StartAsync().GetAwaiter().GetResult();

        Connection = new NpgsqlConnection(_postgres.GetConnectionString());
        Connection.Open();
    }

    public void Dispose()
    {
        Connection.Dispose();
        _postgres.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}
