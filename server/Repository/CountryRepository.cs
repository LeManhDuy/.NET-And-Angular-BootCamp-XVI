using api.Dto;
using api.Filter;
using Microsoft.EntityFrameworkCore;
using server.Data;
using AutoMapper;
using server.Interfaces;
using server.Models;
using server.Dto;

namespace server.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CountryRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public bool CountryExists(int countryId)
        {
            return _context.Countries.Any(c => c.Hidden == false && c.Id == countryId);
        }

        public bool CountryExists(string countryName, int countryId)
        {
            return _context.Countries.Any(c => c.Name.ToLower() == countryName.ToLower() && c.Id != countryId);
        }

        public async Task<CountryDto> CreateAsync(CountryDto countryDto)
        {
            try
            {
                await _context.Countries.AddAsync(_mapper.Map<Country>(countryDto));
                await _context.SaveChangesAsync();
                return countryDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task<CountryDto> UpdateAsync(int countryId, CountryDto countryDto)
        {
            try
            {
                var country = await _context.Countries.Where(p => p.Id == countryId && p.Hidden == false).FirstOrDefaultAsync();
                if (country == null)
                    throw new Exception("Country not found!");

                _mapper.Map(countryDto, country);
                _context.Countries.Update(country);
                await _context.SaveChangesAsync();

                return countryDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task ArchiveAsync(int countryId)
        {
            try
            {
                var country = await _context.Countries.FindAsync(countryId);

                if (country == null)
                    throw new Exception("Country not found!");

                country.Hidden = !country.Hidden;
                _context.Countries.Update(country);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task MultiArchiveAsync(int[] countryIds)
        {
            try
            {
                var checkValid = _context.Countries.Where(c => countryIds.Contains(c.Id)).Count();
                if (checkValid != countryIds.Length)
                    throw new Exception("One or more countries not found!");

                var countries = await _context.Countries.Where(c => countryIds.Contains(c.Id)).ToListAsync();

                foreach (var country in countries)
                {
                    country.Hidden = !country.Hidden;
                }
                _context.Countries.UpdateRange(countries);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task DeleteAsync(int countryId)
        {
            try
            {
                var country = await _context.Countries.Where(p => p.Id == countryId && p.Hidden == true).FirstOrDefaultAsync();
                if (country == null)
                    throw new Exception("Country not found!");
                _context.Countries.Remove(country);

                var owners = await _context.Owners.Where(c => c.Country.Id == countryId).ToListAsync();
                if (owners == null)
                    throw new Exception("Pokemon category not found!");
                foreach (var owner in owners)
                {
                    owner.Country = null;
                }
                _context.Owners.UpdateRange(owners);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task MultiDeleteAsync(int[] countryIds)
        {
            try
            {
                var checkValid = _context.Countries.Where(c => countryIds.Contains(c.Id) && c.Hidden == true).Count();
                if (checkValid != countryIds.Length)
                    throw new Exception("One or more countries not found!");

                var countries = await _context.Countries.Where(c => countryIds.Contains(c.Id)).ToListAsync();
                _context.Countries.RemoveRange(countries);

                var owners = await _context.Owners.Where(c => countryIds.Contains(c.Country.Id)).ToListAsync();
                foreach (var owner in owners)
                {
                    owner.Country = null;
                }
                _context.Owners.UpdateRange(owners);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task<List<CountryDto>> GetCountriesAsync(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var data = await _context.Countries.Where(c => c.Hidden == false)
                                               .Include(c => c.Owners)
                                               .Select(p => new CountryDto
                                               {
                                                   Id = p.Id,
                                                   Name = p.Name,
                                                   OwnerName = p.Owners.Where(o => o.Hidden == false).Select(o => o.Name).ToList(),
                                               })
                                               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                                               .Take(validFilter.PageSize)
                                               .ToListAsync();
            return data;
        }

        public async Task<CountryDto> GetCountryAsync(int countryId)
        {
            var data = await _context.Countries.Where(c => c.Hidden == false && c.Id == countryId)
                                               .Include(c => c.Owners)
                                               .Select(p => new CountryDto
                                               {
                                                   Id = p.Id,
                                                   Name = p.Name,
                                                   OwnerName = p.Owners.Where(o => o.Hidden == false).Select(o => o.Name).ToList(),
                                               })
                                               .FirstOrDefaultAsync();
            return data;
        }

        public async Task<CountryDto> GetCountryOfOwnerAsync(int ownerId)
        {
            var data = await _context.Owners
                             .Where(o => o.Hidden == false && o.Id == ownerId)
                             .Select(o => new CountryDto
                             {
                                 Id = o.Country.Id,
                                 Name = o.Country.Name,
                                 Hidden = o.Country.Hidden,
                             })
                             .FirstOrDefaultAsync();

            return data;
        }

        public async Task<List<OwnerDto>> GetOwnersFromACountryAsync(int countryId)
        {
            var data = await _context.Owners
                                     .Where(o => o.Country.Id == countryId)
                                     .Select(o => new OwnerDto
                                     {
                                         Id = o.Id,
                                         Name = o.Name,
                                         Gym = o.Gym,
                                         CountryName = o.Country.Name,
                                     })
                                     .ToListAsync();
            return data;
        }
    }
}