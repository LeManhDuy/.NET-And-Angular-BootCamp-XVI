using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Route("v1/api/country")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IUriService _uriService;
        private readonly DataContext _context;
        public CountryController(ICountryRepository countryRepository, DataContext context, IUriService uriService)
        {
            _countryRepository = countryRepository;
            _uriService = uriService;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCountriesAsync([FromQuery] PaginationFilter filter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var route = Request.Path.Value;

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var pagedData = await _countryRepository.GetCountriesAsync(filter);

            var totalRecords = await _context.Countries.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<CountryDto>(pagedData, validFilter, totalRecords, _uriService, route);

            return Ok(pagedReponse);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCountryAsync(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var country = await _countryRepository.GetCountryAsync(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(country);
        }

        [HttpGet("{ownerId}/owner")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCountryOfOwnerAsync(int ownerId)
        {
            var country = await _countryRepository.GetCountryOfOwnerAsync(ownerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(country);
        }

        [HttpGet("{countryId}/owners-from-country")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<OwnerDto>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOwnersFromACountryAsync(int countryId)
        {
            var owners = await _countryRepository.GetOwnersFromACountryAsync(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owners);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CountryDto>> CreateAsync([FromBody] CountryDto countryDto)
        {
            if (countryDto == null)
                return BadRequest();

            if (_countryRepository.CountryExists(countryDto.Name, countryDto.Id))
            {
                ModelState.AddModelError("", "Country is already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _countryRepository.CreateAsync(countryDto);
                return Ok(countryDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CountryDto>> UpdateAsync([FromRoute] int countryId, [FromBody] CountryDto countryDto)
        {
            if (countryDto == null)
                return BadRequest("Null");

            if (countryId != countryDto.Id)
                return BadRequest("Country is not exists");

            if (_countryRepository.CountryExists(countryDto.Name, countryDto.Id))
            {
                ModelState.AddModelError("", "Country is already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _countryRepository.UpdateAsync(countryId, countryDto);
                return Ok(countryDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{countryId}/archive")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CountryDto>> ArchiveAsync([FromRoute] int countryId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _countryRepository.ArchiveAsync(countryId);
                return Ok("Archive successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{countryIds}/multiple-archive")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CountryDto>> MultiArchiveAsync([FromRoute] string countryIds)
        {
            var countryIdArray = countryIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _countryRepository.MultiArchiveAsync(countryIdArray);
                return Ok("Multi archive successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CountryDto>> DeleteAsync([FromRoute] int countryId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _countryRepository.DeleteAsync(countryId);
                return Ok("Delete successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{countryIds}/multiple-delete")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CountryDto>> MultiDeleteAsync([FromRoute] string countryIds)
        {
            var countryIdArray = countryIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _countryRepository.MultiDeleteAsync(countryIdArray);
                return Ok("Multi delete successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}