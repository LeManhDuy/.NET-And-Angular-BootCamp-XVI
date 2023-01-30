using server.Models;

namespace server.Interfaces
{
  public interface ICountryRepository
  {
    Task<List<Country>> GetCountriesAsync();
    Task<Country> GetCountryAsync(int countryId);
    Task<Country> GetCountryOfOwnerAsync(int ownerId);
    Task<List<Owner>> GetOwnersFromACountryAsync(int countryId);
    bool CountryExists(int countryId);
  }
}