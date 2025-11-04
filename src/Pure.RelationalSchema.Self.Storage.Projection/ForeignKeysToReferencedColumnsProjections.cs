using System.Collections;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record ForeignKeysToReferencedColumnsProjections : IEnumerable<IRow>
{
    private readonly IForeignKey _entity;

    private readonly IEnumerable<IColumn> _columns;

    public ForeignKeysToReferencedColumnsProjections(IForeignKey entity)
        : this(entity, new ForeignKeysToReferencedColumnsTable().Columns) { }

    public ForeignKeysToReferencedColumnsProjections(
        IForeignKey entity,
        IEnumerable<IColumn> columns
    )
    {
        _entity = entity;
        _columns = columns;
    }

    public IEnumerator<IRow> GetEnumerator()
    {
        IDeterminedHash fkHash = new CachedDeterminedHash(
            new RowHash(new ForeignKeyProjection(_entity))
        );
        return _entity
            .ReferencedColumns.Select(x => new ForeignKeyToReferencingColumnProjection(
                fkHash,
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
