using System.Collections;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record SchemasToTablesProjections : IEnumerable<IRow>
{
    private readonly ISchema _entity;

    private readonly IEnumerable<IColumn> _columns;

    public SchemasToTablesProjections(ISchema entity)
        : this(entity, new SchemasToTablesTable().Columns) { }

    public SchemasToTablesProjections(ISchema entity, IEnumerable<IColumn> columns)
    {
        _entity = entity;
        _columns = columns;
    }

    public IEnumerator<IRow> GetEnumerator()
    {
        IDeterminedHash schemaHash = new CachedDeterminedHash(
            new RowHash(new SchemaEntityProjection(_entity))
        );
        return _entity
            .Tables.Select(x => new SchemaToTablesProjection(
                schemaHash,
                new RowHash(new TableProjection(x)),
                _columns
            ))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
