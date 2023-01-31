using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
  public class Category
  {
    public int Id { get; set; }
    [Required]
    [MaxLength(20)]
    public string Name { get; set; }
    [Required]
    public bool Hidden { get; set; }
    public List<PokemonCategory> PokemonCategories { get; set; }
  }
}
