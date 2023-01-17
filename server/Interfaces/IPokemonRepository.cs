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
    ICollection<Pokemon> GetPokemons();
    Pokemon GetPokemon(int id);
    Pokemon GetPokemon(string name);
    decimal GetPokemonRating(int pokemonId);
    bool PokemonExists(int pokemonId);
  }
}