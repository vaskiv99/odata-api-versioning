using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace Microsoft.Examples.Interfaces
{
    /// <summary>
    /// OData response wrapper.
    /// </summary>
    public sealed class ODataCollectionResponse<TItem>
    {
        /// <summary>
        /// OData context.
        /// </summary>
        [JsonProperty( "@odata.context" )]
        [JsonPropertyName( "@odata.context" )]
        public string Context { get; set; } = default!;

        /// <summary>
        /// Total count of elements if $count is true. Null otherwise.
        /// </summary>
        [JsonProperty( "@odata.count" )]
        [JsonPropertyName( "@odata.count" )]
        public int? Count { get; set; }

        /// <summary>
        /// Response elements.
        /// </summary>
        [JsonProperty( "value" )]
        [JsonPropertyName( "value" )]
        public TItem[] Items { get; set; } = Array.Empty<TItem>();
    }
}