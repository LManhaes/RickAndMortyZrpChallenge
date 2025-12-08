using System.Collections.Generic;
using RickAndMortyZrpChallenge.Domain.Entities;

namespace RickAndMortyZrpChallenge.Domain.Models
{
    public class EpisodePageResult
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public List<Episode> Episodes { get; set; } = new();
    }
}
