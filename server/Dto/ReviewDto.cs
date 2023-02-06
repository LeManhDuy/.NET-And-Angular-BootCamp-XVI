using server.Models;

namespace api.Dto
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public bool Hidden { get; set; }
        public string ReviewerName { get; set; }
        public string PokemonName { get; set; }
    }
}
