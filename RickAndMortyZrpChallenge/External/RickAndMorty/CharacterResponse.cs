using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RickAndMortyZrpChallenge.External.RickAndMorty
{
    public class CharacterResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("species")]
        public string Species { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("gender")]
        public string Gender { get; set; } = string.Empty;

        [JsonPropertyName("origin")]
        public OriginLocation Origin { get; set; } = new OriginLocation();

        [JsonPropertyName("location")]
        public OriginLocation Location { get; set; } = new OriginLocation();

        [JsonPropertyName("image")]
        public string Image { get; set; } = string.Empty;

        [JsonPropertyName("episode")]
        public List<string> EpisodeUrls { get; set; } = new List<string>();

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("created")]
        public string Created { get; set; } = string.Empty;
    }

    public class OriginLocation
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}
