using MashupLogic.ArtistOrBand.Models;
using MashupLogic.ArtistOrBand.Models.Interfaces;
using MashupLogic.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace MashupLogic.ArtistOrBand
{
    // This class contains methods for querying the different apis.
    public class CallsToServices : ICallsToServices
    {
        // Starts an HTTPClient for the requests.
        private HttpClient Client { get; }
        public CallsToServices(IHttpClientFactory clientFactory)
        {
            Client = clientFactory.CreateClient();
        }

        // This method gets data from the MusicBrainz api.
        // Inputs: 
        // MBID is the identifier used to get data from the MusicBrainz api. 
        // requestId is an unique identifier for this request.
        // Return:
        // MusicBrainzModel that contains the data.
        public async Task<MusicBrainzModel> GetMusicBrainzDataAsync(string MBID, string requestId)
        {
            MusicBrainzModel reciveData;
            var request = new HttpRequestMessage(HttpMethod.Get,
            "https://musicbrainz.org/ws/2/artist/" + MBID + "?inc=url-rels+release-groups");
            request.Headers.Add("Accept", "application/json");
            // The User-Agent is set to the unique Id of this request since this api blocks too many calls from the same User-Agent.
            request.Headers.Add("User-Agent", "CodeTestId-" + requestId);

            HttpResponseMessage response = await Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                reciveData = await response.Content.ReadFromJsonAsync<MusicBrainzModel>();
            }
            else
            {
                throw new MissingCriticalDataException("There was an error calling the MusicBrainz api: " + response.ReasonPhrase + ". Check to make sure the MBID is a valid MBID identifier and connected to an artist or band");
            }
            return reciveData;
        }

        // This method creates a wikipedia link either directly from a connection to wikipedia or from wikiData.
        // Inputs: 
        // musicBrainzData is the result data from the musicBrainz api and contains a link to wikiData and/or wikipedia. 
        // Return:
        // string that contains the wikipedia link.
        public async Task<List<Album>> GetAlbumDataAsync(MusicBrainzModel reciveData)
        {
            List<Album> albumList = new List<Album>();

            List<Task<HttpResponseMessage>> responses = new List<Task<HttpResponseMessage>>();

            foreach (var item in reciveData.Releasegroups)
            {
                var request = new HttpRequestMessage(HttpMethod.Get,
               "http://coverartarchive.org" + "/release-group/" + item.Id);


                responses.Add(Client.SendAsync(request));
            }

            var results = await Task.WhenAll(responses);

            foreach (var response in results)
            {
                Album album = new Album();

               
                if (response.IsSuccessStatusCode)
                {
                    CoverArtModel coverArtData = await response.Content.ReadFromJsonAsync<CoverArtModel>();
                    album.ImageUrl = coverArtData.Images.First(coverArt => coverArt.Front == true).ImageUrl;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // A 404 in this case tells us no coverArt exists for this particular album but this should not throw an error since this is expected.
                }
                else
                {
                    throw new MissingSecondaryDataException("The album data could not be loaded correctly. Reason: " + response.ReasonPhrase);
                }
                albumList.Add(album);
            }
            return albumList;
        }

        // This method queries data from wikipedia by first getting a valid wikipediaLink and then sending a get request.
        // Inputs: 
        // musicBrainzData is the result data from the musicBrainz api and contains a link to wikiData and/or wikipedia. 
        // Return:
        // WikipediaModel that contains the wikipedia data.
        public async Task<WikipediaModel> GetWikipediaDataAsync(MusicBrainzModel musicBrainzData)
        {
            string wikipediaLink = await GetWikipediaLinkAsync(musicBrainzData);

            WikipediaModel wikipediaData;

            string wikipediaBandName = wikipediaLink.Replace("https://en.wikipedia.org/wiki/", "");
            var request = new HttpRequestMessage(HttpMethod.Get,
           "https://en.wikipedia.org/w/api.php?action=query&format=json&prop=extracts&exintro=true&redirects=true&titles=" + wikipediaBandName);

            HttpResponseMessage response = await Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                wikipediaData = await response.Content.ReadFromJsonAsync<WikipediaModel>();
            }
            else
            {
                throw new MissingSecondaryDataException("The Wikipedia api call failed therefore a description might not be available. Reason: " + response.ReasonPhrase);
            }
            return wikipediaData;
        }

        // This method creates a wikipedia link either directly from a connection to wikipedia or from wikiData.
        // Inputs: 
        // musicBrainzData is the result data from the musicBrainz api and contains a link to wikiData and/or wikipedia. 
        // Return:
        // string that contains the wikipedia link.
        private async Task<string> GetWikipediaLinkAsync(MusicBrainzModel musicBrainzData)
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
                wikipediaLink = await GetWikpediaIdFromWikiDataAsync(testtest.Url.Resource);
            }
            return wikipediaLink;
        }

        // This method creates a wikipedia link from wikiData
        // Inputs: 
        // wikiDataLink is the identifier needed to access the WikiData api for the correct band/artist. 
        // Return:
        // string that contains the wikipedia link.
        private async Task<string> GetWikpediaIdFromWikiDataAsync(string wikiDataLink)
        {
            string wikipediaLink = "";

            string wikiDataId = wikiDataLink.Replace("https://www.wikidata.org/wiki/", "");
            var request = new HttpRequestMessage(HttpMethod.Get,
           "https://www.wikidata.org/w/api.php?action=wbgetentities&ids=" + wikiDataId + "&format=json&props=sitelinks");

            HttpResponseMessage response = await Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                WikiDataModel wikiDataData = await response.Content.ReadFromJsonAsync<WikiDataModel>();
                // Check if we received a link to the respective english wikipedia site.
                if (wikiDataData.Entities.ContainsKey(wikiDataId))
                {
                    wikipediaLink = HttpUtility.UrlEncode(wikiDataData.Entities[wikiDataId].Sitelinks.Enwiki.Title);
                }
                else
                {
                    throw new MissingSecondaryDataException("The wikiData contains no connection to wikipedia therefore a description might not be available. Reason: " + response.ReasonPhrase);
                }
            }
            else
            {
                throw new MissingSecondaryDataException("The wikiData api call failed therefore a description might not be available. Reason: " + response.ReasonPhrase);
            }
            return wikipediaLink;
        }
    }
}
