using api.Dto;
using api.Filter;
using api.Helper;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Interfaces;
using server.Models;

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
    }
}