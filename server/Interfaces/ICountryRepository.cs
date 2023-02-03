using api.Dto;
using api.Filter;

namespace server.Interfaces
{
    public interface ICountryRepository
    {
        Task<List<CountryDto>> GetCountriesAsync(PaginationFilter filter);
        Task<CountryDto> GetCountryAsync(int countryId);
        Task<CountryDto> GetCountryOfOwnerAsync(int ownerId);
        Task<List<OwnerDto>> GetOwnersFromACountryAsync(int countryId);
        Task<CountryDto> CreateAsync(CountryDto countryDto);
        Task<CountryDto> UpdateAsync(int countryId, CountryDto countryDto);
        Task ArchiveAsync(int countryId);
        Task MultiArchiveAsync(int[] countryIds);
        Task DeleteAsync(int countryId);
        Task MultiDeleteAsync(int[] countryIds);
        bool CountryExists(int countryId);
        bool CountryExists(string countryName, int countryId);
    }
}