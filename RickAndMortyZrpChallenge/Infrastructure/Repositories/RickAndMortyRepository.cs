using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using RickAndMortyZrpChallenge.Domain.Entities;
using RickAndMortyZrpChallenge.Domain.Models;
using RickAndMortyZrpChallenge.Domain.Repositories;
using RickAndMortyZrpChallenge.External.RickAndMorty;

namespace RickAndMortyZrpChallenge.Infrastructure.Repositories
{
    public class RickAndMortyRepository : IRickAndMortyRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RickAndMortyRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient("RickAndMorty");
        }

        public async Task<EpisodePageResult> GetEpisodesAsync(int page, string? seasonCode)
        {
            var client = CreateClient();
            var query = $"episode?page={page}";
            if (!string.IsNullOrWhiteSpace(seasonCode))
            {
                query += $"&episode={Uri.EscapeDataString(seasonCode)}";
            }

            var response = await client.GetFromJsonAsync<PagedResponse<EpisodeResponse>>(query);

            if (response == null)
            {
                return new EpisodePageResult
                {
                    Page = page,
                    TotalPages = 0,
                    TotalCount = 0
                };
            }

            var episodes = response.Results
                .Select(MapEpisode)
                .ToList();

            return new EpisodePageResult
            {
                Page = page,
                TotalPages = response.Info.Pages,
                TotalCount = response.Info.Count,
                Episodes = episodes
            };
        }

        public async Task<Episode?> GetEpisodeByIdAsync(int id)
        {
            var client = CreateClient();
            var response = await client.GetFromJsonAsync<EpisodeResponse>($"episode/{id}");
            return response == null ? null : MapEpisode(response);
        }

        public async Task<IReadOnlyList<Character>> GetCharactersByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids.Distinct().ToList();
            if (idList.Count == 0)
            {
                return Array.Empty<Character>();
            }

            var client = CreateClient();

            if (idList.Count == 1)
            {
                var singleResponse =
                    await client.GetFromJsonAsync<CharacterResponse>($"character/{idList[0]}");
                if (singleResponse == null)
                {
                    return Array.Empty<Character>();
                }

                return new[] { MapCharacter(singleResponse) };
            }

            var joinedIds = string.Join(",", idList);
            var response =
                await client.GetFromJsonAsync<List<CharacterResponse>>($"character/{joinedIds}");

            if (response == null)
            {
                return Array.Empty<Character>();
            }

            return response.Select(MapCharacter).ToList();
        }

        public async Task<Character?> GetCharacterByIdAsync(int id)
        {
            var client = CreateClient();
            var response = await client.GetFromJsonAsync<CharacterResponse>($"character/{id}");
            return response == null ? null : MapCharacter(response);
        }

        private static Episode MapEpisode(EpisodeResponse response)
        {
            var characterIds = response.Characters
                .Select(TryGetIdFromUrl)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            return new Episode
            {
                Id = response.Id,
                Name = response.Name,
                AirDate = response.AirDate,
                EpisodeCode = response.Episode,
                CharacterIds = characterIds
            };
        }

        private static Character MapCharacter(CharacterResponse response)
        {
            return new Character
            {
                Id = response.Id,
                Name = response.Name,
                Status = response.Status,
                Species = response.Species,
                Type = response.Type,
                Gender = response.Gender,
                OriginName = response.Origin?.Name ?? string.Empty,
                LocationName = response.Location?.Name ?? string.Empty,
                ImageUrl = response.Image
            };
        }

        private static int? TryGetIdFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            var lastSegment = url.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            if (int.TryParse(lastSegment, out var id))
            {
                return id;
            }

            return null;
        }
    }
}
