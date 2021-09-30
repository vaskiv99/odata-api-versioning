using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.Examples.Interfaces;
using Microsoft.OData.UriParser;
using System;
using System.Reflection;

namespace Microsoft.Examples
{
    internal sealed class ODataQueryOptionsSlimBuilder : IODataQueryOptionsSlimBuilder
    {
        private static readonly PropertyInfo _queryContextRequestContainerProperty
            = typeof( ODataQueryContext ).GetProperty( nameof( ODataQueryContext.RequestContainer ) )
              ?? throw new InvalidOperationException();

        private readonly IODataFeature _feature;

        public ODataQueryOptionsSlimBuilder( IODataFeature feature )
        {
            _feature = feature;
        }

        public ODataQueryOptionsSlim<TElement> Build<TElement>(
            ODataQueryValuesSource valuesSource,
            ODataValidationSettings validationSettings )
            where TElement : class
        {
            if ( valuesSource.IsEmpty )
            {
                return ODataQueryOptionsSlim<TElement>.Empty;
            }

            var queryContext = CreateQueryContext<TElement>();
            var parser = CreateQueryParser( queryContext, valuesSource );

            return new ODataQueryOptionsSlim<TElement>(
                BuildFilterOption(),
                BuildOrderByOption(),
                BuildSelectExpandOption(),
                BuildCountOption(),
                BuildSkipOption(),
                BuildTopOption(),
                validationSettings );

            FilterQueryOption? BuildFilterOption()
            {
                if ( valuesSource.TryGetOptionRawValue( ODataQueryOptionName.Filter, out var filterRawValue ) )
                {
                    return new FilterQueryOption( filterRawValue, queryContext, parser );
                }

                return default;
            }

            OrderByQueryOption? BuildOrderByOption()
            {
                if ( valuesSource.TryGetOptionRawValue( ODataQueryOptionName.OrderBy, out var orderByRawValue ) )
                {
                    return new OrderByQueryOption( orderByRawValue, queryContext, parser );
                }

                return default;
            }

            SelectExpandQueryOption? BuildSelectExpandOption()
            {
                var hasSelect = valuesSource.TryGetOptionRawValue( ODataQueryOptionName.Select, out var selectRawValue );
                var hasExpand = valuesSource.TryGetOptionRawValue( ODataQueryOptionName.Expand, out var expandRawValue );

                if ( hasSelect || hasExpand )
                {
                    return new SelectExpandQueryOption( selectRawValue, expandRawValue, queryContext, parser );
                }

                return default;
            }

            CountQueryOption? BuildCountOption()
            {
                if ( valuesSource.TryGetOptionRawValue( ODataQueryOptionName.Count, out var rawCountValue ) )
                {
                    return new CountQueryOption( rawCountValue, queryContext, parser );
                }

                return default;
            }

            SkipQueryOption? BuildSkipOption()
            {
                if ( valuesSource.TryGetOptionRawValue( ODataQueryOptionName.Skip, out var rawSkipValue ) )
                {
                    return new SkipQueryOption( rawSkipValue, queryContext, parser );
                }

                return default;
            }

            TopQueryOption? BuildTopOption()
            {
                if ( valuesSource.TryGetOptionRawValue( ODataQueryOptionName.Top, out var rawTopValue ) )
                {
                    return new TopQueryOption( rawTopValue, queryContext, parser );
                }

                return default;
            }
        }

        private ODataQueryOptionParser CreateQueryParser(
            ODataQueryContext queryContext,
            ODataQueryValuesSource valuesSource )
            => new ODataQueryOptionParser(
                queryContext.Model,
                queryContext.ElementType,
                queryContext.NavigationSource,
                valuesSource.ParserParameters )
            {
                Resolver = _feature.GetUriResolver()
            };

        private ODataQueryContext CreateQueryContext<TElement>()
            where TElement : class
        {
            var context = new ODataQueryContext( _feature.GetEdmModel(), typeof( TElement ), _feature.Path );

            // Using reflection because setter of the property is internal
            // The ONLY other way to initialize it is by passing context object to ODataQueryOptions constructor!
            _queryContextRequestContainerProperty.SetValue( context, _feature.RequestContainer );

            return context;
        }
    }
}