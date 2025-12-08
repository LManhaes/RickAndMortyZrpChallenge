using System.Collections.Generic;

namespace RickAndMortyZrpChallenge.External.RickAndMorty
{
    public class PagedResponse<T>
    {
        public Info Info { get; set; } = new Info();
        public List<T> Results { get; set; } = new();
    }
}
