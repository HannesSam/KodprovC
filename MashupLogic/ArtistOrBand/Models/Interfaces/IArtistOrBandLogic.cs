using System.Threading.Tasks;

namespace MashupLogic.ArtistOrBand.Models.Interfaces
{
    public interface IArtistOrBandLogic
    {
        Task<ArtistOrBandModel> CreateArtistOrBandMashupAsync(string MBID, string requestId);
    }
}