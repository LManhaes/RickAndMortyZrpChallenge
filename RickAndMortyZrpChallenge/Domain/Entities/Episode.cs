using System.Collections.Generic;

namespace RickAndMortyZrpChallenge.Domain.Entities
{
    public class Episode
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AirDate { get; set; } = string.Empty;
        public string EpisodeCode { get; set; } = string.Empty;
        public List<int> CharacterIds { get; set; } = new();
    }
}
