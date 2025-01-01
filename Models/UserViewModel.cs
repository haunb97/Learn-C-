namespace RunGroopWebApp.Models
{
    public class UserViewModel
    {
        public string? Id { get; set; }
        public string UserName { get; set; }
        public int? Pace { get; set; }
        public int? Mileage { get; set; }
        public string? ProfileImageUrl { get; set; }
        //[ForeignKey("Address")]
        //public int? AddressId { get; set; }
        //public Address? Address { get; set; }

        //public ICollection<Club> Clubs { get; set; }

        //public ICollection<Race> Races { get; set; }
    }
}
