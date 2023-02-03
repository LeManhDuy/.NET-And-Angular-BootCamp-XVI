using api.Dto;
using api.Filter;
using server.Models;

namespace server.Interfaces
{
    public interface IOwnerRepository
    {
        Task<List<OwnerDto>> GetOwnersAsync(PaginationFilter filter);
        Task<Owner> GetOwnerAsync(int ownerId);
        Task<List<Owner>> GetOwnerOfPokemonAsync(int pokemonId);
        Task<List<Pokemon>> GetPokemonByOwnerAsync(int ownerId);
        Task<OwnerDto> CreateAsync(int countryId, OwnerDto ownerDto);
        Task<OwnerDto> UpdateAsync(int countryId, int ownerId, OwnerDto ownerDto);
        Task<bool> MultiUpdateAsync(int countryId, int[] ownerIdArray);
        Task ArchiveAsync(int ownerId);
        Task MultiArchiveAsync(int[] ownerIds);
        Task DeleteAsync(int ownerId);
        Task MultiDeleteAsync(int[] ownerIds);
        bool OwnerExist(int ownerId);
        bool OwnerExist(int ownerId, string ownerName);
    }
}