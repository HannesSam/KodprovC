
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CygniKodprovApp.ArtistOrBand.Models
{
    public interface ICallsToServices
    {
        Task<List<Album>> GetAlbumDataAsync(MusicBrainzModel reciveData);
        Task<MusicBrainzModel> GetMusicBrainzDataAsync(string ArtistOrBandMBID, string requestId);
        Task<WikipediaModel> GetWikipediaDataAsync(string wikipediaLink);
        Task<string> GetWikpediaIdFromWikiDataAsync(string wikiDataLink);
    }
}