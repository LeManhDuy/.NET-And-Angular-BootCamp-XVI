
using api.Dto;
using api.Filter;
using server.Models;

namespace server.Interfaces
{
    public interface IReviewerRepository
    {
        Task<List<ReviewerDto>> GetReviewersAsync(PaginationFilter filter);
        Task<ReviewerDto> GetReviewerAsync(int reviewerId);
        Task<List<Review>> GetReviewsByReviewerAsync(int reviewerId);
        Task<ReviewerDto> CreateAsync(ReviewerDto reviewerDto);
        Task<ReviewerDto> UpdateAsync(int reviewerId, ReviewerDto reviewerDto);
        Task ArchiveAsync(int reviewerId);
        Task MultiArchiveAsync(int[] reviewerIds);
        Task DeleteAsync(int reviewerId);
        Task MultiDeleteAsync(int[] reviewerIds);
        bool ReviewerExists(int reviewerId);
    }
}