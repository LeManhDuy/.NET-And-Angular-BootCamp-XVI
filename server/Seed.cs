using server.Data;
using server.Models;

namespace server
{
  public class Seed
  {
    private readonly DataContext dataContext;
    public Seed(DataContext context)
    {
      this.dataContext = context;
    }
    public void SeedDataContext()
    {
      if (!dataContext.PokemonOwners.Any())
      {
        var pokemonOwners = new List<PokemonOwner>()
                {
                    new PokemonOwner()
                    {
                        Pokemon = new Pokemon()
                        {
                            Name = "Pikachu",
                            BirthDate = new DateTime(1903,1,1),
                            Hidden = false,
                            PokemonCategories = new List<PokemonCategory>()
                            {
                                new PokemonCategory { Category = new Category() { Name = "Electric",Hidden = false,}}
                            },
                            Reviews = new List<Review>()
                            {
                                new Review { Title="Pikachu",Text = "Pickahu is the best pokemon, because it is electric", Rating = 5, Hidden = false,
                                Reviewer = new Reviewer(){ FirstName = "Teddy", LastName = "Smith", Hidden = false, } },
                                new Review { Title="Pikachu", Text = "Pickachu is the best a killing rocks", Rating = 5, Hidden = false,
                                Reviewer = new Reviewer(){ FirstName = "Taylor", LastName = "Jones", Hidden = false, } },
                                new Review { Title="Pikachu",Text = "Pickchu, pickachu, pikachu", Rating = 1, Hidden = false,
                                Reviewer = new Reviewer(){ FirstName = "Jessica", LastName = "McGregor", Hidden = false, } },
                            }
                        },
                        Owner = new Owner()
                        {
                            Name = "Jack",
                            Gym = "Brocks Gym",
                            Hidden = false,
                            Country = new Country()
                            {
                                Name = "Kanto"
                            }
                        }
                    },

                    new PokemonOwner()
                    {
                        Pokemon = new Pokemon()
                        {
                            Name = "Venasuar",
                            BirthDate = new DateTime(1903,1,1),
                            Hidden = true,
                            PokemonCategories = new List<PokemonCategory>()
                            {
                                new PokemonCategory { Category = new Category() { Name = "Leaf",Hidden = true,}}
                            },
                            Reviews = new List<Review>()
                            {
                                new Review { Title="Veasaur",Text = "Venasuar is the best pokemon, because it is electric", Rating = 5,Hidden = true,
                                Reviewer = new Reviewer(){ FirstName = "Teddy", LastName = "Smith",Hidden = true, } },
                                new Review { Title="Veasaur",Text = "Venasuar is the best a killing rocks", Rating = 5,Hidden = true,
                                Reviewer = new Reviewer(){ FirstName = "Taylor", LastName = "Jones",Hidden = true, } },
                                new Review { Title="Veasaur",Text = "Venasuar, Venasuar, Venasuar", Rating = 1,Hidden = true,
                                Reviewer = new Reviewer(){ FirstName = "Jessica", LastName = "McGregor",Hidden = true, } },
                            }
                        },
                        Owner = new Owner()
                        {
                            Name = "Ash",
                            Gym = "Ashs Gym",
                            Hidden = true,
                            Country = new Country()
                            {
                                Name = "Millet Town"
                            }
                        }
                    }
                };
        dataContext.PokemonOwners.AddRange(pokemonOwners);
        dataContext.SaveChanges();
      }
    }
  }
}