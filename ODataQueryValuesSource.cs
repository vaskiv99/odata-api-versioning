using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Examples
{
    internal sealed class ODataQueryValuesSource
    {
        private readonly IDictionary<string, string> _normalizedValues;

        public ODataQueryValuesSource(
            IDictionary<string, string> normalizedValues,
            IImmutableSet<string> unsupportedSystemQueryOptions )
        {
            _normalizedValues = normalizedValues;

            UnsupportedSystemQueryOptions = unsupportedSystemQueryOptions;
        }

        public IDictionary<string, string> ParserParameters
            => new Dictionary<string, string>( _normalizedValues );

        public IImmutableSet<string> UnsupportedSystemQueryOptions { get; }

        public bool IsEmpty => _normalizedValues.Count == 0;

        public bool TryGetOptionRawValue( ODataQueryOptionName optionName, [NotNullWhen( true )] out string? rawValue )
            => _normalizedValues.TryGetValue( optionName, out rawValue ) && !string.IsNullOrWhiteSpace( rawValue );
    }
}