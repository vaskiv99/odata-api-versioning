using Microsoft.AspNetCore.Http;

namespace Microsoft.Examples.Interfaces
{
    internal interface IODataQueryValuesExtractor
    {
        ODataQueryValuesSource ExtractFromForm( IFormCollection form );

        ODataQueryValuesSource ExtractFromQuery( IQueryCollection query );
    }
}