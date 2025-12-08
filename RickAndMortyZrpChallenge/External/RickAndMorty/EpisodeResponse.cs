using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RickAndMortyZrpChallenge.External.RickAndMorty
{
    public class EpisodeResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("air_date")]
        public string AirDate { get; set; } = string.Empty;

        [JsonPropertyName("episode")]
        public string Episode { get; set; } = string.Empty;

        [JsonPropertyName("characters")]
        public List<string> Characters { get; set; } = new();

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("created")]
        public string Created { get; set; } = string.Empty;
    }
}
