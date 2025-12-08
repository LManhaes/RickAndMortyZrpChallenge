using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RickAndMortyZrpChallenge.Application.Models;
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CharacterDetailDto>> GetCharacter(int id)
        {
            var character = await _service.GetCharacterAsync(id);
            if (character == null)
            {
                return NotFound();
            }

            return Ok(character);
        }
    }
}
