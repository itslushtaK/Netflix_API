using Application.Common.Abstractions;
using MediatR;

namespace Application.Features.Movies
{
    public class AddRating
    {
        public record Command(int MovieId, int Value) : IRequest
        {

        }
        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;
            public CommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                
                await _context.SaveChangesAsync(cancellationToken);

            }
        }
    }
}
