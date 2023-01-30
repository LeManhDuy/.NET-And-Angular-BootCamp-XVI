using server.Data;
using server.Interfaces;
using server.Models;
using Microsoft.EntityFrameworkCore;

namespace server.Repository
{
  public class CategoryRepository : ICategoryRepository
  {
    private readonly DataContext _context;

    public CategoryRepository(DataContext context)
    {
      _context = context;
    }

    public bool CategoryExists(int categoryId)
    {
      return _context.Categories.Any(c => c.Id == categoryId && c.Hidden == false);
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
      return await _context.Categories.Where(c => c.Hidden == false).OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Category> GetCategoryAsync(int categoryId)
    {
      return await _context.Categories.Where(c => c.Id == categoryId && c.Hidden == false).OrderBy(c => c.Name).FirstOrDefaultAsync();
    }

    public async Task<List<Pokemon>> GetPokemonByCategoriesAsync(int categoryId)
    {
      return await _context.PokemonCategories.Where(pc => pc.CategoryId == categoryId).Select(pc => pc.Pokemon).ToListAsync();
    }
  }
}