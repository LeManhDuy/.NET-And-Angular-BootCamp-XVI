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
    private readonly DataContext _dataContext;
    public CategoryController(ICategoryRepository categoryRepository, DataContext dataContext)
    {
      _categoryRepository = categoryRepository;
      _dataContext = dataContext;
    }


    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
    public IActionResult GetCategory()
    {
      var categories = _categoryRepository.GetCategories();
      if (!ModelState.IsValid)
        return BadRequest(ModelState);
      return Ok(categories);
    }

    [HttpGet("{categoryId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
    public IActionResult GetCategory(int categoryId)
    {
      if (!_categoryRepository.CategoryExists(categoryId))
        return NotFound();

      var pokemon = _categoryRepository.GetCategory(categoryId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      return Ok(pokemon);
    }

    [HttpGet("{categoryId}/pokemon")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
    public IActionResult GetPokemonByCategories(int categoryId)
    {
      if (!_categoryRepository.CategoryExists(categoryId))
        return NotFound();

      var pokemon = _categoryRepository.GetPokemonByCategories(categoryId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      return Ok(pokemon);
    }
  }
}