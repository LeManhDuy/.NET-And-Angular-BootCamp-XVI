namespace server.Models
{
  public class Owner
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Gym { get; set; }
    public bool Hidden { get; set; }
    public Country Country { get; set; }
    public List<PokemonOwner> PokemonOwners { get; set; }
  }
}
