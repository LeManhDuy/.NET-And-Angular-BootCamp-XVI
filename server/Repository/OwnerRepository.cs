using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Interfaces;
using server.Models;

namespace server.Repository
{
  public class OwnerRepository : IOwnerRepository
  {
    private readonly DataContext _context;

    public OwnerRepository(DataContext context)
    {
      _context = context;
    }
    
    public async Task<List<Owner>> GetOwnersAsync()
    {
      return await _context.Owners.Where(o => o.Hidden == false).OrderBy(o => o.Name).ToListAsync();
    }

    public async Task<Owner> GetOwnerAsync(int ownerId)
    {
      return await _context.Owners.Where(o => o.Id == ownerId && o.Hidden == false).OrderBy(o => o.Name).FirstOrDefaultAsync();
    }

    public async Task<List<Owner>> GetOwnerOfPokemonAsync(int pokemonId)
    {
      return await _context.PokemonOwners.Where(po => po.PokemonId == pokemonId).Select(po => po.Owner).ToListAsync();
    }

    public async Task<List<Pokemon>> GetPokemonByOwnerAsync(int ownerId)
    {
      return await _context.PokemonOwners.Where(po => po.OwnerId == ownerId).Select(po => po.Pokemon).ToListAsync();
    }

    public bool OwnerExist(int ownerId)
    {
      return _context.Owners.Any(o => o.Id == ownerId && o.Hidden == false);
    }
  }
}