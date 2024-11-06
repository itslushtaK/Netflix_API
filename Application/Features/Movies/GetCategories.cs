using Domain.Enums;
using MediatR;

namespace Application.Features.Movies
{
    public class GetCategories
    {
        public record Query : IRequest<Response>;

        public class QueryHandler : IRequestHandler<Query, Response>
        {
            public Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var categories = Enum.GetNames(typeof(Category)).ToList();
                return Task.FromResult(new Response(categories));
            }
        }
        public record Response(List<string> Names);
    }
}
