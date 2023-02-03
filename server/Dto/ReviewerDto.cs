using server.Models;

namespace api.Dto
{
    public class ReviewerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Hidden { get; set; }
        public List<Review> ReviewInformation { get; set; }
    }
}
