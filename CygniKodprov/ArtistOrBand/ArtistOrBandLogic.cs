using CygniKodprovApp.ArtistOrBand.Models;
using CygniKodprovApp.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CygniKodprovApp.ArtistOrBand
{
    // This class contains the main logic for the ArtistOrBand mashup.
    public class ArtistOrBandLogic : IArtistOrBandLogic
    {
        // Property that holds the main data modell for the class that will hold all the return data.
        private ArtistOrBandModel ArtistOrBandData { get; }
        // Property that holds the helper class for api calls to the different services.
        private ICallsToServices CallsToServices { get; }
        public ArtistOrBandLogic(ICallsToServices callsToServices)
        {
            CallsToServices = callsToServices;
            // Creates a new data model for the return data and creates an empty list of albums and sets the succes code to the defeault value 1(success). 
            ArtistOrBandData = new ArtistOrBandModel
            {
                Albums = new List<Album>(),
                SuccessCode = 1
            };
        }

        // This method starts the creation of the ArtistAndBand mashup.
        // Inputs: 
        // MBID is the identifyer used to get the intial data from MusicBrainz. 
        // requestId is an unique idntifyer for this request.
        // Return:
        // ArtistOrBandModel contains the completted mashup.
        public async Task<ArtistOrBandModel> CreateArtistOrBandMashupAsync(string MBID, string requestId)
        {

            // Get the data from the MusicBrainz api.
            MusicBrainzModel musicBrainzData = await CallsToServices.GetMusicBrainzDataAsync(MBID, requestId);

            // Get a link that can be used to query wikipedia as well as handle one exception that the tasks could throw.
            string wikipediaLink;
            try
            {
                wikipediaLink = await GetWikipediaLink(musicBrainzData);
            }
            catch (MissingSecondaryDataException ex)
            {
                // If the process of collectiong this data fails sets the succes code to 2(Partial success) and logs the error in the return object.
                ArtistOrBandData.SuccessCode = 2;
                ArtistOrBandData.ErrorNotes += ex.Message;
                wikipediaLink = "";
            }

            // Start the call to get data from wikipedia and CoverArt.
            Task<List<Album>> albumListTask = CallsToServices.GetAlbumDataAsync(musicBrainzData);
            Task<WikipediaModel> wikipediaDataTask; wikipediaDataTask = CallsToServices.GetWikipediaDataAsync(wikipediaLink);

            // Await the completions of the tasks and load in the data as well as handle one exception that the tasks could throw.
            WikipediaModel wikipediaData;
            List<Album> albumList;
            try
            {
                wikipediaData = await wikipediaDataTask;
            }
            catch (MissingSecondaryDataException ex)
            {
                // If the process of collectiong this data fails sets the succes code to 2(Partial success) and logs the error in the return object.
                ArtistOrBandData.SuccessCode = 2;
                ArtistOrBandData.ErrorNotes += ex.Message;
                wikipediaData = new WikipediaModel();
            }
            try
            {
                albumList = await albumListTask;
            }
            catch (MissingSecondaryDataException ex)
            {
                // If the process of collectiong this data fails sets the succes code to 2(Partial success) and logs the error in the return object.
                ArtistOrBandData.SuccessCode = 2;
                ArtistOrBandData.ErrorNotes += ex.Message;
                albumList = new List<Album>();
            }

            // Assigns all the variables to the return object.
            ArtistOrBandData.Id = MBID;
            ArtistOrBandData.Name = musicBrainzData.Name;
            // Maybe an error check should be done here but if the wikipedia query succeds an empty pages list should not be possible.
            ArtistOrBandData.Description = wikipediaData.Query.Pages.FirstOrDefault().Value.Extract;
            ArtistOrBandData.Albums = albumList;

            return ArtistOrBandData;
        }

        // This method creates a wikipedia link either directly from a connection to wikipedia or from wikiData.
        // Inputs: 
        // musicBrainzData is the result data from the musicBrainz api and contains a link to wikiData and/or wikipedia. 
        // Return:
        // string that contains the wikipedia link.
        private async Task<string> GetWikipediaLink(MusicBrainzModel musicBrainzData)
        {
            string wikipediaLink;
            var wikpediaRelation = musicBrainzData.Relations.FirstOrDefault(musicBrainz => musicBrainz.Type == "wikipedia");
            if (wikpediaRelation != null)
            {
                // If a direct link exists the wikipediaLink is set to the correct url.
                wikipediaLink = wikpediaRelation.Url.Resource;
            }
            else
            {
                // If a direct link does not exist the wikipediaLink is set through the Wikidata link.
                var testtest = musicBrainzData.Relations.FirstOrDefault(musicBrainz => musicBrainz.Type == "wikidata");
                wikipediaLink = await CallsToServices.GetWikpediaIdFromWikiDataAsync(testtest.Url.Resource);
            }
            return wikipediaLink;
        }


    }
}