using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Models;

namespace server.Interfaces
{
  public interface ICountryRepository
  {
    ICollection<Country> GetCountries();
    Country GetCountry(int countryId);
    Country GetCountryByOwner(int ownerId);
    ICollection<Owner> GetOwnersFromACountry(int countryId);
    bool CountryExists(int countryId);
  }
}