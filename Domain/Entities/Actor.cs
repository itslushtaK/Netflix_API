namespace Domain.Entities
{
    public class Actor : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName{ get; set; }
        public int? Age { get; set; }
        public string? Photo { get; set; }
        public string Country { get; set; }
        public ICollection<ActorMovie>? ActorMovies { get; set; }
    }
}
