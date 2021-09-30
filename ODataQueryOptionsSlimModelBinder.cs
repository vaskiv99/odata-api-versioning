using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Examples.Interfaces;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Examples
{
    internal sealed class ODataQueryOptionsSlimModelBinder : IModelBinder
    {
        private static readonly MethodInfo _buildOptionsMethod
            = typeof( ODataQueryOptionsSlimModelBinder ).GetMethod( nameof( BuildOptions ), BindingFlags.Instance | BindingFlags.NonPublic )
              ?? throw new InvalidOperationException();

        private readonly IODataQueryValuesExtractor _extractor;
        private readonly IODataQueryOptionsSlimBuilder _builder;

        public ODataQueryOptionsSlimModelBinder(
            IODataQueryValuesExtractor extractor,
            IODataQueryOptionsSlimBuilder builder )
        {
            _extractor = extractor;
            _builder = builder;
        }

        public Task BindModelAsync( ModelBindingContext bindingContext )
        {
            var valuesSource = GetValuesSource( bindingContext );

            var elementType = bindingContext.ModelType.GetGenericArguments().First();

            var model = BuildModel( elementType, valuesSource );
            bindingContext.Result = ModelBindingResult.Success( model );

            AddUnsupportedQueryOptionErrors(
                bindingContext.ModelState,
                valuesSource.UnsupportedSystemQueryOptions );

            return Task.CompletedTask;
        }

        private ODataQueryValuesSource GetValuesSource( ModelBindingContext bindingContext )
        {
            var bindingSource = bindingContext.BindingSource;
            var httpRequest = bindingContext.HttpContext.Request;

            if ( bindingSource == BindingSource.Form )
            {
                return _extractor.ExtractFromForm( httpRequest.Form );
            }
            else if ( bindingSource == BindingSource.Query )
            {
                return _extractor.ExtractFromQuery( httpRequest.Query );
            }
            else
            {
                throw new NotSupportedException(
                    $"{bindingSource} is not supported." +
                    $"Please use [{nameof( FromFormAttribute )}] or [{nameof( FromQueryAttribute )}]." );
            }
        }

        private void AddUnsupportedQueryOptionErrors(
            ModelStateDictionary modelState,
            IImmutableSet<string> unsupportedSystemQueryOptions )
        {
            foreach ( var unsupportedQueryOption in unsupportedSystemQueryOptions )
            {
                var added = modelState.TryAddModelError( unsupportedQueryOption, "Unsupported OData query option" );

                if ( !added )
                {
                    break;
                }
            }
        }

        private object BuildModel( Type elementType, ODataQueryValuesSource valuesSource )
        {
            var parameters = new object[] { valuesSource };

            var model = _buildOptionsMethod
                .MakeGenericMethod( elementType )
                .Invoke( this, parameters );

            return model ?? throw new InvalidOperationException( "Built model cannot be null" );
        }

        private ODataQueryOptionsSlim<TElement> BuildOptions<TElement>( ODataQueryValuesSource valuesSource )
            where TElement : class
            => _builder.Build<TElement>( valuesSource, new ODataValidationSettings() );
    }
}