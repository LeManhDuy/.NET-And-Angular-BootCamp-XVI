namespace api.Dto
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Hidden { get; set; }
        public List<string> OwnerName { get; set; }
    }
}
