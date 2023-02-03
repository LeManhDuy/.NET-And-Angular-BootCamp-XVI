
using api.Dto;
using api.Filter;
using server.Models;

namespace server.Interfaces
{
    public interface IReviewerRepository
    {
        Task<List<ReviewerDto>> GetReviewersAsync(PaginationFilter filter);
        Task<ReviewerDto> GetReviewerAsync(int reviewerId);
        Task<List<ReviewerDto>> GetReviewsByReviewerAsync(int reviewerId);
        bool ReviewerExists(int reviewerId);
    }
}