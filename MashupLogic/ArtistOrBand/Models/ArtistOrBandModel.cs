using System.Collections.Generic;

namespace MashupLogic.ArtistOrBand.Models
{
    public class ArtistOrBandModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Album> Albums { get; set; }
        // The successCode tells us if the request was a full success(1) or a partial success(2) which happens when some of the data cannot be retrieved.
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
