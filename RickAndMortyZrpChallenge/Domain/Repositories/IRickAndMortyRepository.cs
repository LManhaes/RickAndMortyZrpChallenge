using System.Collections.Generic;
using System.Threading.Tasks;
using RickAndMortyZrpChallenge.Domain.Entities;
using RickAndMortyZrpChallenge.Domain.Models;

namespace RickAndMortyZrpChallenge.Domain.Repositories
{
    public interface IRickAndMortyRepository
    {
        Task<EpisodePageResult> GetEpisodesAsync(int page, string? seasonCode);
        Task<Episode?> GetEpisodeByIdAsync(int id);
        Task<IReadOnlyList<Character>> GetCharactersByIdsAsync(IEnumerable<int> ids);
        Task<Character?> GetCharacterByIdAsync(int id);
    }
}
