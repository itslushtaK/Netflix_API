using Application.Common.Abstractions;
using MediatR;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Actors
{
    public class Delete
    {
        public record Command(int id): IRequest;

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context=context;
            }
            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var actor = await _context.Actors.FindAsync(request.id);

                // Check if actor is found
                if (actor == null)
                {
                    throw new NotFoundException();
                }

                _context.Actors.Remove(actor);
                await _context.SaveChangesAsync(cancellationToken);

            }
        }
    }
}
