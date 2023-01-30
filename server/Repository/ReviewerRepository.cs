using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Interfaces;
using server.Models;

namespace server.Repository
{
  public class ReviewerRepository : IReviewerRepository
  {
    private readonly DataContext _dataContext;

    public ReviewerRepository(DataContext dataContext)
    {
      _dataContext = dataContext;
    }

    public async Task<Reviewer> GetReviewerAsync(int reviewerId)
    {
      return await _dataContext.Reviewers.Where(r => r.Id == reviewerId && r.Hidden == false).Include(e => e.Reviews).FirstOrDefaultAsync();
    }

    public async Task<List<Reviewer>> GetReviewersAsync()
    {
      return await _dataContext.Reviewers.Where(r => r.Hidden == false).ToListAsync();
    }

    public async Task<List<Review>> GetReviewsByReviewerAsync(int reviewerId)
    {
      return await _dataContext.Reviews.Where(r => r.Reviewer.Id == reviewerId && r.Hidden == false).ToListAsync();
    }

    public bool ReviewerExists(int reviewerId)
    {
      return _dataContext.Reviewers.Any(r => r.Id == reviewerId && r.Hidden == false);
    }
  }
}