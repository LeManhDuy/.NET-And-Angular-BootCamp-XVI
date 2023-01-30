using server.Models;

namespace server.Interfaces
{
  public interface IReviewRepository
  {
    Task<List<Review>> GetReviewsAsync();
    Task<Review> GetReviewAsync(int reviewId);
    Task<List<Review>> GetReviewsOfAPokemonAsync(int pokemonId);
    bool ReviewExists(int reviewId);
  }
}