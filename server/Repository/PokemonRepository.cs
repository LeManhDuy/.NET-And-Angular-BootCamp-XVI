using server.Data;
using server.Interfaces;
using server.Models;
using Microsoft.EntityFrameworkCore;
using server.Dto;
using AutoMapper;
using System.Linq;

namespace server.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        // dp-repo
        // ctor
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public PokemonRepository(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<List<PokemonDto>> GetPokemonsAsync()
        {
            return await _context.Pokemons.Where(p => p.Hidden == false)
                                          .Include(p => p.PokemonCategories)
                                          .ThenInclude(pc => pc.Category)
                                          .Select(p => new PokemonDto
                                          {
                                              Id = p.Id,
                                              Name = p.Name,
                                              BirthDate = p.BirthDate,
                                              CategoryName = p.PokemonCategories.FirstOrDefault().Category.Name
                                          })
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

        public bool PokemonExists(string pokemonName)
        {
            return _context.Pokemons.Any(p => p.Hidden == false && p.Name.ToLower() == pokemonName.ToLower());
        }

        public async Task<PokemonDto> CreateAsync(int ownerId, int categoryId, PokemonDto pokemonDto)
        {
            try
            {
                var pokemon = _mapper.Map<Pokemon>(pokemonDto);
                if (pokemon == null)
                {
                    throw new Exception("Pokemon not found!");
                }

                var pokemonOwnerEntity = await _context.Owners.Where(o => o.Id == ownerId).FirstOrDefaultAsync();
                if (pokemonOwnerEntity != null)
                {
                    var pokemonOwner = new PokemonOwner()
                    {
                        Owner = pokemonOwnerEntity,
                        Pokemon = pokemon,
                    };
                    await _context.AddAsync(pokemonOwner);
                }

                var pokemonCategoryEntity = await _context.Categories.Where(c => c.Id == categoryId).FirstOrDefaultAsync();
                if (pokemonCategoryEntity != null)
                {
                    var pokemonCategory = new PokemonCategory()
                    {
                        Category = pokemonCategoryEntity,
                        Pokemon = pokemon,
                    };
                    await _context.AddAsync(pokemonCategory);
                }

                await _context.SaveChangesAsync();
                return pokemonDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdatePokemonCategoryAsync(int pokemonId, int categoryId)
        {
            try
            {
                var pokemon = await _context.Pokemons.FindAsync(pokemonId);
                var category = await _context.Categories.FindAsync(categoryId);
                if (pokemon == null || category == null)
                {
                    return false;
                }

                var pokemonCategory = await _context.PokemonCategories.Where(pc => pc.PokemonId == pokemonId && pc.CategoryId == categoryId).FirstOrDefaultAsync();

                if (pokemonCategory != null)
                {
                    pokemonCategory.PokemonId = pokemonId;
                    pokemonCategory.CategoryId = 1;
                    _context.PokemonCategories.Update(pokemonCategory);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PokemonDto> UpdatePokemonAsync(int pokemonId, PokemonDto pokemonDto)
        {
            try
            {
                var pokemon = await _context.Pokemons.FindAsync(pokemonId);

                if (pokemon == null)
                {
                    throw new Exception("Pokemon not found!");
                }

                _mapper.Map(pokemonDto, pokemon);
                _context.Pokemons.Update(pokemon);
                await _context.SaveChangesAsync();

                return pokemonDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task ArchiveAsync(int pokemonId)
        {
            try
            {
                var pokemon = await _context.Pokemons.FindAsync(pokemonId);

                if (pokemon == null)
                {
                    throw new Exception("Pokemon not found!");
                }

                pokemon.Hidden = !pokemon.Hidden;
                _context.Pokemons.Update(pokemon);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task MultiArchiveAsync(int[] pokemonId)
        {
            try
            {
                var pokemons = await _context.Pokemons.Where(c => pokemonId.Contains(c.Id)).ToListAsync();
                foreach (var pokemon in pokemons)
                {
                    pokemon.Hidden = !pokemon.Hidden;
                }
                _context.Pokemons.UpdateRange(pokemons);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteAsync(int pokemonId)
        {
            try
            {
                var pokemon = await _context.Pokemons.FindAsync(pokemonId);
                if (pokemon == null)
                {
                    throw new Exception("Pokemon not found!");
                }
                _context.Pokemons.Remove(pokemon);

                var pokemonsOwner = await _context.PokemonOwners.Where(c => c.PokemonId == pokemonId).ToListAsync();
                if (pokemonsOwner == null)
                {
                    throw new Exception("Pokemon owner not found!");
                }
                _context.PokemonOwners.RemoveRange(pokemonsOwner);

                var pokemonsCategory = await _context.PokemonCategories.Where(c => c.PokemonId == pokemonId).ToListAsync();
                if (pokemonsCategory == null)
                {
                    throw new Exception("Pokemon category not found!");
                }
                _context.PokemonCategories.RemoveRange(pokemonsCategory);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task MultiDeleteAsync(int[] pokemonId)
        {
            try
            {
                var pokemons = await _context.Pokemons.Where(c => pokemonId.Contains(c.Id)).ToListAsync();
                _context.Pokemons.RemoveRange(pokemons);

                var pokemonsOwner = await _context.PokemonOwners.Where(c => pokemonId.Contains(c.PokemonId)).ToListAsync();
                _context.PokemonOwners.RemoveRange(pokemonsOwner);

                var pokemonsCategory = await _context.PokemonCategories.Where(c => pokemonId.Contains(c.PokemonId)).ToListAsync();
                _context.PokemonCategories.RemoveRange(pokemonsCategory);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}