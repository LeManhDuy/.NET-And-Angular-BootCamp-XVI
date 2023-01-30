using server.Data;
using server.Interfaces;
using server.Models;
using Microsoft.EntityFrameworkCore;
namespace server.Repository
{
  public class PokemonRepository : IPokemonRepository
  {
    // dp-repo
    // ctor
    private readonly DataContext _context;
    public PokemonRepository(DataContext context)
    {
      _context = context;
    }

    public async Task<List<Pokemon>> GetPokemonsAsync()
    {
      return await _context.Pokemons.Where(p => p.Hidden == false)
                                    .Include(p => p.PokemonCategories)
                                    .ToListAsync();
    }

    public async Task<Pokemon> GetPokemonAsync(int pokemonId)
    {
      return await _context.Pokemons.Where(p => p.Hidden == false && p.Id == pokemonId)
                                    .FirstOrDefaultAsync();
    }

    public async Task<Pokemon> GetPokemonAsync(string name)
    {
      return await _context.Pokemons.Where(p => p.Hidden == false && p.Name == name)
                                    .FirstOrDefaultAsync();
    }

    public decimal GetPokemonRating(int pokemonId)
    {
      var reviews = _context.Reviews.Where(r => r.Pokemon.Id == pokemonId);
      if (reviews.Count() <= 0)
      {
        return 0;
      }
      return ((decimal)reviews.Sum(r => r.Rating) / reviews.Count());
    }

    public bool PokemonExists(int pokemonId)
    {
      return _context.Pokemons.Any(p => p.Hidden == false && p.Id == pokemonId);
    }
  }
}