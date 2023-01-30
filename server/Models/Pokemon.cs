namespace server.Models
{
  public class Pokemon
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public bool Hidden { get; set; }
    public List<Review> Reviews { get; set; }
    public List<PokemonOwner> PokemonOwners { get; set; }
    public List<PokemonCategory> PokemonCategories { get; set; }
  }
}
