using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Abstractions
{
    public interface IApplicationDbContext
    {
        DbSet<Movie> Movies { get; set; }
        DbSet<Actor> Actors { get; set; }
        DbSet<ActorMovie> ActorMovies { get; set; }
        DbSet<Producent> Producents { get; set; }
        DbSet<Like> Likes { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
