using server.Models;

namespace server.Interfaces
{
  public interface ICategoryRepository
  {
    ICollection<Category> GetCategories();
    Category GetCategory(int id);
    ICollection<Pokemon> GetPokemonByCategories(int categoryId);
    bool CategoryExists(int id);
  }
}