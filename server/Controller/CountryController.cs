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
    [Route("v1/api/country")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        public CountryController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCountriesAsync()
        {
            var countries = await _countryRepository.GetCountriesAsync();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(countries);
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
    }
}