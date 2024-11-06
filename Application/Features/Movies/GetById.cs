using Application.Common.Abstractions;
using Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Application.Features.Movies
{
    public class GetById
    {
        public record Query(int id) : IRequest<Response>;
        public class QueryHandler : IRequestHandler<Query, Response>
        {
            private readonly IApplicationDbContext _context;
            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var movie = await _context.Movies
                    .Include(movie => movie.ActorMovies)
                        .ThenInclude(actorMovie => actorMovie.Actor)
                    .FirstOrDefaultAsync(movie => movie.Id == request.id);

                if (movie == null)
                {
                    throw new NotFoundException();
                }

                return new Response(
                    movie.Id,
                    movie.Name,
                    movie.Description,
                    //movie.LikeCount,
                    movie.Category.GetDescription(),
                    movie.CreatedAt,
                    movie.ActorMovies.Select(actorMovie => new ActorDto(actorMovie.ActorId,actorMovie.Actor.FirstName , actorMovie.Actor.LastName, actorMovie.Actor.Photo)).ToList(),
                    movie.Video
                );
            }
        }
        public record Response(int id, string name, string description,/*int likes */ string category, DateTime createdAt, ICollection<ActorDto> actors, string? video);
        public record ActorDto(int Id, string firstName , string lastName , string? PhotoUrl); 
    }
}
