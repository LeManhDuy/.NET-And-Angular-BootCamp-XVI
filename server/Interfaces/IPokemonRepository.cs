using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Filter;
using server.Dto;
using server.Models;

namespace server.Interfaces
{
    public interface IPokemonRepository
    {
        //custom respone
        //update tach lam 3 phan
        // interface-namespace
        Task<List<PokemonDto>> GetPokemonsAsync(PaginationFilter filter);
        Task<PokemonDto> GetPokemonAsync(int pokemonId);
        Task<Pokemon> GetPokemonAsync(string name);
        decimal GetPokemonRating(int pokemonId);
        bool PokemonExists(int pokemonId);
        bool PokemonExists(string pokemonName);
        Task<PokemonDto> CreateAsync(int[] ownerId, int[] categoryId, PokemonDto pokemonDto);
        Task<PokemonDto> UpdatePokemonAsync(int pokemonId, PokemonDto pokemonDto);
        Task<bool> UpdatePokemonCategoryAsync(int pokemonId, int[] categoryId);
        Task<bool> UpdatePokemonOwnerAsync(int pokemonId, int[] ownerId);
        Task ArchiveAsync(int pokemonId);
        Task MultiArchiveAsync(int[] pokemonId);
        Task DeleteAsync(int pokemonId);
        Task MultiDeleteAsync(int[] pokemonId);

    }
}