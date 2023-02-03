using server.Data;
using server.Interfaces;
using server.Models;
using Microsoft.EntityFrameworkCore;
using server.Dto;
using AutoMapper;
using api.Filter;

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

        public bool CategoryExists(string categoryName, int categoryId)
        {
            return _context.Categories.Any(c => c.Name.ToLower() == categoryName.ToLower() && c.Id != categoryId);
        }

        public async Task<CategoryDto> CreateAsync(CategoryDto categoryDto)
        {
            try
            {
                await _context.Categories.AddAsync(_mapper.Map<Category>(categoryDto));
                await _context.SaveChangesAsync();
                return categoryDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task<CategoryDto> UpdateAsync(int categoryId, CategoryDto categoryDto)
        {
            try
            {
                var category = await _context.Categories.Where(p => p.Id == categoryId && p.Hidden == false).FirstOrDefaultAsync();

                if (category == null)
                    throw new Exception("Category not found!");

                _mapper.Map(categoryDto, category);
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                return categoryDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task ArchiveAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);

                if (category == null)
                    throw new Exception("Category not found!");

                category.Hidden = !category.Hidden;
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task MultiArchiveAsync(int[] categoryIds)
        {
            try
            {
                var checkValid = _context.Categories.Where(c => categoryIds.Contains(c.Id)).Count();
                if (checkValid != categoryIds.Length)
                    throw new Exception("One or more categories not found!");

                var categories = await _context.Categories.Where(c => categoryIds.Contains(c.Id)).ToListAsync();

                foreach (var category in categories)
                {
                    category.Hidden = !category.Hidden;
                }
                _context.Categories.UpdateRange(categories);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task DeleteAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories.Where(p => p.Id == categoryId && p.Hidden == true).FirstOrDefaultAsync();
                if (category == null)
                    throw new Exception("Category not found!");
                _context.Categories.Remove(category);

                var pokemonsCategory = await _context.PokemonCategories.Where(c => c.CategoryId == categoryId).ToListAsync();
                if (pokemonsCategory == null)
                    throw new Exception("Pokemon category not found!");
                _context.PokemonCategories.RemoveRange(pokemonsCategory);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task MultiDeleteAsync(int[] categoryIds)
        {
            try
            {
                var checkValid = _context.Categories.Where(c => categoryIds.Contains(c.Id) && c.Hidden == true).Count();
                if (checkValid != categoryIds.Length)
                    throw new Exception("One or more categories not found!");

                var categories = await _context.Categories.Where(c => categoryIds.Contains(c.Id)).ToListAsync();
                _context.Categories.RemoveRange(categories);

                var pokemonsCategory = await _context.PokemonCategories.Where(c => categoryIds.Contains(c.CategoryId)).ToListAsync();
                _context.PokemonCategories.RemoveRange(pokemonsCategory);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task<List<Category>> GetCategoriesAsync(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var data = await _context.Categories
                                        .Where(c => c.Hidden == false)
                                        .OrderBy(c => c.Name)
                                        .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                                        .Take(validFilter.PageSize)
                                        .ToListAsync();
            return data;
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