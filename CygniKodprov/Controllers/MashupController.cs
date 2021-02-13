using CygniKodprov.Mashup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CygniKodprov.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MashupController : ControllerBase
    {
        private readonly ILogger<MashupController> _logger;
        private readonly MashupLogic mashup;
        public MashupController(ILogger<MashupController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            mashup = new MashupLogic(httpClientFactory);
        }

        [HttpGet]
        public async Task<string> GetAsync(string MBID)
        {
            await mashup.StartApiAsync(MBID);
            return "Hej";
        }
    }
}
