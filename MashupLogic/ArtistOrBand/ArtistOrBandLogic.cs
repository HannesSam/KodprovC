using MashupLogic.ArtistOrBand.Models;
using MashupLogic.ArtistOrBand.Models.Interfaces;
using MashupLogic.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MashupLogic.ArtistOrBand
{
    // This class contains the main logic for the ArtistOrBand mashup.
    public class ArtistOrBandLogic : IArtistOrBandLogic
    {
        // Property that holds the main data model for the class that will hold all the return data.
        private ArtistOrBandModel ArtistOrBandData { get; }
        // Property that holds the helper class for api calls to the different services.
        private ICallsToServices CallsToServices { get; }
        public ArtistOrBandLogic(ICallsToServices callsToServices)
        {
            CallsToServices = callsToServices;
            // Creates a new data model for the return data and creates an empty list of albums and sets the success code to the default value 1(success). 
            ArtistOrBandData = new ArtistOrBandModel
            {
                Albums = new List<Album>(),
                SuccessCode = 1
            };
        }

        // This method starts the creation of the ArtistAndBand mashup.
        // Inputs: 
        // MBID is the identifier  used to get the initial data from MusicBrainz. 
        // requestId is an unique identifier  for this request.
        // Return:
        // ArtistOrBandModel contains the completed mashup.
        public async Task<ArtistOrBandModel> CreateArtistOrBandMashupAsync(string MBID, string requestId)
        {

            // Get the data from the MusicBrainz api.
            MusicBrainzModel musicBrainzData = await CallsToServices.GetMusicBrainzDataAsync(MBID, requestId);

            // Start the call to get data from wikipedia and CoverArt.
            Task<List<Album>> albumListTask = CallsToServices.GetAlbumDataAsync(musicBrainzData);
            Task<WikipediaModel> wikipediaDataTask; wikipediaDataTask = CallsToServices.GetWikipediaDataAsync(musicBrainzData);

            // Await the completion of the tasks and load in the data as well as handle one exception that the tasks could throw.
            WikipediaModel wikipediaData;
            List<Album> albumList;
            try
            {
                wikipediaData = await wikipediaDataTask;
            }
            catch (MissingSecondaryDataException ex)
            {
                // If the process of collecting  this data fails sets the success code to 2(Partial success) and logs the error in the return object.
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
                // If the process of collecting  this data fails sets the success code to 2(Partial success) and logs the error in the return object.
                ArtistOrBandData.SuccessCode = 2;
                ArtistOrBandData.ErrorNotes += ex.Message;
                albumList = new List<Album>();
            }

            // Assigns all outgoing variables to the return object.
            ArtistOrBandData.Id = MBID;
            ArtistOrBandData.Name = musicBrainzData.Name;
            // Maybe an error check should be done here but if the wikipedia query succeeds an empty pages list should not be possible.
            ArtistOrBandData.Description = wikipediaData.Query.Pages.FirstOrDefault().Value.Extract;
            ArtistOrBandData.Albums = albumList;

            return ArtistOrBandData;
        }




    }
}