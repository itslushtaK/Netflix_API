using Application.Common.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Owin;
using SendGrid.Helpers.Errors.Model;
using System.Diagnostics;

namespace Application.Features.Producents
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
                IQueryable<Producent> producents = _context.Producents.OrderByDescending(producent => producent.Id);

                if(producents == null)
                {
                    throw new NotFoundException("producent is null");
                }

                return await producents.Select(producent => new Response(producent.Id,producent.FirstName, producent.LastName, producent.Country, producent.Photo, producent.CreatedAt)).ToListAsync();
            }
        }

        public record Response(int id ,string firstName , string lastName , string country , string photo , DateTime createdAt);
    }
}
