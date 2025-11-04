using Pure.HashCodes.Abstractions;
using Pure.Primitives.Abstractions.String;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection;

public sealed record CellSwitch<TSelector> : ICell
{
    private readonly TSelector _parameter;

    private readonly Func<TSelector, IDeterminedHash> _hashFactory;

    private readonly ICell? _defaultValue;

    private readonly IEnumerable<KeyValuePair<TSelector, ICell>> _branches;

    public CellSwitch(
        TSelector parameter,
        IEnumerable<KeyValuePair<TSelector, ICell>> branches,
        Func<TSelector, IDeterminedHash> hashFactory
    )
        : this(parameter, branches, hashFactory, null!) { }

    public CellSwitch(
        TSelector parameter,
        IEnumerable<KeyValuePair<TSelector, ICell>> branches,
        Func<TSelector, IDeterminedHash> hashFactory,
        ICell defaultValue
    )
    {
        _parameter = parameter;
        _hashFactory = hashFactory;
        _defaultValue = defaultValue;
        _branches = branches;
    }

    public IString Value
    {
        get
        {
            IEnumerable<byte> parameterHash = _hashFactory(_parameter);

            IEnumerable<ICell> filteredBranches = _branches
                .Where(x => parameterHash.SequenceEqual(_hashFactory(x.Key)))
                .Select(x => x.Value);

            return _defaultValue == null
                ? filteredBranches.First().Value
                : filteredBranches.FirstOrDefault(_defaultValue).Value;
        }
    }

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        throw new NotSupportedException();
    }
}
