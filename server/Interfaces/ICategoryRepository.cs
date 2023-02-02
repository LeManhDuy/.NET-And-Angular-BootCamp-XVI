using api.Filter;
using server.Dto;
using server.Models;

namespace server.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategoriesAsync(PaginationFilter filter);
        Task<Category> GetCategoryAsync(int categoryId);
        Task<List<Pokemon>> GetPokemonByCategoriesAsync(int categoryId);
        bool CategoryExists(int categoryId);
        bool CategoryExists(string categoryName);
        Task<CategoryDto> CreateAsync(CategoryDto categoryDto);
        Task<CategoryDto> UpdateAsync(int categoryId, CategoryDto categoryDto);
        Task ArchiveAsync(int categoryId);
        Task MultiArchiveAsync(int[] categoryId);
        Task DeleteAsync(int categoryId);
        Task MultiDeleteAsync(int[] categoryId);
    }
}