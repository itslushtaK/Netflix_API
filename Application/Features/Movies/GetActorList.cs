using Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Movies
{
    public class GetActorList
    {
        public record Query(int movieId) : IRequest<List<Response>>;

        public class QueryHandler : IRequestHandler<Query, List<Response>>
        {
            private readonly IApplicationDbContext _context;
            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var actors = await _context.ActorMovies
                    .Where(ma => ma.MovieId == request.movieId)
                    .Select(ma => new Response(
                        ma.Actor.FirstName,
                        ma.Actor.LastName
                    ))
                    .ToListAsync(cancellationToken);

                return actors;
            }
        }

        public record Response(string FirstName, string LastName);
    }
}
