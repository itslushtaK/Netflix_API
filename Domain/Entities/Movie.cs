using Domain.Enums;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Movie : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        //public decimal Price { get; set; }
        public int? Rating { get; set;}
        public Category Category { get; set; }
        public string? Photo { get; set; }
        public string? Video { get; set; }
        public string? VideoPublicId { get; set; }
        public int? LikeCount { get; set; }
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<ActorMovie> ActorMovies { get; set; }
        public int ProducentId { get; set; }
        public Producent Producent { get; set; }
        public void UpdateLikeCount()
        {
            LikeCount = Likes.Count;
        }
    }
}
