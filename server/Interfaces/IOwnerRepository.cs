using server.Models;

namespace server.Interfaces
{
  public interface IOwnerRepository
  {
    Task<List<Owner>> GetOwnersAsync();
    Task<Owner> GetOwnerAsync(int ownerId);
    Task<List<Owner>> GetOwnerOfPokemonAsync(int pokemonId);
    Task<List<Pokemon>> GetPokemonByOwnerAsync(int ownerId);
    bool OwnerExist(int ownerId);
  }
}