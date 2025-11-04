using System.Collections;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record IndexesToColumnsProjections : IEnumerable<IRow>
{
    private readonly IIndex _entity;

    private readonly IEnumerable<IColumn> _columns;

    public IndexesToColumnsProjections(IIndex entity)
        : this(entity, new IndexesToColumnsTable().Columns) { }

    public IndexesToColumnsProjections(IIndex entity, IEnumerable<IColumn> columns)
    {
        _entity = entity;
        _columns = columns;
    }

    public IEnumerator<IRow> GetEnumerator()
    {
        IDeterminedHash indexHash = new CachedDeterminedHash(
            new RowHash(new IndexProjection(_entity))
        );
        return _entity
            .Columns.Select(x => new IndexToColumnsProjection(
                indexHash,
                new RowHash(new ColumnProjection(x)),
                _columns
            ))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
