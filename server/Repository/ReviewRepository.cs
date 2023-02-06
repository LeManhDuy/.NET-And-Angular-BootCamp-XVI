using api.Dto;
using api.Filter;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Interfaces;
using server.Models;
using static System.Net.Mime.MediaTypeNames;

namespace server.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context;

        public ReviewRepository(DataContext context)
        {
            _context = context;
        }

        public async Task ArchiveAsync(int reviewId)
        {
            try
            {
                var review = await _context.Reviews.Where(p => p.Id == reviewId).FirstOrDefaultAsync();
                if (review == null)
                    throw new Exception("Review not found!");

                review.Hidden = !review.Hidden;
                _context.Reviews.Update(review);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task<Review> CreateAsync(int reviewerId, int pokemonId, Review review)
        {
            try
            {
                var reviewer = await _context.Reviewers.Where(c => c.Id == reviewerId && c.Hidden == false).FirstOrDefaultAsync();
                if (reviewer == null)
                    throw new Exception("One or more reviewers not found!");

                var pokemon = await _context.Pokemons.Where(c => c.Id == pokemonId && c.Hidden == false).FirstOrDefaultAsync();
                if (pokemon == null)
                    throw new Exception("One or more pokemons not found!");

                var createReview = new Review()
                {
                    Title = review.Title,
                    Text = review.Text,
                    Rating = review.Rating,
                    Hidden = review.Hidden,
                    Reviewer = reviewer,
                    Pokemon = pokemon
                };

                await _context.Reviews.AddAsync(createReview);
                await _context.SaveChangesAsync();

                return review;
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task DeleteAsync(int reviewId)
        {
            try
            {
                var review = await _context.Reviews.Where(p => p.Id == reviewId && p.Hidden == true).FirstOrDefaultAsync();
                if (review == null)
                    throw new Exception("Review not found!");
                _context.Reviews.Remove(review);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task<Review> GetReviewAsync(int reviewId)
        {
            return await _context.Reviews.Where(r => r.Id == reviewId && r.Hidden == false).FirstOrDefaultAsync();
        }

        public async Task<List<ReviewDto>> GetReviewsAsync(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var data = await _context.Reviews.Where(c => c.Hidden == false)
                                               .Include(c => c.Reviewer)
                                               .Select(p => new ReviewDto
                                               {
                                                   Id = p.Id,
                                                   Title = p.Title,
                                                   Text = p.Title,
                                                   Rating = p.Rating,
                                                   ReviewerName = p.Reviewer.FirstName,
                                                   PokemonName = p.Pokemon.Name
                                               })
                                               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                                               .Take(validFilter.PageSize)
                                               .ToListAsync();
            return data;
        }

        public async Task<List<Review>> GetReviewsOfAPokemonAsync(int pokemonId)
        {
            return await _context.Reviews.Where(r => r.Pokemon.Id == pokemonId && r.Hidden == false).ToListAsync();
        }

        public async Task MultiArchiveAsync(int[] reviewIds)
        {
            try
            {
                var checkValid = _context.Reviews.Count(c => reviewIds.Contains(c.Id));
                if (checkValid != reviewIds.Length)
                    throw new Exception("One or more review not found!");

                var reviews = await _context.Reviews.Where(c => reviewIds.Contains(c.Id)).ToListAsync();
                foreach (var review in reviews)
                {
                    review.Hidden = !review.Hidden;
                }
                _context.Reviews.UpdateRange(reviews);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task MultiDeleteAsync(int[] reviewIds)
        {
            try
            {
                var checkValid = _context.Reviews.Count(c => reviewIds.Contains(c.Id));
                if (checkValid != reviewIds.Length)
                    throw new Exception("One or more reviews not found!");

                var reviews = await _context.Reviews.Where(c => reviewIds.Contains(c.Id) && c.Hidden == true).ToListAsync();
                _context.Reviews.RemoveRange(reviews);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public bool ReviewExists(int reviewId)
        {
            return _context.Reviews.Any(r => r.Id == reviewId && r.Hidden == false);
        }

        public async Task<Review> UpdateAsync(int reviewerId, int pokemonId, int reviewId, Review review)
        {
            try
            {
                var reviewData = await _context.Reviews.Where(p => p.Id == reviewId && p.Hidden == false).FirstOrDefaultAsync();
                if (reviewData == null)
                    throw new Exception("Review not found!");

                var reviewer = await _context.Reviewers.Where(c => c.Id == reviewerId && c.Hidden == false).FirstOrDefaultAsync();
                if (reviewer == null)
                    throw new Exception("One or more reviewers not found!");

                var pokemon = await _context.Pokemons.Where(c => c.Id == pokemonId && c.Hidden == false).FirstOrDefaultAsync();
                if (pokemon == null)
                    throw new Exception("One or more pokemons not found!");

                reviewData.Title = review.Title;
                reviewData.Text = review.Text;
                reviewData.Rating = review.Rating;
                reviewData.Hidden = review.Hidden;
                reviewData.Reviewer = reviewer;
                reviewData.Pokemon = pokemon;

                _context.Reviews.Update(reviewData);
                await _context.SaveChangesAsync();

                return review;
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

    }
}