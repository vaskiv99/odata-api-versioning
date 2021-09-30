using Microsoft.AspNet.OData.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.Examples
{
    internal static class ODataFeatureExtensions
    {
        public static IEdmModel GetEdmModel( this IODataFeature feature )
            => feature.RequestContainer.GetRequiredService<IEdmModel>();

        public static ODataUriResolver GetUriResolver( this IODataFeature feature )
            => feature.RequestContainer.GetRequiredService<ODataUriResolver>();
    }
}