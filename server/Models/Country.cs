namespace server.Models
{
  public class Country
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Hidden { get; set; }
    public List<Owner> Owners { get; set; }
  }
}
