using System.Text.Json.Serialization;

namespace MashupLogic.ArtistOrBand.Models
{
    public class MusicBrainzModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("relations")]
        public Relation[] Relations { get; set; }
        [JsonPropertyName("release-groups")]
        public ReleaseGroups[] Releasegroups { get; set; }
    }
    public class Relation
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("url")]
        public Url Url { get; set; }
    }

    public class Url
    {
        [JsonPropertyName("resource")]
        public string Resource { get; set; }
    }

    public class ReleaseGroups
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}