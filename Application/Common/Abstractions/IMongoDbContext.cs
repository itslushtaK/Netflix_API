using Domain.Entities;
using MongoDB.Driver;

namespace Application.Common.Abstractions
{
    public interface IMongoDbContext
    {
        IMongoCollection<Movie> Movies { get; }
    }
}
