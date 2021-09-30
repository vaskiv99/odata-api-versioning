using System;
using System.Collections.Generic;

namespace Microsoft.Examples
{
    public sealed class ODataQueryOptionName : IEquatable<ODataQueryOptionName>
    {
        private static readonly Lazy<IDictionary<string, ODataQueryOptionName>> _supportedOptionsMap
            = new Lazy<IDictionary<string, ODataQueryOptionName>>(GetAllSupportedOptionsMap);

        private static IDictionary<string, ODataQueryOptionName> GetAllSupportedOptionsMap()
        {
            return new Dictionary<string, ODataQueryOptionName> { { Filter.Name, Filter } };
        }

        private ODataQueryOptionName(string name)
        {
            Name = name;
        }

        private string Name { get; }

        public static ODataQueryOptionName Filter { get; }
            = new ODataQueryOptionName("$filter");

        private static IDictionary<string, ODataQueryOptionName> SupportedOptionsMap => _supportedOptionsMap.Value;

        public static implicit operator string(ODataQueryOptionName optionName)
            => optionName.Name;

        public bool Equals(ODataQueryOptionName? other)
            => other != null && other.Name == Name;

        public override bool Equals(object? other)
        {
            if (other is ODataQueryOptionName otherOption)
            {
                return Equals(otherOption);
            }

            return false;
        }

        public override int GetHashCode()
            => Name.GetHashCode();

        public override string ToString()
            => Name;
    }
}