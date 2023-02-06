using api.Dto;
using api.Filter;
using server.Models;

namespace server.Interfaces
{
    public interface IReviewRepository
    {
        Task<List<ReviewDto>> GetReviewsAsync(PaginationFilter filter);
        Task<Review> GetReviewAsync(int reviewId);
        Task<List<Review>> GetReviewsOfAPokemonAsync(int pokemonId);
        Task<Review> CreateAsync(int reviewerId, int pokemonId, Review review);
        Task<Review> UpdateAsync(int reviewerId, int pokemonId, int reviewId, Review review);
        Task ArchiveAsync(int reviewId);
        Task MultiArchiveAsync(int[] reviewIds);
        Task DeleteAsync(int reviewId);
        Task MultiDeleteAsync(int[] reviewIds);
        bool ReviewExists(int reviewId);
    }
}