using Application.Common.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;
using Application.Common.Exceptions;

namespace Application.Features.Movies
{
    public class GetList
    {
        public record Query(string? name, Category? category) : IRequest<List<Response>>;

        public class QueryHandler : IRequestHandler<Query, List<Response>>
        {
            private readonly IApplicationDbContext _context;

            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Movies.Include(movie => movie.ActorMovies)
                                            .Include(movie => movie.Likes) // Include Likes to access their count
                                            .OrderByDescending(movie => movie.CreatedAt)
                                            .AsQueryable();

                if (request.name != null)
                {
                    var movie = await query.FirstOrDefaultAsync(movie => movie.Name == request.name);

                    if (movie == null)
                    {
                        throw new NotFoundException();
                    }

                    // Update like count based on likes
                    movie.UpdateLikeCount();

                    var response = new Response(
                        movie.Id,
                        movie.Name,
                        movie.Description,
                        movie.Rating,
                        movie.LikeCount,
                        movie.Photo,
                        movie.Category.GetDescription(),
                        movie.CreatedAt,
                        movie.ActorMovies.Select(actorMovie => actorMovie.ActorId).ToList(),
                        movie.Video
                    );

                    return new List<Response> { response };
                }
                else
                {
                    if (request.category != null)
                    {
                        query = query.Where(movie => movie.Category == request.category);
                    }

                    var movies = await query.ToListAsync();

                    if (movies.Count == 0)
                    {
                        throw new NotFoundException();
                    }

                    // Update like counts for each movie
                    foreach (var movie in movies)
                    {
                        movie.UpdateLikeCount(); // Ensure LikeCount is updated
                    }

                    return movies.Select(movie => new Response(
                        movie.Id,
                        movie.Name,
                        movie.Description,
                        movie.Rating,
                        movie.LikeCount,
                        movie.Photo,
                        movie.Category.GetDescription(),
                        movie.CreatedAt,
                        movie.ActorMovies.Select(actorMovie => actorMovie.ActorId).ToList(),
                        movie.Video
                    )).ToList();
                }
            }

        }
        public record Response(int id, string name, string description, int? rating,int? likeCount, string? photo, string category, DateTime createdAt, ICollection<int> actorIds, string? video);
    }
}
