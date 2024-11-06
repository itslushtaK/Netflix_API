﻿namespace Domain.Entities
{
    public class Producent : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Photo { get; set; }
        public string Country {  get; set; }
        public ICollection<Movie> Movies { get; set; }
    }
}
