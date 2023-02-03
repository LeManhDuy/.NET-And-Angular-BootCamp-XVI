using api.Dto;
using api.Filter;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dto;
using server.Interfaces;
using server.Models;

namespace server.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public OwnerRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<OwnerDto>> GetOwnersAsync(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var data = await _context.Owners.Where(c => c.Hidden == false)
                                               .Include(c => c.Country)
                                               .Select(p => new OwnerDto
                                               {
                                                   Id = p.Id,
                                                   Name = p.Name,
                                                   CountryName = p.Country.Name,
                                               })
                                               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                                               .Take(validFilter.PageSize)
                                               .ToListAsync();
            return data;
        }

        public async Task<Owner> GetOwnerAsync(int ownerId)
        {
            return await _context.Owners.Where(o => o.Id == ownerId && o.Hidden == false).OrderBy(o => o.Name).FirstOrDefaultAsync();
        }

        public async Task<List<Owner>> GetOwnerOfPokemonAsync(int pokemonId)
        {
            return await _context.PokemonOwners.Where(po => po.PokemonId == pokemonId).Select(po => po.Owner).ToListAsync();
        }

        public async Task<List<Pokemon>> GetPokemonByOwnerAsync(int ownerId)
        {
            return await _context.PokemonOwners.Where(po => po.OwnerId == ownerId).Select(po => po.Pokemon).ToListAsync();
        }

        public bool OwnerExist(int ownerId)
        {
            return _context.Owners.Any(o => o.Id == ownerId && o.Hidden == false);
        }
        public bool OwnerExist(int ownerId, string ownerName)
        {
            return _context.Owners.Any(o => o.Id != ownerId && o.Name.ToLower() == ownerName.ToLower());
        }

        public async Task<OwnerDto> CreateAsync(int countryId, OwnerDto ownerDto)
        {
            try
            {
                var country = await _context.Countries.Where(c => c.Id == countryId && c.Hidden == false).FirstOrDefaultAsync();
                if (country == null)
                    throw new Exception("One or more owners not found!");

                var owner = new Owner()
                {
                    Name = ownerDto.Name,
                    Gym = ownerDto.Gym,
                    Hidden = ownerDto.Hidden,
                    Country = country
                };

                await _context.Owners.AddAsync(owner);
                await _context.SaveChangesAsync();

                return ownerDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task<OwnerDto> UpdateAsync(int countryId, int ownerId, OwnerDto ownerDto)
        {
            try
            {
                var country = await _context.Countries.Where(p => p.Id == countryId && p.Hidden == false).FirstOrDefaultAsync();
                if (country == null)
                    throw new Exception("Country not found!");

                var owner = await _context.Owners.Where(p => p.Id == ownerId && p.Hidden == false).FirstOrDefaultAsync();
                if (owner == null)
                    throw new Exception("Owner not found!");

                owner.Name = ownerDto.Name;
                owner.Gym = ownerDto.Gym;
                owner.Hidden = ownerDto.Hidden;
                owner.Country = country;

                _context.Owners.Update(owner);
                await _context.SaveChangesAsync();

                return ownerDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task<bool> MultiUpdateAsync(int countryId, int[] ownerIdArray)
        {
            try
            {
                var checkValidOwners = _context.Owners.Count(c => ownerIdArray.Contains(c.Id) && c.Hidden == false);
                if (checkValidOwners != ownerIdArray.Length)
                    throw new Exception("One or more owners not found!");

                var country = await _context.Countries.Where(p => p.Id == countryId && p.Hidden == false).FirstOrDefaultAsync();
                if (country == null)
                    throw new Exception("Country not found!");

                var owners = await _context.Owners.Where(c => ownerIdArray.Contains(c.Id) && c.Hidden == false).ToListAsync();

                foreach (var owner in owners)
                {
                    owner.Country = country;
                    _context.Update(owner);
                }
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task ArchiveAsync(int ownerId)
        {
            try
            {
                var owner = await _context.Owners.Where(p => p.Id == ownerId).FirstOrDefaultAsync();
                if (owner == null)
                    throw new Exception("Pokemon not found!");

                owner.Hidden = !owner.Hidden;
                _context.Owners.Update(owner);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task MultiArchiveAsync(int[] ownerIds)
        {
            try
            {
                var checkValid = _context.Owners.Count(c => ownerIds.Contains(c.Id));
                if (checkValid != ownerIds.Length)
                    throw new Exception("One or more owner not found!");

                var owners = await _context.Owners.Where(c => ownerIds.Contains(c.Id)).ToListAsync();
                foreach (var owner in owners)
                {
                    owner.Hidden = !owner.Hidden;
                }
                _context.Owners.UpdateRange(owners);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task DeleteAsync(int ownerId)
        {
            try
            {
                var owner = await _context.Owners.Where(p => p.Id == ownerId && p.Hidden == true).FirstOrDefaultAsync();
                if (owner == null)
                    throw new Exception("Owner not found!");
                _context.Owners.Remove(owner);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task MultiDeleteAsync(int[] ownerIds)
        {
            try
            {
                var checkValid = _context.Owners.Count(c => ownerIds.Contains(c.Id));
                if (checkValid != ownerIds.Length)
                    throw new Exception("One or more owner not found!");

                var owners = await _context.Owners.Where(c => ownerIds.Contains(c.Id) && c.Hidden == true).ToListAsync();
                _context.Owners.RemoveRange(owners);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }
    }
}