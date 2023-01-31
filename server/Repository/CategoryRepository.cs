using server.Data;
using server.Interfaces;
using server.Models;
using Microsoft.EntityFrameworkCore;
using server.Dto;
using AutoMapper;

namespace server.Repository
{
  public class CategoryRepository : ICategoryRepository
  {
    private readonly DataContext _context;

    private readonly IMapper _mapper;

    public CategoryRepository(DataContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public bool CategoryExists(int categoryId)
    {
      return _context.Categories.Any(c => c.Id == categoryId && c.Hidden == false);
    }
    public bool CategoryExists(string categoryName)
    {
      return _context.Categories.Any(c => c.Name.ToLower() == categoryName.ToLower() && c.Hidden == false);
    }

    public async Task<CategoryDto> CreateAsync(CategoryDto categoryDto)
    {
      await _context.Categories.AddAsync(_mapper.Map<Category>(categoryDto));
      await _context.SaveChangesAsync();
      return categoryDto;
    }

    public async Task<CategoryDto> UpdateAsync(int categoryId, CategoryDto categoryDto)
    {
      var category = _mapper.Map<Category>(categoryDto);

      _context.Categories.Update(category);
      await _context.SaveChangesAsync();

      return categoryDto;
    }

    public Task<CategoryDto> DeleteAsync(int categoryId)
    {
      throw new NotImplementedException();
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
      return await _context.Categories.Where(c => c.Hidden == false).OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Category> GetCategoryAsync(int categoryId)
    {
      return await _context.Categories.Where(c => c.Id == categoryId && c.Hidden == false).OrderBy(c => c.Name).FirstOrDefaultAsync();
    }

    public async Task<List<Pokemon>> GetPokemonByCategoriesAsync(int categoryId)
    {
      return await _context.PokemonCategories.Where(pc => pc.CategoryId == categoryId).Select(pc => pc.Pokemon).ToListAsync();
    }
  }
}