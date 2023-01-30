namespace server.Models
{
  public class Category
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Hidden { get; set; }
    public List<PokemonCategory> PokemonCategories { get; set; }
  }
}
