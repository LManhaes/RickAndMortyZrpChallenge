namespace RickAndMortyZrpChallenge.Application.Models.Requests
{
    public sealed class GetEpisodesRequest
    {        
        public int Page { get; set; } = 1;       
        public int? Season { get; set; }
    }
}
