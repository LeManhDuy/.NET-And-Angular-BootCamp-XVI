using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Interfaces;
using server.Models;

namespace server.Repository
{
  public class ReviewRepository : IReviewRepository
  {
    private readonly DataContext _dataContext;

    public ReviewRepository(DataContext dataContext)
    {
      _dataContext = dataContext;
    }

    public async Task<Review> GetReviewAsync(int reviewId)
    {
      return await _dataContext.Reviews.Where(r => r.Id == reviewId && r.Hidden == false).FirstOrDefaultAsync();
    }

    public async Task<List<Review>> GetReviewsAsync()
    {
      return await _dataContext.Reviews.Where(r => r.Hidden == false).ToListAsync();
    }

    public async Task<List<Review>> GetReviewsOfAPokemonAsync(int pokemonId)
    {
      return await _dataContext.Reviews.Where(r => r.Pokemon.Id == pokemonId && r.Hidden == false).ToListAsync();
    }

    public bool ReviewExists(int reviewId)
    {
      return _dataContext.Reviews.Any(r => r.Id == reviewId && r.Hidden == false );
    }
  }
}