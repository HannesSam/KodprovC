using CygniKodprovApp.ArtistOrBand.Models;
using CygniKodprovApp.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace CygniKodprovApp.ArtistOrBand
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
        // MBID is the identifyer used to get data from the MusicBrainz api. 
        // requestId is an unique idntifyer for this request.
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

            foreach (var release in reciveData.Releasegroups)
            {
                Album album = new Album();
                album.Id = release.Id;
                album.Title = release.Title;

                var request = new HttpRequestMessage(HttpMethod.Get,
                "http://coverartarchive.org" + "/release-group/" + album.Id);

                HttpResponseMessage response = await Client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    CoverArtModel coverArtData = await response.Content.ReadFromJsonAsync<CoverArtModel>();
                    album.ImageUrl = coverArtData.Images.First(coverArt => coverArt.Front == true).ImageUrl;
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {

                }
                else
                {
                    throw new MissingSecondaryDataException("The album data could not be loaded correctly. Reason: " + response.ReasonPhrase);
                }
                albumList.Add(album);
            }
            return albumList;
        }

        // This method creates a wikipedia link either directly from a connection to wikipedia or from wikiData.
        // Inputs: 
        // musicBrainzData is the result data from the musicBrainz api and contains a link to wikiData and/or wikipedia. 
        // Return:
        // string that contains the wikipedia link.
        public async Task<string> GetWikpediaIdFromWikiDataAsync(string wikiDataLink)
        {
            string wikipediaLink = "";

            string wikiDataId = wikiDataLink.Replace("https://www.wikidata.org/wiki/", "");
            var request = new HttpRequestMessage(HttpMethod.Get,
           "https://www.wikidata.org/w/api.php?action=wbgetentities&ids=" + wikiDataId + "&format=json&props=sitelinks");

            HttpResponseMessage response = await Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                WikiDataModel wikiDataData = await response.Content.ReadFromJsonAsync<WikiDataModel>();
                if (wikiDataData.Entities.ContainsKey(wikiDataId))
                {
                    wikipediaLink = HttpUtility.UrlEncode(wikiDataData.Entities[wikiDataId].Sitelinks.Enwiki.Title);
                }
                else
                {
                    throw new MissingSecondaryDataException("The wikiData contains no connection therefore a descritpion might not be availible. Reason: " + response.ReasonPhrase);
                }
            }
            else
            {
                throw new MissingSecondaryDataException("The wikiData api call failed therfore a description might not be availible. Reason: " + response.ReasonPhrase);
            }
            return wikipediaLink;
        }

        // This method creates a wikipedia link either directly from a connection to wikipedia or from wikiData.
        // Inputs: 
        // musicBrainzData is the result data from the musicBrainz api and contains a link to wikiData and/or wikipedia. 
        // Return:
        // string that contains the wikipedia link.
        public async Task<WikipediaModel> GetWikipediaDataAsync(string wikipediaLink)
        {
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
                throw new MissingSecondaryDataException("The Wikipeida api call failed therfore a description might not be availible. Reason: " + response.ReasonPhrase);
            }
            return wikipediaData;
        }
    }
}
