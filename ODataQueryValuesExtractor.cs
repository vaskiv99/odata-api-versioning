using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Examples.Interfaces;
using Microsoft.Extensions.Primitives;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Examples
{
    internal sealed class ODataQueryValuesExtractor : IODataQueryValuesExtractor
    {
        private readonly ODataUriResolver _uriResolver;

        public ODataQueryValuesExtractor( IODataFeature oDataFeature )
        {
            _uriResolver = oDataFeature.GetUriResolver();

            if ( _uriResolver.EnableNoDollarQueryOptions )
            {
                throw new NotSupportedException( "No dollar query options are not supported" );
            }
        }

        public ODataQueryValuesSource ExtractFromForm( IFormCollection form )
            => ExtractFromStringValues( form );

        public ODataQueryValuesSource ExtractFromQuery( IQueryCollection query )
            => ExtractFromStringValues( query );

        private ODataQueryValuesSource ExtractFromStringValues( IEnumerable<KeyValuePair<string, StringValues>> source )
        {
            var supportedOptionsDictionaryBuilder = ImmutableDictionary.CreateBuilder<string, string>();
            var unsupportedOptionsSetBuilder = ImmutableHashSet.CreateBuilder<string>();

            foreach ( var (optionName, values) in source )
            {
                var normalizedOptionName = optionName.Trim();

                if ( _uriResolver.EnableCaseInsensitive )
                {
                    normalizedOptionName = normalizedOptionName.ToLowerInvariant();
                }

                if ( normalizedOptionName.StartsWith( "$" ) )
                {
                    if ( ODataQueryOptionName.TryParseSupported( normalizedOptionName, out var parsedOptionName ) )
                    {
                        supportedOptionsDictionaryBuilder.Add( parsedOptionName, values.ToString() );
                    }
                    else
                    {
                        unsupportedOptionsSetBuilder.Add( normalizedOptionName );
                    }
                }
                else if ( normalizedOptionName.StartsWith( "@" ) )
                {
                    supportedOptionsDictionaryBuilder.Add( normalizedOptionName, values.ToString() );
                }
            }

            return new ODataQueryValuesSource(
                supportedOptionsDictionaryBuilder.ToImmutable(),
                unsupportedOptionsSetBuilder.ToImmutable() );
        }
    }
}