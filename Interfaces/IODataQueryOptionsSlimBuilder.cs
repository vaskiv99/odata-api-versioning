using Microsoft.AspNet.OData.Query;

namespace Microsoft.Examples.Interfaces
{
    internal interface IODataQueryOptionsSlimBuilder
    {
        ODataQueryOptionsSlim<TElement> Build<TElement>(
            ODataQueryValuesSource valuesSource,
            ODataValidationSettings validationSettings )
            where TElement : class;
    }
}