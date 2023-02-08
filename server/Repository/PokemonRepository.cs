using server.Data;
using server.Interfaces;
using server.Models;
using Microsoft.EntityFrameworkCore;
using server.Dto;
using AutoMapper;
using System.Linq;
using api.Filter;
using api.Wrappers;
using api.Helper;
using Azure.Core;
using api.Services;

namespace server.Repository
{
  public class PokemonRepository : IPokemonRepository
  {
    // dp-repo
    // ctor
    private readonly IMapper _mapper;
    private readonly DataContext _context;
    private readonly IUriService _uriService;
    public PokemonRepository(IMapper mapper, DataContext context, IUriService uriService)
    {
      _mapper = mapper;
      _context = context;
      _uriService = uriService;
    }

    public async Task<List<PokemonDto>> GetPokemonsAsync(PaginationFilter filter)
    {
        var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

        var pagedData = await _context.Pokemons
                          .Where(p => p.Hidden == false)
                          .Include(p => p.PokemonCategories)
                          .ThenInclude(pc => pc.Category)
                          .Select(p => new PokemonDto
                          {
                              Id = p.Id,
                              Name = p.Name,
                              BirthDate = p.BirthDate,
                              CategoryName = p.PokemonCategories
                                              .Where(pc => pc.Category.Hidden == false)
                                              .Select(pc => pc.Category.Name).ToList()
                          })
                          .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                          .Take(validFilter.PageSize)
                          .ToListAsync();

        return pagedData;
    }

    // public async Task<List<PokemonDto>> GetPokemonsAsync()
    // {
    //   return await _context.Pokemons.Where(p => p.Hidden == false)
    //                                 .Include(p => p.PokemonCategories)
    //                                 .ThenInclude(pc => pc.Category)
    //                                 .Select(p => new PokemonDto
    //                                 {
    //                                   Id = p.Id,
    //                                   Name = p.Name,
    //                                   BirthDate = p.BirthDate,
    //                                   CategoryName = p.PokemonCategories
    //                                           .Where(pc => pc.Category.Hidden == false)
    //                                           .Select(pc => pc.Category.Name).ToList()
    //                                 })
    //                                 .Take(20)
    //                                 .ToListAsync();
    // }

