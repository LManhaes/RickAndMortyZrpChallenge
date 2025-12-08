using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RickAndMortyZrpChallenge.Application.Models;
using RickAndMortyZrpChallenge.Application.Services;

namespace RickAndMortyZrpChallenge.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class EpisodesController : ControllerBase
    {
        private readonly IRickAndMortyService _service;

        public EpisodesController(IRickAndMortyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResultDto<EpisodeDto>>> GetEpisodes(
            [FromQuery] int page = 1,
            [FromQuery] int? season = null)
        {
            if (page <= 0)
            {
                page = 1;
            }

            var result = await _service.GetEpisodesAsync(page, season);
            return Ok(result);
        }

        [HttpGet("{id:int}/characters")]
        public async Task<ActionResult<IEnumerable<CharacterSummaryDto>>> GetCharactersForEpisode(int id)
        {
            var characters = await _service.GetCharactersByEpisodeAsync(id);
            return Ok(characters);
        }
    }
}
