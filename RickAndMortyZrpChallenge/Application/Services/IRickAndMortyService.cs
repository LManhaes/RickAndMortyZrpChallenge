using System.Collections.Generic;
using System.Threading.Tasks;
using RickAndMortyZrpChallenge.Application.Models;

namespace RickAndMortyZrpChallenge.Application.Services
{
    public interface IRickAndMortyService
    {
        Task<PaginatedResultDto<EpisodeDto>> GetEpisodesAsync(int page, int? season);
        Task<IEnumerable<CharacterSummaryDto>> GetCharactersByEpisodeAsync(int episodeId);
        Task<CharacterDetailDto?> GetCharacterAsync(int characterId);
    }
}
