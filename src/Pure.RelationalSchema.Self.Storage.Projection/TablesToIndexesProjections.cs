using System.Collections;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record TablesToIndexesProjections : IEnumerable<IRow>
{
    private readonly ITable _entity;

    private readonly IEnumerable<IColumn> _columns;

    public TablesToIndexesProjections(ITable entity)
        : this(entity, new TablesToIndexesTable().Columns) { }

    public TablesToIndexesProjections(ITable entity, IEnumerable<IColumn> columns)
    {
        _entity = entity;
        _columns = columns;
    }

    public IEnumerator<IRow> GetEnumerator()
    {
        IDeterminedHash tableHash = new CachedDeterminedHash(
            new RowHash(new TableProjection(_entity))
        );
        return _entity
            .Indexes.Select(x => new TableToIndexProjection(
                tableHash,
                new RowHash(new IndexProjection(x)),
                _columns
            ))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
