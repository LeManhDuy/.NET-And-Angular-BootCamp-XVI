using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Interfaces;
using server.Models;

namespace server.Controller
{
  [Route("v1/api/category")]
  [ApiController]
  public class CategoryController : ControllerBase
  {
    private readonly ICategoryRepository _categoryRepository;
    public CategoryController(ICategoryRepository categoryRepository)
    {
      _categoryRepository = categoryRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCategoryAsync()
    {
      var categories = await _categoryRepository.GetCategoriesAsync();
      if (!ModelState.IsValid)
        return BadRequest(ModelState);
      return Ok(categories);
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
    public async Task<IActionResult> GetPokemonByCategoryIdAsync(int categoryId)
    {
      if (!_categoryRepository.CategoryExists(categoryId))
        return NotFound();

      var pokemon = await _categoryRepository.GetPokemonByCategoriesAsync(categoryId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      return Ok(pokemon);
    }
  }
}