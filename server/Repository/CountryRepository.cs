using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Interfaces;
using server.Models;

namespace server.Repository
{
  public class CountryRepository : ICountryRepository
  {
    private readonly DataContext _context;

    public CountryRepository(DataContext context)
    {
      _context = context;
    }

    public bool CountryExists(int countryId)
    {
      return _context.Countries.Any(c => c.Hidden == false && c.Id == countryId);
    }

    public async Task<List<Country>> GetCountriesAsync()
    {
      return await _context.Countries.Where(c => c.Hidden == false).ToListAsync();
    }

    public async Task<Country> GetCountryAsync(int countryId)
    {
      return await _context.Countries.Where(c => c.Hidden == false && c.Id == countryId).FirstOrDefaultAsync();
    }

    public async Task<Country> GetCountryOfOwnerAsync(int ownerId)
    {
      return await _context.Owners.Where(o => o.Hidden == false && o.Id == ownerId).Select(c => c.Country).FirstOrDefaultAsync();
    }

    public async Task<List<Owner>> GetOwnersFromACountryAsync(int countryId)
    {
      return await _context.Owners.Where(o => o.Country.Id == countryId).ToListAsync();
    }
  }
}