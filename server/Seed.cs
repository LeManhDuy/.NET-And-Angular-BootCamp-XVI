using Faker;
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
            for (int i = 1; i <= 2000; i++)
            {
                var pokemonOwner = new PokemonOwner()
                {
                    Pokemon = new Pokemon()
                    {
                        Name = Name.FullName(NameFormats.WithPrefix),
                        BirthDate = DateTime.Now.AddDays(new Random().Next(1000)),
                        Hidden = false,
                        PokemonCategories = new List<PokemonCategory>()
                                {
                                    new PokemonCategory { Category = new Category() { Name = Name.FullName(),Hidden = false,}}
                                },
                        Reviews = new List<Review>()
                                {
                                    new Review { Title=Address.City(),Text = Lorem.Sentences(3).ToString(), Rating = RandomNumber.Next(0, 10), Hidden = false,
                                    Reviewer = new Reviewer(){ FirstName =  Name.First(), LastName = Name.Last(), Hidden = false, } },
                                    new Review { Title=Address.City(),Text = Lorem.Sentences(3).ToString(), Rating = RandomNumber.Next(0, 10), Hidden = false,
                                    Reviewer = new Reviewer(){ FirstName =  Name.First(), LastName = Name.Last(), Hidden = false, } }
                                }
                    },
                    Owner = new Owner()
                    {
                        Name = Name.FullName(NameFormats.WithPrefix),
                        Gym = Address.City(),
                        Hidden = false,
                        Country = new Models.Country()
                        {
                            Name = Address.Country()
                        }
                    }
                };
                dataContext.PokemonOwners.Add(pokemonOwner);
                dataContext.SaveChanges();
            }
        }
    }
}