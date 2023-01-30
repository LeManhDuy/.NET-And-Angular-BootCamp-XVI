using Microsoft.AspNetCore.Mvc;
using server.Interfaces;
using server.Models;

namespace server.Controller
{
  [Route("v1/api/reviewer")]
  [ApiController]
  public class ReviewerController : ControllerBase
  {
    private readonly IReviewerRepository _reviewerRepository;

    public ReviewerController(IReviewerRepository reviewerRepository)
    {
      _reviewerRepository = reviewerRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
    public async Task<IActionResult> GetReviewersAsync()
    {
      var reviewers = await _reviewerRepository.GetReviewersAsync();

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      return Ok(reviewers);
    }

    [HttpGet("{reviewerId}")]
    [ProducesResponseType(200, Type = typeof(Reviewer))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetPokemonAsync(int reviewerId)
    {
      if (!_reviewerRepository.ReviewerExists(reviewerId))
        return NotFound();

      var reviewer = await _reviewerRepository.GetReviewerAsync(reviewerId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      return Ok(reviewer);
    }

    [HttpGet("{reviewerId}/reviews")]
    [ProducesResponseType(200, Type = typeof(Review))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetReviewsByAReviewer(int reviewerId)
    {
      if (!_reviewerRepository.ReviewerExists(reviewerId))
        return NotFound();

      var reviews = await _reviewerRepository.GetReviewsByReviewerAsync(reviewerId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      return Ok(reviews);
    }
  }
}