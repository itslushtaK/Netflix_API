using Application.Common.Abstractions;
using Domain.Entities;
using FluentValidation;
using MediatR;
using SendGrid.Helpers.Errors.Model;

namespace Application.Features.Actors
{
    public class GetById
    {
        public record Query(int id ): IRequest<Response>;
        
        public class QueryHandler : IRequestHandler<Query , Response>
        {
            private readonly IApplicationDbContext _context;
            public QueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var actor = _context.Actors.OrderByDescending(actor => actor.Id).FirstOrDefault(actor=> actor.Id == request.id);
                if(actor is null)
                {
                    throw new NotFoundException();
                }
                return new Response(actor.Id, actor.FirstName, actor.LastName,actor.Age, actor.CreatedAt, actor.Photo);
            }
        }
        public record Response(int id , string FirstName , string LastName ,int? age , DateTime CreatedAt , string? photo);
    }
}
