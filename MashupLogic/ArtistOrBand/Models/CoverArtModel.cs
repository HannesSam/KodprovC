using System.Text.Json.Serialization;

namespace MashupLogic.ArtistOrBand.Models
{
    public class CoverArtModel
    {
        [JsonPropertyName("images")]
        public Image[] Images { get; set; }
    }
    public class Image
    {
        [JsonPropertyName("front")]
        public bool Front { get; set; }
        [JsonPropertyName("image")]
        public string ImageUrl { get; set; }
    }

}
