using api.Filter;
using api.Helper;
using api.Services;
using api.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dto;
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
        private readonly IUriService _uriService;
        private readonly DataContext _context;
        public PokemonController(IPokemonRepository pokemonRepository, DataContext context, IUriService uriService)
        {
            _pokemonRepository = pokemonRepository;
            _uriService = uriService;
            _context = context;
        }

        // [HttpGet]
        // [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        // [ProducesResponseType(404)]
        // public async Task<IActionResult> GetPokemonAsync()
        // {
        //     var pokemon = await _pokemonRepository.GetPokemonsAsync();
        //     if (!ModelState.IsValid)
        //         return BadRequest(ModelState);
        //     return Ok(pokemon);
        // }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPokemonAsync([FromQuery] PaginationFilter filter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var route = Request.Path.Value;

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var pagedData = await _pokemonRepository.GetPokemonsAsync(filter);

            var totalRecords = await _context.Pokemons.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<PokemonDto>(pagedData, validFilter, totalRecords, _uriService, route);

            return Ok(pagedReponse);
        }

        [HttpGet("{pokemonId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPokemonAsync(int pokemonId)
        {
            if (!_pokemonRepository.PokemonExists(pokemonId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pokemon = await _pokemonRepository.GetPokemonAsync(pokemonId);

            return Ok(new Response<PokemonDto>(pokemon));
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
        ///api/pokemons/

        //    {
        //        ownerId.
        //        cateId
        //    }
        [HttpPost("{ownerIds}&{categoryIds}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PokemonDto>> CreateAsync([FromRoute] string ownerIds, [FromRoute] string categoryIds, [FromBody] PokemonDto pokemonDto)
        {
            if (pokemonDto == null)
                return BadRequest("Null");

            if (_pokemonRepository.PokemonExists(pokemonDto.Name, pokemonDto.Id))
            {
                ModelState.AddModelError("", "Pokemon is already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var categoryIdArray = categoryIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
                var ownerIdArray = ownerIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
                await _pokemonRepository.CreateAsync(ownerIdArray, categoryIdArray, pokemonDto);
                return Ok(pokemonDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{pokemonId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PokemonDto>> UpdatePokemonAsync([FromRoute] int pokemonId, [FromBody] PokemonDto pokemonDto)
        {
            if (pokemonDto == null)
                return BadRequest("Null");

            if (pokemonId != pokemonDto.Id)
                return BadRequest("Pokemon is not exists");

            if (_pokemonRepository.PokemonExists(pokemonDto.Name, pokemonDto.Id))
            {
                ModelState.AddModelError("", "Pokemon is already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _pokemonRepository.UpdatePokemonAsync(pokemonId, pokemonDto);
                return Ok(pokemonDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{pokemonId}&{categoryIds}/category")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PokemonDto>> UpdatePokemonCategoryAsync([FromRoute] int pokemonId, [FromRoute] string categoryIds)
        {
            if (!_pokemonRepository.PokemonExists(pokemonId))
                return NotFound("Pokemon is not exists");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var categoryIdArray = categoryIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
                await _pokemonRepository.UpdatePokemonCategoryAsync(pokemonId, categoryIdArray);
                return Ok("Update pokemon category successfully");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{pokemonId}&{ownerIds}/owner")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PokemonDto>> UpdatePokemonOwnerAsync([FromRoute] int pokemonId, [FromRoute] string ownerIds)
        {
            if (!_pokemonRepository.PokemonExists(pokemonId))
                return NotFound("Pokemon is not exists");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var ownerIdArray = ownerIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
                await _pokemonRepository.UpdatePokemonOwnerAsync(pokemonId, ownerIdArray);
                return Ok("Update pokemon owner successfully");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{pokemonId}/archive")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PokemonDto>> ArchiveAsync([FromRoute] int pokemonId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _pokemonRepository.ArchiveAsync(pokemonId);
                return Ok("Archive successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{pokemonIds}/multiple-archive")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PokemonDto>> MultiArchiveAsync([FromRoute] string pokemonIds)
        {
            var pokemonIdArray = pokemonIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _pokemonRepository.MultiArchiveAsync(pokemonIdArray);
                return Ok("Multi archive successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{pokemonId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PokemonDto>> DeleteAsync([FromRoute] int pokemonId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _pokemonRepository.DeleteAsync(pokemonId);
                return Ok("Delete successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{pokemonIds}/multiple-delete")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PokemonDto>> MultiDeleteAsync([FromRoute] string pokemonIds)
        {
            var pokemonIdArray = pokemonIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _pokemonRepository.MultiDeleteAsync(pokemonIdArray);
                return Ok("Multi delete successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}