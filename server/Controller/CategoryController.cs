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
    [Route("v1/api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly DataContext _context;
        private readonly IUriService _uriService;
        public CategoryController(ICategoryRepository categoryRepository, DataContext context, IUriService uriService)
        {
            _categoryRepository = categoryRepository;
            _context = context;
            _uriService = uriService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCategoriesAsync([FromQuery] PaginationFilter filter)
        {
            //var categories = await _categoryRepository.GetCategoriesAsync();
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);
            //return Ok(categories);

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

            if (_categoryRepository.CategoryExists(categoryDto.Name))
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
                return StatusCode(500, e);
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

            if (_categoryRepository.CategoryExists(categoryDto.Name))
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
                return StatusCode(500, e);
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
                return StatusCode(500, e);
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
                return StatusCode(500, e);
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
                return StatusCode(500, e);
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
                return StatusCode(500, e);
            }
        }
    }
}