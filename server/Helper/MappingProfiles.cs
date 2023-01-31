using AutoMapper;
using server.Dto;
using server.Models;

namespace server.Helper
{
  //am-profile
  public class MappingProfiles : Profile
  {
    public MappingProfiles()
    {
      CreateMap<Pokemon, PokemonDto>();
      CreateMap<PokemonDto, Pokemon>();
      CreateMap<Category, CategoryDto>();
      CreateMap<CategoryDto, Category>();
    }
  }
}