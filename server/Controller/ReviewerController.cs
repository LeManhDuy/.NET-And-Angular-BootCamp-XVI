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
    [Route("v1/api/reviewer")]
    [ApiController]
    public class ReviewerController : ControllerBase
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IUriService _uriService;
        private readonly DataContext _context;

        public ReviewerController(IReviewerRepository reviewerRepository, DataContext context, IUriService uriService)
        {
            _reviewerRepository = reviewerRepository;
            _uriService = uriService;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetReviewersAsync([FromQuery] PaginationFilter filter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var route = Request.Path.Value;

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var pagedData = await _reviewerRepository.GetReviewersAsync(filter);

            var totalRecords = await _context.Reviews.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ReviewerDto>(pagedData, validFilter, totalRecords, _uriService, route);

            return Ok(pagedReponse);
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

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ReviewerDto>> CreateAsync([FromBody] ReviewerDto reviewerDto)
        {
            if (reviewerDto == null)
                return BadRequest();

            if (_reviewerRepository.ReviewerExists(reviewerDto.Id))
            {
                ModelState.AddModelError("", "Reviewer is already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _reviewerRepository.CreateAsync(reviewerDto);
                return Ok(reviewerDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{reviwerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ReviewerDto>> UpdateAsync([FromRoute] int reviwerId, [FromBody] ReviewerDto reviewerDto)
        {
            if (reviewerDto == null)
                return BadRequest("Null");

            if (reviwerId != reviewerDto.Id)
                return BadRequest("Reviewer is not exists");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _reviewerRepository.UpdateAsync(reviwerId, reviewerDto);
                return Ok(reviewerDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{reviwerId}/archive")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ReviewerDto>> ArchiveAsync([FromRoute] int reviwerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _reviewerRepository.ArchiveAsync(reviwerId);
                return Ok("Archive successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{reviwerIds}/multiple-archive")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ReviewerDto>> MultiArchiveAsync([FromRoute] string reviwerIds)
        {
            var reviwerIdArray = reviwerIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _reviewerRepository.MultiArchiveAsync(reviwerIdArray);
                return Ok("Multi archive successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{reviwerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ReviewerDto>> DeleteAsync([FromRoute] int reviwerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _reviewerRepository.DeleteAsync(reviwerId);
                return Ok("Delete successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{reviwerIds}/multiple-delete")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ReviewerDto>> MultiDeleteAsync([FromRoute] string reviwerIds)
        {
            var reviwerIdArray = reviwerIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _reviewerRepository.MultiDeleteAsync(reviwerIdArray);
                return Ok("Multi delete successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}