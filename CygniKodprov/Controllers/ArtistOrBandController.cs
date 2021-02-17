using MashupLogic.ArtistOrBand.Models;
using MashupLogic.ArtistOrBand.Models.Interfaces;
using MashupLogic.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MashupAPI.Controllers
{
    [Route("Mashup/[controller]")]
    [ApiController]
    public class ArtistOrBandController : ControllerBase
    {
        private IArtistOrBandLogic ArtistOrBandLogic { get; }
        public ArtistOrBandController(IArtistOrBandLogic artistOrBandLogic)
        {
            ArtistOrBandLogic = artistOrBandLogic;
        }

        [HttpGet]
        public async Task<ActionResult<ArtistOrBandModel>> GetAsync(string MBID)
        {
            // Checks that the input string is a valid Guid otherwise returns the BadRequest(500) status code.
            if (!Guid.TryParse(MBID, out _))
            {
                return BadRequest("The input string is not a valid MBID identifier");
            }

            // Creates a unique Id for this request which  can be used in logging and as a tool to avoid some api call limits. 
            string requestId = Guid.NewGuid().ToString();

            // Runs the main logic of the request and handles exceptions.
            ArtistOrBandModel artistOrBandData;
            try
            {
                artistOrBandData = await ArtistOrBandLogic.CreateArtistOrBandMashupAsync(MBID, requestId);
            }
            catch (MissingCriticalDataException ex)
            {
                // Returns a 404 if the main lookup query fails.
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Returns a 500 if an otherwise unhandled exception is thrown.
                return StatusCode(500, ex.Message);
            }
            return artistOrBandData;
        }
    }
}
