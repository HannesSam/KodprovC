using System.Collections.Generic;

namespace MashupLogic.ArtistOrBand.Models
{
    public class ArtistOrBandModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Album> Albums { get; set; }
        public int SuccessCode { get; set; }
        public string ErrorNotes { get; set; }
    }

    public class Album
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
    }
}
