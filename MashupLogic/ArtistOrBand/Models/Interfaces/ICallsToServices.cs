using System.Collections.Generic;
using System.Threading.Tasks;

namespace MashupLogic.ArtistOrBand.Models.Interfaces
{
    public interface ICallsToServices
    {
        Task<List<Album>> GetAlbumDataAsync(MusicBrainzModel reciveData);
        Task<MusicBrainzModel> GetMusicBrainzDataAsync(string MBID, string requestId);
        Task<WikipediaModel> GetWikipediaDataAsync(MusicBrainzModel musicBrainzData);
    }
}