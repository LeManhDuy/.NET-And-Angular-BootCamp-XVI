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
    [Route("v1/api/review")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly DataContext _context;
        private readonly IUriService _uriService;
        public ReviewController(IReviewRepository reviewRepository, IPokemonRepository pokemonRepository, DataContext context, IUriService uriService)
        {
            _reviewRepository = reviewRepository;
            _pokemonRepository = pokemonRepository;
            _context = context;
            _uriService = uriService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public async Task<IActionResult> GetReviewsAsync([FromQuery] PaginationFilter filter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var route = Request.Path.Value;

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var pagedData = await _reviewRepository.GetReviewsAsync(filter);

            var totalRecords = await _context.Pokemons.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ReviewDto>(pagedData, validFilter, totalRecords, _uriService, route);

            return Ok(pagedReponse);
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

        [HttpPost("{reviewerId}&{pokemonId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Review>> CreateAsync([FromRoute] int reviewerId, [FromRoute] int pokemonId, [FromBody] Review review)
        {
            if (review == null)
                return BadRequest();

            if (_reviewRepository.ReviewExists(review.Id))
            {
                ModelState.AddModelError("", "Review is already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _reviewRepository.CreateAsync(reviewerId, pokemonId, review);
                return Ok(review);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{reviewerId}&{pokemonId}&{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ReviewDto>> UpdateAsync([FromRoute] int reviewerId, [FromRoute] int pokemonId, [FromRoute] int reviewId, [FromBody] Review review)
        {
            if (review == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _reviewRepository.UpdateAsync(reviewerId, pokemonId, reviewId, review);
                return Ok(review);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{reviewId}/archive")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ReviewDto>> ArchiveAsync([FromRoute] int reviewId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _reviewRepository.ArchiveAsync(reviewId);
                return Ok("Archive successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{reviewIds}/multiple-archive")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ReviewDto>> MultiArchiveAsync([FromRoute] string reviewIds)
        {
            var reviewIdArray = reviewIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _reviewRepository.MultiArchiveAsync(reviewIdArray);
                return Ok("Multi archive successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ReviewDto>> DeleteAsync([FromRoute] int reviewId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _reviewRepository.DeleteAsync(reviewId);
                return Ok("Delete successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{reviewIds}/multiple-delete")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ReviewDto>> MultiDeleteAsync([FromRoute] string reviewIds)
        {
            var reviewIdArray = reviewIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _reviewRepository.MultiDeleteAsync(reviewIdArray);
                return Ok("Multi delete successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}