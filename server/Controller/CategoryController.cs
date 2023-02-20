using api.Filter;
using api.Helper;
using api.Interfaces;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dto;
using server.Interfaces;
using server.Models;
using server.Repository;

namespace server.Controller
{
    [Route("v1/api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly DataContext _context;
        private readonly IUriService _uriService;
        public CategoryController(ICategoryRepository categoryRepository, DataContext context, IUriService uriService, ITokenRepository tokenRepository)
        {
            _categoryRepository = categoryRepository;
            _context = context;
            _uriService = uriService;
            _tokenRepository = tokenRepository;
        }

        /// <summary>
        /// Creates an Employee.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Employee
        ///     {        
        ///       "firstName": "Mike",
        ///       "lastName": "Andrew",
        ///       "emailId": "Mike.Andrew@gmail.com"        
        ///     }
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCategoriesAsync([FromQuery] PaginationFilter filter)
        {
            if (!_tokenRepository.IsTokenValid())
                return NotFound();

            _tokenRepository.GetCurrentUser();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var route = Request.Path.Value;

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var pagedData = await _categoryRepository.GetCategoriesAsync(filter);

            var totalRecords = await _context.Pokemons.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<Category>(pagedData, validFilter, totalRecords, _uriService, route);

            return Ok(pagedReponse);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCategoryAsync(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var pokemon = await _categoryRepository.GetCategoryAsync(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemon);
        }

        [HttpGet("{categoryId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPokemonByCategoryIdAsync([FromRoute] int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var pokemon = await _categoryRepository.GetPokemonByCategoriesAsync(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemon);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CategoryDto>> CreateAsync([FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null)
                return BadRequest();

            if (_categoryRepository.CategoryExists(categoryDto.Name, categoryDto.Id))
            {
                ModelState.AddModelError("", "Category is already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _categoryRepository.CreateAsync(categoryDto);
                return Ok(categoryDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CategoryDto>> UpdateAsync([FromRoute] int categoryId, [FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null)
                return BadRequest("Null");

            if (categoryId != categoryDto.Id)
                return BadRequest("Category is not exists");

            if (_categoryRepository.CategoryExists(categoryDto.Name, categoryDto.Id))
            {
                ModelState.AddModelError("", "Category is already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _categoryRepository.UpdateAsync(categoryId, categoryDto);
                return Ok(categoryDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{categoryId}/archive")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CategoryDto>> ArchiveAsync([FromRoute] int categoryId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _categoryRepository.ArchiveAsync(categoryId);
                return Ok("Archive successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{categoryIds}/multiple-archive")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CategoryDto>> MultiArchiveAsync([FromRoute] string categoryIds)
        {
            var categoryIdArray = categoryIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _categoryRepository.MultiArchiveAsync(categoryIdArray);
                return Ok("Multi archive successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CategoryDto>> DeleteAsync([FromRoute] int categoryId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _categoryRepository.DeleteAsync(categoryId);
                return Ok("Delete successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{categoryIds}/multiple-delete")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CategoryDto>> MultiDeleteAsync([FromRoute] string categoryIds)
        {
            var categoryIdArray = categoryIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _categoryRepository.MultiDeleteAsync(categoryIdArray);
                return Ok("Multi delete successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}