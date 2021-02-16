using System.Threading.Tasks;

namespace CygniKodprovApp.ArtistOrBand.Models
{
    public interface IArtistOrBandLogic
    {
        Task<ArtistOrBandModel> CreateArtistOrBandMashupAsync(string MBID, string requestId);
    }
}