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
        public ODataQueryOptionsSlim<TElement> Build<TElement>()
            where TElement : class
        {
            return new ODataQueryOptionsSlim<TElement>();
        }
    }
}