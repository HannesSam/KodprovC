using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CygniKodprovApp.ArtistOrBand.Models
{
    public class WikipediaModel
    {
        [JsonPropertyName("query")]
        public Query Query { get; set; }
    }
    public class Query
    {
        [JsonPropertyName("pages")]
        public Dictionary<string, Pages> Pages { get; set; }
    }

    public class Pages
    {
        [JsonPropertyName("extract")]
        public string Extract { get; set; }
    }

}
