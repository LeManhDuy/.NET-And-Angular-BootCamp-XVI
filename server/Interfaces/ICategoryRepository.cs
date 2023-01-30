using server.Models;

namespace server.Interfaces
{
  public interface ICategoryRepository
  {
    Task<List<Category>> GetCategoriesAsync();
    Task<Category> GetCategoryAsync(int categoryId);
    Task<List<Pokemon>> GetPokemonByCategoriesAsync(int categoryId);
    bool CategoryExists(int categoryId);
  }
}