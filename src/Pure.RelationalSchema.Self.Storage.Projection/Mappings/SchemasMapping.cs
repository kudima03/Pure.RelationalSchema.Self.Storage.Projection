using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection.Mappings;

internal sealed record SchemasMapping : ICell
{
    private readonly ISchema _schema;

    private readonly IColumn _column;

    public SchemasMapping(ISchema schema, IColumn column)
    {
        _schema = schema;
        _column = column;
    }

    public IString Value =>
        new CellMapping<IColumn>(
            _column,
            [new KeyValuePair<IColumn, ICell>(new NameColumn(), new Cell(_schema.Name))],
            x => new ColumnHash(x)
        ).Value;

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        throw new NotSupportedException();
    }
}
