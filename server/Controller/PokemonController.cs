using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Interfaces;
using server.Models;

namespace server.Controller
{
  //asp-api-controller
  [Route("v1/api/pokemon")]
  [ApiController]
  public class PokemonController : ControllerBase
  {
    private readonly IPokemonRepository _pokemonRepository;
    private readonly DataContext _dataContext;
    public PokemonController(IPokemonRepository pokemonRepository, DataContext dataContext)
    {
      _pokemonRepository = pokemonRepository;
      _dataContext = dataContext;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
    public IActionResult GetPokemon()
    {
      var pokemon = _pokemonRepository.GetPokemons();
      if (!ModelState.IsValid)
        return BadRequest(ModelState);
      return Ok(pokemon);
    }

    [HttpGet("{pokemonId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemon(int pokemonId)
    {
      if (!_pokemonRepository.PokemonExists(pokemonId))
        return NotFound();

      var pokemon = _pokemonRepository.GetPokemon(pokemonId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      return Ok(pokemon);
    }

    [HttpGet("{pokemonId}/rating")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<decimal>))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemonRating(int pokemonId)
    {
      if (!_pokemonRepository.PokemonExists(pokemonId))
        return NotFound();

      var pokemonRating = _pokemonRepository.GetPokemonRating(pokemonId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      return Ok(pokemonRating);
    }

    
  }
}