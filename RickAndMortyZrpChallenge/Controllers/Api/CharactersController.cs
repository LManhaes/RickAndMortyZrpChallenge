using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RickAndMortyZrpChallenge.Application.Models;
using RickAndMortyZrpChallenge.Application.Models.Requests;
using RickAndMortyZrpChallenge.Application.Services;

namespace RickAndMortyZrpChallenge.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharactersController : ControllerBase
    {
        private readonly IRickAndMortyService _service;

        public CharactersController(IRickAndMortyService service)
        {
            _service = service;
        }

        // GET api/characters/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CharacterDetailDto>> GetCharacter([FromRoute] GetCharacterRequest request)
        {
            // Se o Id for <= 0, FluentValidation + [ApiController]
            // já terão retornado HTTP 400 antes de chegar aqui.

            var character = await _service.GetCharacterAsync(request.Id);
            if (character == null)
            {
                return NotFound();
            }

            return Ok(character);
        }
    }
}
