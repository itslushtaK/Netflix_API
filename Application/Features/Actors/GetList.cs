using Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Application.Features.Actors
{
    public class GetList
    {
        public record Query() : IRequest<List<Response>>;

        public class QueryHandler : IRequestHandler<Query, List<Response>>
        {
            private readonly IApplicationDbContext _context;
            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<Response>> Handle(Query request, CancellationToken cancellationToken)
            {
                var actors = _context.Actors.OrderByDescending(actor => actor.Id);
                if (actors is null)
                {
                    throw new NotFoundException();
                }
                return await actors.Select(actor => new Response(actor.Id, actor.FirstName, actor.LastName, actor.Age, actor.CreatedAt ,actor.Country, actor.Photo))
                                   .ToListAsync();
            }
        }
        public record Response(int Id, string FirstName, string LastName, int? Age, DateTime CreatedAt, string Country, string? photo);
    }
}
