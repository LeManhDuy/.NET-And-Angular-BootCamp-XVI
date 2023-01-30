using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public ICollection<Country> GetCountries()
    {
      return _context.Countries.Where(c => c.Hidden == false).ToList();
    }

    public Country GetCountry(int countryId)
    {
      return _context.Countries.Where(c => c.Hidden == false && c.Id == countryId).FirstOrDefault();
    }

    public Country GetCountryByOwner(int ownerId)
    {
      return _context.Owners.Where(o => o.Hidden == false && o.Id == ownerId).Select(c => c.Country).FirstOrDefault();
    }

    public ICollection<Owner> GetOwnersFromACountry(int countryId)
    {
      return _context.Owners.Where(o => o.Country.Id == countryId).ToList();
    }
  }
}