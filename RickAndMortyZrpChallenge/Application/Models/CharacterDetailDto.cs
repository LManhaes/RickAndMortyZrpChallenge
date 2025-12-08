namespace RickAndMortyZrpChallenge.Application.Models
{
    public class CharacterDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string OriginName { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
    }
}
