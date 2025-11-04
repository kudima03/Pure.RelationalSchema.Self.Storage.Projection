using System.Collections;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record SchemaToForeignKeysProjections : IEnumerable<IRow>
{
    private readonly ISchema _schema;

    private readonly IEnumerable<IColumn> _columns;

    public SchemaToForeignKeysProjections(ISchema schema)
        : this(schema, new SchemasToForeignKeysTable().Columns) { }

    public SchemaToForeignKeysProjections(ISchema schema, IEnumerable<IColumn> columns)
    {
        _schema = schema;
        _columns = columns;
    }

    public IEnumerator<IRow> GetEnumerator()
    {
        IDeterminedHash schemaHash = new CachedDeterminedHash(
            new RowHash(new SchemaEntityProjection(_schema))
        );
        return _schema
            .ForeignKeys.Select(x => new SchemaToForeignKeysProjection(
                schemaHash,
                new RowHash(new ForeignKeyProjection(x)),
                _columns
            ))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
