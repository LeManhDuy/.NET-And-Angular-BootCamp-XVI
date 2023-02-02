using server.Models;

namespace server.Dto
{
    public class PokemonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Hidden { get; set; }
        public List<string> CategoryName { get; set; }
    }
}