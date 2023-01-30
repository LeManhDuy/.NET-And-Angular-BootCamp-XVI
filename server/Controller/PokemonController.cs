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
    public PokemonController(IPokemonRepository pokemonRepository)
    {
      _pokemonRepository = pokemonRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPokemonAsync()
    {
      var pokemon = await _pokemonRepository.GetPokemonsAsync();
      if (!ModelState.IsValid)
        return BadRequest(ModelState);
      return Ok(pokemon);
    }

    [HttpGet("{pokemonId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPokemonAsync(int pokemonId)
    {
      if (!_pokemonRepository.PokemonExists(pokemonId))
        return NotFound();

      var pokemon = await _pokemonRepository.GetPokemonAsync(pokemonId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      return Ok(pokemon);
    }

    [HttpGet("{pokemonId}/rating")]
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