using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Examples.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Examples
{
    internal sealed class ODataQueryOptionsSlimModelBinder : IModelBinder
    {
        private static readonly MethodInfo _buildOptionsMethod
            = typeof(ODataQueryOptionsSlimModelBinder).GetMethod(nameof(BuildOptions), BindingFlags.Instance | BindingFlags.NonPublic)
              ?? throw new InvalidOperationException();

        private readonly IODataQueryOptionsSlimBuilder _builder;

        public ODataQueryOptionsSlimModelBinder(
            IODataQueryOptionsSlimBuilder builder)
        {
            _builder = builder;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var elementType = bindingContext.ModelType.GetGenericArguments().First();

            var model = BuildModel(elementType);
            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }

        private object BuildModel(Type elementType)
        {
            var model = _buildOptionsMethod
                .MakeGenericMethod(elementType)
                .Invoke(this, null);

            return model ?? throw new InvalidOperationException("Built model cannot be null");
        }

        private ODataQueryOptionsSlim<TElement> BuildOptions<TElement>()
            where TElement : class
            => _builder.Build<TElement>();
    }
}