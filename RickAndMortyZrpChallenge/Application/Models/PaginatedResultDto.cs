using System.Collections.Generic;

namespace RickAndMortyZrpChallenge.Application.Models
{
    public class PaginatedResultDto<T>
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public List<T> Items { get; set; } = new();
    }
}
