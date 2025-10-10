using System.Data;
using Npgsql;
using Pure.Primitives.Bool;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.ColumnType;
using Testcontainers.PostgreSql;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Storage.PostgreSQL.Tests;

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
