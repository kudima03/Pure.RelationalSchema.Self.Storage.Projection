using System.Data;
using System.Diagnostics.CodeAnalysis;
using Pure.Primitives.Materialized.String;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.HashCodes;
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

    //[Fact]
    //public void CorrectGroupCount()
    //{
    //    Assert.Equal(
    //        11,
    //        new SchemaProjection(new RelationalSchemaSchema())
    //            .Select(x => x.ToArray())
    //            .Count()
    //    );
    //}

    //[Fact]
    //public void CorrectCellsCount()
    //{
    //    Assert.Equal(
    //        287,
    //        new SchemaProjection(new RelationalSchemaSchema())
    //            .SelectMany(x =>
    //                x.SelectMany(c => c.Cells.Values.Select(v => v.Value)).ToArray()
    //            )
    //            .Count()
    //    );
    //}

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
                new PostgreSqlStoredSchemaDataSet(schema, new FakeConn(_databaseFixture.Connection)),
                new SchemaProjection(new RelationalSchemaSchema())
            );

        var a = schemaDataSet.TablesDatasets.Keys.Select(x => (new MaterializedString(x.Name).Value, Convert.ToHexString(new TableHash(x).ToArray()))).ToList();

        Assert.Equal(200, schemaDataSet.TablesDatasets.SelectMany(x => x.Value).Count());
    }
}

internal sealed record FakeConn : IDbConnection
{
    private readonly IDbConnection _connection;

    public FakeConn(IDbConnection connection)
    {
        _connection = connection;
    }

    public string ConnectionString { get => _connection.ConnectionString; set => _connection.ConnectionString = value; }

    public int ConnectionTimeout => _connection.ConnectionTimeout;

    public string Database => _connection.Database;

    public ConnectionState State => _connection.State;

    public IDbTransaction BeginTransaction()
    {
        return _connection.BeginTransaction();
    }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        return _connection.BeginTransaction(il);
    }

    public void ChangeDatabase(string databaseName)
    {
        _connection.ChangeDatabase(databaseName);
    }

    public void Close()
    {
        _connection.Close();
    }

    public IDbCommand CreateCommand()
    {
        return new FakeCommand(_connection.CreateCommand());
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public void Open()
    {
        _connection.Open();
    }
}

internal sealed record FakeCommand : IDbCommand
{
    private readonly IDbCommand _command;

    public FakeCommand(IDbCommand command)
    {
        _command = command;
    }

    public string CommandText
    {
        get
        {
            return _command.CommandText;
        }
        set
        {
            _command.CommandText = value;
        }
    }
    public int CommandTimeout { get => _command.CommandTimeout; set => _command.CommandTimeout = value; }
    public CommandType CommandType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public IDbConnection? Connection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IDataParameterCollection Parameters => throw new NotImplementedException();

    public IDbTransaction? Transaction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public UpdateRowSource UpdatedRowSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Cancel()
    {
        _command.Cancel();
    }

    public IDbDataParameter CreateParameter()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _command.Dispose();
    }

    public int ExecuteNonQuery()
    {
        if (_command.CommandText == "")
        {
            return 0;
        }
        return _command.ExecuteNonQuery();
    }

    public IDataReader ExecuteReader()
    {
        if (_command.CommandText == "")
        {
            return new ASd();
        }
        return _command.ExecuteReader();
    }

    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
        throw new NotImplementedException();
    }

    public object? ExecuteScalar()
    {
        throw new NotImplementedException();
    }

    public void Prepare()
    {
        throw new NotImplementedException();
    }
}

internal record ASd : IDataReader
{
    public object this[int i] => throw new NotImplementedException();

    public object this[string name] => throw new NotImplementedException();

    public int Depth => throw new NotImplementedException();

    public bool IsClosed => throw new NotImplementedException();

    public int RecordsAffected => throw new NotImplementedException();

    public int FieldCount => throw new NotImplementedException();

    public void Close()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public bool GetBoolean(int i)
    {
        throw new NotImplementedException();
    }

    public byte GetByte(int i)
    {
        throw new NotImplementedException();
    }

    public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
    {
        throw new NotImplementedException();
    }

    public char GetChar(int i)
    {
        throw new NotImplementedException();
    }

    public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
    {
        throw new NotImplementedException();
    }

    public IDataReader GetData(int i)
    {
        throw new NotImplementedException();
    }

    public string GetDataTypeName(int i)
    {
        throw new NotImplementedException();
    }

    public DateTime GetDateTime(int i)
    {
        throw new NotImplementedException();
    }

    public decimal GetDecimal(int i)
    {
        throw new NotImplementedException();
    }

    public double GetDouble(int i)
    {
        throw new NotImplementedException();
    }

    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)]
    public Type GetFieldType(int i)
    {
        throw new NotImplementedException();
    }

    public float GetFloat(int i)
    {
        throw new NotImplementedException();
    }

    public Guid GetGuid(int i)
    {
        throw new NotImplementedException();
    }

    public short GetInt16(int i)
    {
        throw new NotImplementedException();
    }

    public int GetInt32(int i)
    {
        throw new NotImplementedException();
    }

    public long GetInt64(int i)
    {
        throw new NotImplementedException();
    }

    public string GetName(int i)
    {
        throw new NotImplementedException();
    }

    public int GetOrdinal(string name)
    {
        throw new NotImplementedException();
    }

    public DataTable? GetSchemaTable()
    {
        throw new NotImplementedException();
    }

    public string GetString(int i)
    {
        throw new NotImplementedException();
    }

    public object GetValue(int i)
    {
        throw new NotImplementedException();
    }

    public int GetValues(object[] values)
    {
        throw new NotImplementedException();
    }

    public bool IsDBNull(int i)
    {
        throw new NotImplementedException();
    }

    public bool NextResult()
    {
        throw new NotImplementedException();
    }

    public bool Read()
    {
        return false;
    }
}
