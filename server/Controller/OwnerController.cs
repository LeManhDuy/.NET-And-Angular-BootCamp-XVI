using System;
using System.Collections.Generic;
using api.Dto;
using api.Filter;
using api.Helper;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dto;
using server.Interfaces;
using server.Models;
using server.Repository;

namespace server.Controller
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly DataContext _context;
        private readonly IUriService _uriService;
        public OwnerController(IOwnerRepository ownerRepository, IPokemonRepository pokemonRepository, DataContext context, IUriService uriService)
        {
            _ownerRepository = ownerRepository;
            _pokemonRepository = pokemonRepository;
            _context = context;
            _uriService = uriService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<OwnerDto>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOwnersAsync([FromQuery] PaginationFilter filter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var route = Request.Path.Value;

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var pagedData = await _ownerRepository.GetOwnersAsync(filter);

            var totalRecords = await _context.Pokemons.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OwnerDto>(pagedData, validFilter, totalRecords, _uriService, route);

            return Ok(pagedReponse);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOwnerAsync(int ownerId)
        {
            if (!_ownerRepository.OwnerExist(ownerId))
                return NotFound();

            var owner = await _ownerRepository.GetOwnerAsync(ownerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owner);
        }

        [HttpGet("{pokemonId}/owner")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOwnerOfPokemon(int pokemonId)
        {
            if (!_pokemonRepository.PokemonExists(pokemonId))
                return NotFound();

            var owner = await _ownerRepository.GetOwnerOfPokemonAsync(pokemonId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owner);
        }

        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPokemonByOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExist(ownerId))
                return NotFound();
            var pokemon = await _ownerRepository.GetPokemonByOwnerAsync(ownerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemon);
        }

        [HttpPost("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<OwnerDto>> CreateAsync([FromBody] OwnerDto ownerDto, [FromRoute] int countryId)
        {
            if (ownerDto == null)
                return BadRequest();

            if (_ownerRepository.OwnerExist(ownerDto.Id, ownerDto.Name))
            {
                ModelState.AddModelError("", "Owner is already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _ownerRepository.CreateAsync(countryId, ownerDto);
                return Ok(ownerDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{countryId}&{ownerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<OwnerDto>> UpdateAsync([FromRoute] int countryId, [FromRoute] int ownerId, [FromBody] OwnerDto ownerDto)
        {
            if (ownerDto == null)
                return BadRequest();

            if (_ownerRepository.OwnerExist(ownerDto.Id, ownerDto.Name))
            {
                ModelState.AddModelError("", "Owner is already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _ownerRepository.UpdateAsync(countryId, ownerId, ownerDto);
                return Ok(ownerDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{countryId}&{ownerIds}/multiple-update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<OwnerDto>> MultiUpdateAsync([FromRoute] int countryId, [FromRoute] string ownerIds)
        {
            var ownerIdArray = ownerIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var data = await _ownerRepository.MultiUpdateAsync(countryId, ownerIdArray);
                return Ok(data);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{ownerId}/archive")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PokemonDto>> ArchiveAsync([FromRoute] int ownerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _ownerRepository.ArchiveAsync(ownerId);
                return Ok("Archive successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{ownerIds}/multiple-archive")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PokemonDto>> MultiArchiveAsync([FromRoute] string ownerIds)
        {
            var ownerIdArray = ownerIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _ownerRepository.MultiArchiveAsync(ownerIdArray);
                return Ok("Multi archive successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{ownerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PokemonDto>> DeleteAsync([FromRoute] int ownerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _ownerRepository.DeleteAsync(ownerId);
                return Ok("Delete successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{ownerIds}/multiple-delete")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CategoryDto>> MultiDeleteAsync([FromRoute] string ownerIds)
        {
            var ownerIdArray = ownerIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _ownerRepository.MultiDeleteAsync(ownerIdArray);
                return Ok("Multi delete successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}