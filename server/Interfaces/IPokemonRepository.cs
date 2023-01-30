using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Models;

namespace server.Interfaces
{
  public interface IPokemonRepository
  {
    // interface-namespace
    Task<List<Pokemon>> GetPokemonsAsync();
    Task<Pokemon> GetPokemonAsync(int pokemonId);
    Task<Pokemon> GetPokemonAsync(string name);
    decimal GetPokemonRating(int pokemonId);
    bool PokemonExists(int pokemonId);
  }
}