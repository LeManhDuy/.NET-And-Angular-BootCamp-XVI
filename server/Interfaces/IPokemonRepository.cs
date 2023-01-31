using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Dto;
using server.Models;

namespace server.Interfaces
{
  public interface IPokemonRepository
  {
    // interface-namespace
    Task<List<PokemonDto>> GetPokemonsAsync();
    Task<Pokemon> GetPokemonAsync(int pokemonId);
    Task<Pokemon> GetPokemonAsync(string name);
    decimal GetPokemonRating(int pokemonId);
    bool PokemonExists(int pokemonId);
    bool PokemonExists(string pokemonName);
    Task<PokemonDto> CreateAsync(int ownerId, int categoryId, PokemonDto pokemonDto);
  }
}