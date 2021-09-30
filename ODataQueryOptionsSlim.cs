using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.UriParser;

namespace Microsoft.Examples
{
    [ModelBinder(typeof(ODataQueryOptionsSlimModelBinder))]
    public sealed class ODataQueryOptionsSlim<TElement>
        where TElement : class
    {
        private readonly FilterQueryOption _filterOption;

        internal ODataQueryOptionsSlim(FilterQueryOption filter)
        {
            _filterOption = filter;
        }

        public ODataQueryOptionsSlim() { }

        public FilterClause Filter => _filterOption?.FilterClause;
    }
}