    public async Task<PokemonDto> GetPokemonAsync(int pokemonId)
    {
      return await _context.Pokemons.Where(p => p.Hidden == false && p.Id == pokemonId)
                                    .Include(p => p.PokemonCategories)
                                    .ThenInclude(pc => pc.Category)
                                    .Select(p => new PokemonDto
                                    {
                                      Id = p.Id,
                                      Name = p.Name,
                                      BirthDate = p.BirthDate,
                                      CategoryName = p.PokemonCategories
                                                        .Where(pc => pc.Category.Hidden == false)
                                                        .Select(pc => pc.Category.Name).ToList()
                                    })
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

    public bool PokemonExists(string pokemonName, int pokemonId)
    {
      return _context.Pokemons.Any(p => p.Id != pokemonId && p.Name.ToLower() == pokemonName.ToLower());
    }

    public async Task<PokemonDto> CreateAsync(int[] ownerIdArray, int[] categoryIdArray, PokemonDto pokemonDto)
    {
      try
      {
        var categories = _context.Categories.Count(c => categoryIdArray.Contains(c.Id) && c.Hidden == false);
        if (categories != categoryIdArray.Length)
          throw new Exception("One or more categories not found!");

        var owners = _context.Owners.Count(c => ownerIdArray.Contains(c.Id) && c.Hidden == false);
        if (owners != ownerIdArray.Length)
          throw new Exception("One or more owners not found!");

        var pokemon = _mapper.Map<Pokemon>(pokemonDto);
        await _context.Pokemons.AddAsync(pokemon);
        await _context.SaveChangesAsync();

        var pokemonCategories = new List<PokemonCategory>();
        foreach (var newCategoryId in categoryIdArray)
        {
          var item = new PokemonCategory()
          {
            PokemonId = pokemon.Id,
            CategoryId = newCategoryId
          };
          pokemonCategories.Add(item);
        }
        _context.PokemonCategories.AddRange(pokemonCategories);

        var pokemonOwners = new List<PokemonOwner>();
        foreach (var newOwnerId in ownerIdArray)
        {
          var item = new PokemonOwner()
          {
            PokemonId = pokemon.Id,
            OwnerId = newOwnerId
          };
          pokemonOwners.Add(item);
        }
        _context.PokemonOwners.AddRange(pokemonOwners);

        await _context.SaveChangesAsync();
        return pokemonDto;
      }
      catch (Exception ex)
      {
        throw new Exception("Message: " + ex.Message);
      }
    }

    public async Task<bool> UpdatePokemonCategoryAsync(int pokemonId, int[] categoryIdArray)
    {
      try
      {
        var pokemon = await _context.Pokemons.Where(p => p.Id == pokemonId && p.Hidden == false).FirstOrDefaultAsync();
        if (pokemon == null)
          throw new Exception("Pokemon not found!");

        var categories = _context.Categories.Count(c => categoryIdArray.Contains(c.Id) && c.Hidden == false);
        if (categories != categoryIdArray.Length)
          throw new Exception("One or more categories not found!");

        var idPokemonCategoryList = await _context.PokemonCategories.Where(pc => pc.PokemonId == pokemonId).ToListAsync();
        _context.PokemonCategories.RemoveRange(idPokemonCategoryList);

        var pokemonCategories = new List<PokemonCategory>();
        foreach (var newId in categoryIdArray)
        {
          var item = new PokemonCategory()
          {
            PokemonId = pokemonId,
            CategoryId = newId
          };
          pokemonCategories.Add(item);
        }

        _context.PokemonCategories.AddRange(pokemonCategories);
        await _context.SaveChangesAsync();

        return true;
      }
      catch (Exception ex)
      {
        throw new Exception("Message: " + ex.Message);
      }
    }

    public async Task<bool> UpdatePokemonOwnerAsync(int pokemonId, int[] ownerIdArray)
    {
      try
      {
        var pokemon = await _context.Pokemons.Where(p => p.Id == pokemonId && p.Hidden == false).FirstOrDefaultAsync();
        if (pokemon == null)
          throw new Exception("Pokemon not found!");

        var owners = _context.Owners.Count(c => ownerIdArray.Contains(c.Id) && c.Hidden == false);
        if (owners != ownerIdArray.Length)
          throw new Exception("One or more owners not found!");

        var idPokemonOwnerList = await _context.PokemonOwners.Where(pc => pc.PokemonId == pokemonId).ToListAsync();
        _context.PokemonOwners.RemoveRange(idPokemonOwnerList);

        var pokemonOwners = new List<PokemonOwner>();
        foreach (var newId in ownerIdArray)
        {
          var item = new PokemonOwner()
          {
            PokemonId = pokemonId,
            OwnerId = newId
          };
          pokemonOwners.Add(item);
        }
        _context.PokemonOwners.AddRange(pokemonOwners);
        await _context.SaveChangesAsync();

        return true;
      }
      catch (Exception ex)
      {
        throw new Exception("Message: " + ex.Message);
      }
    }

    public async Task<PokemonDto> UpdatePokemonAsync(int pokemonId, PokemonDto pokemonDto)
    {
      try
      {
        var pokemon = await _context.Pokemons.Where(p => p.Id == pokemonId && p.Hidden == false).FirstOrDefaultAsync();
        if (pokemon == null)
          throw new Exception("Pokemon not found!");

        _mapper.Map(pokemonDto, pokemon);
        _context.Pokemons.Update(pokemon);
        await _context.SaveChangesAsync();

        return pokemonDto;
      }
      catch (Exception ex)
      {
        throw new Exception("Message: " + ex.Message);
      }
    }

    public async Task ArchiveAsync(int pokemonId)
    {
      try
      {
        var pokemon = await _context.Pokemons.Where(p => p.Id == pokemonId).FirstOrDefaultAsync();
        if (pokemon == null)
          throw new Exception("Pokemon not found!");

        pokemon.Hidden = !pokemon.Hidden;
        _context.Pokemons.Update(pokemon);
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        throw new Exception("Message: " + ex.Message);
      }
    }

    public async Task MultiArchiveAsync(int[] pokemonId)
    {
      try
      {
        var checkValid = _context.Pokemons.Count(c => pokemonId.Contains(c.Id));
        if (checkValid != pokemonId.Length)
          throw new Exception("One or more pokemon not found!");

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
        throw new Exception("Message: " + ex.Message);
      }
    }

    public async Task DeleteAsync(int pokemonId)
    {
      try
      {
        var pokemon = await _context.Pokemons.Where(p => p.Id == pokemonId && p.Hidden == true).FirstOrDefaultAsync();
        if (pokemon == null)
          throw new Exception("Pokemon not found!");
        _context.Pokemons.Remove(pokemon);

        var pokemonsOwner = await _context.PokemonOwners.Where(c => c.PokemonId == pokemonId).ToListAsync();
        if (pokemonsOwner == null)
          throw new Exception("Pokemon owner not found!");
        _context.PokemonOwners.RemoveRange(pokemonsOwner);

        var pokemonsCategory = await _context.PokemonCategories.Where(c => c.PokemonId == pokemonId).ToListAsync();
        if (pokemonsCategory == null)
          throw new Exception("Pokemon category not found!");
        _context.PokemonCategories.RemoveRange(pokemonsCategory);

        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        throw new Exception("Message: " + ex.Message);
      }
    }

    public async Task MultiDeleteAsync(int[] pokemonId)
    {
      try
      {
        var checkValid = _context.Pokemons.Count(c => pokemonId.Contains(c.Id));
        if (checkValid != pokemonId.Length)
          throw new Exception("One or more pokemon not found!");

        var pokemons = await _context.Pokemons.Where(c => pokemonId.Contains(c.Id) && c.Hidden == true).ToListAsync();
        _context.Pokemons.RemoveRange(pokemons);

        var pokemonsOwner = await _context.PokemonOwners.Where(c => pokemonId.Contains(c.PokemonId)).ToListAsync();
        _context.PokemonOwners.RemoveRange(pokemonsOwner);

        var pokemonsCategory = await _context.PokemonCategories.Where(c => pokemonId.Contains(c.PokemonId)).ToListAsync();
        _context.PokemonCategories.RemoveRange(pokemonsCategory);

        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        throw new Exception("Message: " + ex.Message);
      }
    }
  }
}