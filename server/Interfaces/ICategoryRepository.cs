using server.Models;

namespace server.Interfaces
{
  public interface ICategoryRepository
  {
    ICollection<Category> GetCategories();
    Category GetCategory(int categoryId);
    ICollection<Pokemon> GetPokemonByCategories(int categoryId);
    bool CategoryExists(int categoryId);
  }
}