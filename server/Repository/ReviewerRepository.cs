using api.Dto;
using api.Filter;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Interfaces;
using server.Models;

namespace server.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext _context;

        public ReviewerRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task<ReviewerDto> GetReviewerAsync(int reviewerId)
        {
            var data = await _context.Reviewers.Where(c => c.Hidden == false && c.Id == reviewerId)
                                   .Include(c => c.Reviews)
                                   .Select(p => new ReviewerDto
                                   {
                                       Id = p.Id,
                                       FirstName = p.FirstName,
                                       LastName = p.LastName,
                                       Hidden = p.Hidden,
                                       ReviewInformation = p.Reviews
                                                            .Where(pc => pc.Hidden == false)
                                                            .Select(pc => new Review
                                                            {
                                                                Title = pc.Title,
                                                                Text = pc.Text,
                                                                Rating = pc.Rating
                                                            }).ToList()
                                   })
                                   .FirstOrDefaultAsync();
            return data;
        }

        public async Task<List<ReviewerDto>> GetReviewersAsync(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var data = await _context.Reviewers.Where(c => c.Hidden == false)
                                               .Include(c => c.Reviews)
                                               .Select(p => new ReviewerDto
                                               {
                                                   Id = p.Id,
                                                   FirstName = p.FirstName,
                                                   LastName = p.LastName,
                                                   Hidden = p.Hidden,
                                                   ReviewInformation = p.Reviews
                                                                        .Where(pc => pc.Hidden == false)
                                                                        .Select(pc => new Review
                                                                        {
                                                                            Title = pc.Title,
                                                                            Text = pc.Text,
                                                                            Rating = pc.Rating
                                                                        }).ToList()
                                               })
                                               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                                               .Take(validFilter.PageSize)
                                               .ToListAsync();
            return data;
        }

        public async Task<List<Review>> GetReviewsByReviewerAsync(int reviewerId)
        {
            return await _context.Reviews.Where(r => r.Reviewer.Id == reviewerId && r.Hidden == false).ToListAsync();
        }

        public bool ReviewerExists(int reviewerId)
        {
            return _context.Reviewers.Any(r => r.Id == reviewerId && r.Hidden == false);
        }
    }
}