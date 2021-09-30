using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Microsoft.Examples
{
    public sealed class ODataQueryOptionName : IEquatable<ODataQueryOptionName>
    {
        private static readonly Lazy<IDictionary<string, ODataQueryOptionName>> _supportedOptionsMap
            = new Lazy<IDictionary<string, ODataQueryOptionName>>( GetAllSupportedOptionsMap );

        private static IDictionary<string, ODataQueryOptionName> GetAllSupportedOptionsMap()
        {
            var optionProperties = typeof( ODataQueryOptionName )
                .GetProperties( BindingFlags.Static | BindingFlags.Public )
                .Where( x => x.PropertyType == typeof( ODataQueryOptionName ) );

            var supportedOptions = optionProperties
                .Select( x => x.GetValue( null ) )
                .Cast<ODataQueryOptionName>();

            return supportedOptions.ToDictionary( x => x.Name );
        }

        private ODataQueryOptionName( string name )
        {
            Name = name;
        }

        private string Name { get; }

        public static ODataQueryOptionName Filter { get; }
            = new ODataQueryOptionName( "$filter" );

        public static ODataQueryOptionName OrderBy { get; }
            = new ODataQueryOptionName( "$orderby" );

        public static ODataQueryOptionName Select { get; }
            = new ODataQueryOptionName( "$select" );

        public static ODataQueryOptionName Expand { get; }
            = new ODataQueryOptionName( "$expand" );

        public static ODataQueryOptionName Count { get; }
            = new ODataQueryOptionName( "$count" );

        public static ODataQueryOptionName Skip { get; }
            = new ODataQueryOptionName( "$skip" );

        public static ODataQueryOptionName Top { get; }
            = new ODataQueryOptionName( "$top" );

        private static IDictionary<string, ODataQueryOptionName> SupportedOptionsMap => _supportedOptionsMap.Value;

        public static implicit operator string( ODataQueryOptionName optionName )
            => optionName.Name;

        public static ODataQueryOptionName operator +( ODataQueryOptionName left, ODataQueryOptionName right )
            => new ODataQueryOptionName( $"{left.Name}/{right.Name}" );

        public static bool TryParseSupported( string normalizedValue, [NotNullWhen( true )] out ODataQueryOptionName? supportedOptionName )
            => SupportedOptionsMap.TryGetValue( normalizedValue, out supportedOptionName );

        public bool Equals( ODataQueryOptionName? other )
            => other != null && other.Name == Name;

        public override bool Equals( object? other )
        {
            if ( other is ODataQueryOptionName otherOption )
            {
                return Equals( otherOption );
            }

            return false;
        }

        public override int GetHashCode()
            => Name.GetHashCode();

        public override string ToString()
            => Name;
    }
}