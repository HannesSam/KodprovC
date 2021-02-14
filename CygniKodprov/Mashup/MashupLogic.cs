using CygniKodprov.Mashup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace CygniKodprov.Mashup
{
    public class MashupLogic
    {
        private readonly IHttpClientFactory _clientFactory;

        public MashupLogic(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task StartApiAsync(string MBID)
        {
            ReciveModel reciveData;
            var request = new HttpRequestMessage(HttpMethod.Get,
           "https://musicbrainz.org/ws/2/artist/" + MBID + "?inc=url-rels+release-groups");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "CodeTest");

            var client = _clientFactory.CreateClient();

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                reciveData = await response.Content.ReadFromJsonAsync<ReciveModel>();
                Console.WriteLine(reciveData);
            }
            else
            {
                //Return error
                throw new Exception("There was an error calling the api: " + response.ReasonPhrase);
            }


        }
    }
}
