using CygniKodprov.Mashup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

            var request = new HttpRequestMessage(HttpMethod.Get,
           "https://musicbrainz.org/ws/2/artist/" + "c2764f38-febf-4e47-a0d7-687980aabf38?inc=url-rels+release-groups");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "CodeTest");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = response.Content;
                var test = JsonSerializer.Deserialize<ReciveModel>(responseStream.ToString());
                Console.WriteLine(test);
            }
            else
            {
               //Return error
            }
        }
    }
}
