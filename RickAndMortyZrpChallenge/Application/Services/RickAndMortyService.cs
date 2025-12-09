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
            // ==== Sanitização / validação dos parâmetros numéricos ====
            page = SanitizePage(page);
            season = SanitizeSeason(season);

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
            episodeId = SanitizeId(episodeId);

            if (episodeId <= 0)
            {
                // Parâmetro inválido → resposta vazia controlada
                return Enumerable.Empty<CharacterSummaryDto>();
            }

            var episode = await _repository.GetEpisodeByIdAsync(episodeId);
            if (episode == null || episode.CharacterIds.Count == 0)
            {
                return Enumerable.Empty<CharacterSummaryDto>();
            }

            var characters = await _repository.GetCharactersByIdsAsync(episode.CharacterIds);

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
            characterId = SanitizeId(characterId);

            if (characterId <= 0)
            {
                return null;
            }

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
       

        private static int SanitizePage(int page)
        {
            // Nunca deixa page < 1
            return page <= 0 ? 1 : page;
        }

        private static int? SanitizeSeason(int? season)
        {
            if (!season.HasValue)
                return null;

            // Exemplo: só considera seasons entre 1 e 9, fora disso vira null
            if (season.Value <= 0 || season.Value > 9)
                return null;

            return season;
        }

        private static int SanitizeId(int id)
        {
            // IDs negativos ou zero são tratados como inválidos
            return id <= 0 ? 0 : id;
        }
        
        public static string SanitizeString(string? value, int maxLength = 200)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var sanitized = value.Trim();

            // Remove caracteres de controle (inclusive quebras "estranhas")
            sanitized = new string(sanitized.Where(c => !char.IsControl(c)).ToArray());

            // Remove sinais de tag básica, reduz risco de XSS na hora de exibir em HTML
            sanitized = sanitized
                .Replace("<", string.Empty)
                .Replace(">", string.Empty);

            if (sanitized.Length > maxLength)
                sanitized = sanitized[..maxLength];

            return sanitized;
        }
    }
}
