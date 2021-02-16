using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CygniKodprovApp.ArtistOrBand.Models
{
    public class WikiDataModel
    {
        public Dictionary<string, WikiDataObject> Entities { get; set; }
    }

    public class WikiDataObject
    {
        [JsonPropertyName("sitelinks")]
        public Sitelinks Sitelinks { get; set; }
    }

    public class Sitelinks
    {
        [JsonPropertyName("enwiki")]
        public Enwiki Enwiki { get; set; }

    }

    public class Enwiki
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
