
using server.Models;

namespace server.Interfaces
{
    public interface IReviewerRepository
    {
        Task<List<Reviewer>> GetReviewersAsync();
        Task<Reviewer> GetReviewerAsync(int reviewerId);
        Task<List<Review>> GetReviewsByReviewerAsync(int reviewerId);
        bool ReviewerExists(int reviewerId);
    }
}