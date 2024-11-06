using Application.Common.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Application.Features.Actors
{
    public class GetMovies
    {

        public record Query(int actorid) : IRequest<List<Response>>;

        public class QueryHandler : IRequestHandler<Query, List<Response>>
        {
            private readonly IApplicationDbContext _context;
            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var movies = await _context.ActorMovies.Where(x => x.ActorId == request.actorid)
                                                        .Select(x => new Response(
                                                            x.Movie.Name)
                ).ToListAsync(cancellationToken);

                
                if (movies == null)
                {
                    throw new NotFoundException("MovieNot Found!");
                }

                return movies;
            }
        }
        public record Response(string title);
    }
}
