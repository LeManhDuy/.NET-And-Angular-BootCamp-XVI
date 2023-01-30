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

    public ICollection<Category> GetCategories()
    {
      return _context.Categories.Where(c => c.Hidden == false).OrderBy(c => c.Name).ToList();
    }

    public Category GetCategory(int categoryId)
    {
      return _context.Categories.Where(c => c.Id == categoryId && c.Hidden == false).OrderBy(c => c.Name).FirstOrDefault();
    }

    public ICollection<Pokemon> GetPokemonByCategories(int categoryId)
    {
      return _context.PokemonCategories.Where(pc => pc.CategoryId == categoryId).Select(pc => pc.Pokemon).ToList();
    }
  }
}