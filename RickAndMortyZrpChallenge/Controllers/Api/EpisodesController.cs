using Microsoft.AspNetCore.Mvc;
using RickAndMortyZrpChallenge.Application.Models.Requests;
using RickAndMortyZrpChallenge.Application.Services;

namespace RickAndMortyZrpChallenge.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class EpisodesController : ControllerBase
    {
        private readonly IRickAndMortyService _service;

        public EpisodesController(IRickAndMortyService service)
        {
            _service = service;
        }

        // GET api/episodes?page=1&season=1
        [HttpGet]
        public async Task<IActionResult> GetEpisodes([FromQuery] GetEpisodesRequest request)
        {
            // Se request for inválido, FluentValidation + [ApiController]
            // já geram HTTP 400 antes de chegar aqui.

            var result = await _service.GetEpisodesAsync(request.Page, request.Season);
            return Ok(result);
        }

        // GET api/episodes/{episodeId}/characters
        [HttpGet("{episodeId:int}/characters")]
        public async Task<IActionResult> GetCharactersByEpisode(int episodeId)
        {
            var characters = await _service.GetCharactersByEpisodeAsync(episodeId);
            return Ok(characters);
        }
    }
}
