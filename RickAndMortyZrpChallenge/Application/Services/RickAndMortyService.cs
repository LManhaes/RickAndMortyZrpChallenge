using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RickAndMortyZrpChallenge.Application.Models;
using RickAndMortyZrpChallenge.Domain.Repositories;

namespace RickAndMortyZrpChallenge.Application.Services
{
    public class RickAndMortyService : IRickAndMortyService
    {
        private readonly IRickAndMortyRepository _repository;

        public RickAndMortyService(IRickAndMortyRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResultDto<EpisodeDto>> GetEpisodesAsync(int page, int? season)
        {
            var seasonCode = season.HasValue ? $"S0{season.Value}" : null;

            var result = await _repository.GetEpisodesAsync(page, seasonCode);

            var dto = new PaginatedResultDto<EpisodeDto>
            {
                Page = result.Page,
                TotalPages = result.TotalPages,
                TotalCount = result.TotalCount,
                Items = result.Episodes.Select(e => new EpisodeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    EpisodeCode = e.EpisodeCode,
                    AirDate = e.AirDate,
                    CharacterCount = e.CharacterIds.Count
                }).ToList()
            };

            return dto;
        }

        public async Task<IEnumerable<CharacterSummaryDto>> GetCharactersByEpisodeAsync(int episodeId)
        {
            var episode = await _repository.GetEpisodeByIdAsync(episodeId);
            if (episode == null || episode.CharacterIds.Count == 0)
            {
                return Enumerable.Empty<CharacterSummaryDto>();
            }

            var characters =
                await _repository.GetCharactersByIdsAsync(episode.CharacterIds);

            return characters.Select(c => new CharacterSummaryDto
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl,
                Status = c.Status,
                Species = c.Species,
                OriginName = c.OriginName,
                LocationName = c.LocationName,
                Gender = c.Gender          
            });


        }

        public async Task<CharacterDetailDto?> GetCharacterAsync(int characterId)
        {
            var character = await _repository.GetCharacterByIdAsync(characterId);
            if (character == null)
            {
                return null;
            }

            return new CharacterDetailDto
            {
                Id = character.Id,
                Name = character.Name,
                ImageUrl = character.ImageUrl,
                Status = character.Status,
                Species = character.Species,
                Type = character.Type,
                Gender = character.Gender,
                OriginName = character.OriginName,
                LocationName = character.LocationName
            };
        }
    }
}
