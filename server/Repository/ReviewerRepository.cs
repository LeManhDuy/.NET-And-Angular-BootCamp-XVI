using api.Dto;
using api.Filter;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Interfaces;
using server.Models;

namespace server.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ReviewerRepository(DataContext dataContext, IMapper mapper)
        {
            _context = dataContext;
            _mapper = mapper;
        }

        public async Task ArchiveAsync(int reviewerId)
        {
            try
            {
                var reviewer = await _context.Reviewers.FindAsync(reviewerId);

                if (reviewer == null)
                    throw new Exception("Reviewer not found!");

                reviewer.Hidden = !reviewer.Hidden;
                _context.Reviewers.Update(reviewer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task<ReviewerDto> CreateAsync(ReviewerDto reviewerDto)
        {
            try
            {
                await _context.Reviewers.AddAsync(_mapper.Map<Reviewer>(reviewerDto));
                await _context.SaveChangesAsync();
                return reviewerDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task DeleteAsync(int reviewerId)
        {
            try
            {
                var reviewer = await _context.Reviewers.Where(p => p.Id == reviewerId && p.Hidden == true).FirstOrDefaultAsync();
                if (reviewer == null)
                    throw new Exception("Reviewer not found!");
                _context.Reviewers.Remove(reviewer);

                var reviews = await _context.Reviews.Where(c => c.Reviewer.Id == reviewerId).ToListAsync();
                if (reviews == null)
                    throw new Exception("Review not found!");
                foreach (var review in reviews)
                {
                    review.Reviewer = null;
                }
                _context.Reviews.UpdateRange(reviews);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
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

        public async Task MultiArchiveAsync(int[] reviewerIds)
        {
            try
            {
                var checkValid = _context.Reviewers.Where(c => reviewerIds.Contains(c.Id)).Count();
                if (checkValid != reviewerIds.Length)
                    throw new Exception("One or more reviewers not found!");

                var reviewers = await _context.Reviewers.Where(c => reviewerIds.Contains(c.Id)).ToListAsync();
                foreach (var reviewer in reviewers)
                {
                    reviewer.Hidden = !reviewer.Hidden;
                }
                _context.Reviewers.UpdateRange(reviewers);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task MultiDeleteAsync(int[] reviewerIds)
        {
            try
            {
                var checkValid = _context.Reviewers.Where(c => reviewerIds.Contains(c.Id) && c.Hidden == true).Count();
                if (checkValid != reviewerIds.Length)
                    throw new Exception("One or more reviewers not found!");

                var reviewers = await _context.Reviewers.Where(c => reviewerIds.Contains(c.Id)).ToListAsync();
                _context.Reviewers.RemoveRange(reviewers);

                var reviews = await _context.Reviews.Where(c => reviewerIds.Contains(c.Reviewer.Id)).ToListAsync();
                foreach (var review in reviews)
                {
                    review.Reviewer = null;
                }
                _context.Reviews.UpdateRange(reviews);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public bool ReviewerExists(int reviewerId)
        {
            return _context.Reviewers.Any(r => r.Id == reviewerId && r.Hidden == false);
        }

        public async Task<ReviewerDto> UpdateAsync(int reviewerId, ReviewerDto reviewerDto)
        {
            try
            {
                var reviewer = await _context.Reviewers.Where(p => p.Id == reviewerId && p.Hidden == false).FirstOrDefaultAsync();
                if (reviewer == null)
                    throw new Exception("Reviewer not found!");

                _mapper.Map(reviewerDto, reviewer);
                _context.Reviewers.Update(reviewer);
                await _context.SaveChangesAsync();

                return reviewerDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }
    }
}