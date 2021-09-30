namespace Microsoft.Examples.Interfaces
{
    internal interface IODataQueryOptionsSlimBuilder
    {
        ODataQueryOptionsSlim<TElement> Build<TElement>() where TElement : class;
    }
}