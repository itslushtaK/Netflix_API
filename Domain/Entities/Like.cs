﻿
namespace Domain.Entities
{
    public class Like
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public string UserId { get; set; }
    }
}
