using server.Data;
using server.Interfaces;
using server.Models;
using Microsoft.EntityFrameworkCore;
using server.Dto;
using AutoMapper;

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
      var pokemon = _mapper.Map<Pokemon>(pokemonDto);
      var pokemonOwnerEntity = await _context.Owners.Where(o => o.Id == ownerId).FirstOrDefaultAsync();
      var pokemonCategoryEntity = await _context.Categories.Where(c => c.Id == categoryId).FirstOrDefaultAsync();

      var pokemonOwner = new PokemonOwner()
      {
        Owner = pokemonOwnerEntity,
        Pokemon = pokemon,
      };
      await _context.AddAsync(pokemonOwner);

      var pokemonCategory = new PokemonCategory()
      {
        Category = pokemonCategoryEntity,
        Pokemon = pokemon,
      };
      await _context.AddAsync(pokemonCategory);

      await _context.SaveChangesAsync();
      return pokemonDto;
    }
  }
}