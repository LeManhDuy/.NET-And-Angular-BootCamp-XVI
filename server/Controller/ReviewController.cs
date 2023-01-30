using Microsoft.AspNetCore.Mvc;
using server.Interfaces;
using server.Models;

namespace server.Controller
{
  [Route("v1/api/review")]
  [ApiController]
  public class ReviewController : ControllerBase
  {
    private readonly IReviewRepository _reviewRepository;
    private readonly IPokemonRepository _pokemonRepository;

    public ReviewController(IReviewRepository reviewRepository, IPokemonRepository pokemonRepository)
    {
      _reviewRepository = reviewRepository;
      _pokemonRepository = pokemonRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
    public async Task<IActionResult> GetReviewsAsync()
    {
      var reviews = await _reviewRepository.GetReviewsAsync();

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      return Ok(reviews);
    }

    [HttpGet("{reviewId}")]
    [ProducesResponseType(200, Type = typeof(Review))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetPokemonAsync(int reviewId)
    {
      if (!_reviewRepository.ReviewExists(reviewId))
        return NotFound();

      var review = await _reviewRepository.GetReviewAsync(reviewId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      return Ok(review);
    }

    [HttpGet("{pokemonId}/pokemon")]
    [ProducesResponseType(200, Type = typeof(Review))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetReviewsForAPokemonAsync(int pokemonId)
    {
      if (!_pokemonRepository.PokemonExists(pokemonId))
        return NotFound();

      var reviews = await _reviewRepository.GetReviewsOfAPokemonAsync(pokemonId);

      if (!ModelState.IsValid)
        return BadRequest();

      return Ok(reviews);
    }
  }
}