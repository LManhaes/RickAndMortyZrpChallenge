namespace RickAndMortyZrpChallenge.Application.Models
{
    public class EpisodeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EpisodeCode { get; set; } = string.Empty;
        public string AirDate { get; set; } = string.Empty;
        public int CharacterCount { get; set; }
    }
}
