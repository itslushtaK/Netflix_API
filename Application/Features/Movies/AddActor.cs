using Application.Common.Abstractions;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;
using System.Runtime.CompilerServices;

namespace Application.Features.Movies
{
    public class AddActor
    {
        public record Command(int MovieId, IEnumerable<int> ActorIds) : IRequest;

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var movie = await _context.Movies
                    .Include(m => m.ActorMovies)
                    .FirstOrDefaultAsync(m => m.Id == request.MovieId, cancellationToken);

                if (movie == null)
                    throw new NotFoundException($"Movie with ID {request.MovieId} is not found.");

                var existingActorIds = movie.ActorMovies.Select(am => am.ActorId);
                var newActorIds = request.ActorIds.Except(existingActorIds);

                foreach (var actorId in newActorIds)
                {
                    movie.ActorMovies.Add(new ActorMovie
                    {
                        ActorId = actorId,
                        MovieId = movie.Id
                    });
                }

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandValidator(IApplicationDbContext context)
            {
                _context = context;

                RuleFor(actormovie => actormovie.ActorIds)
                    .NotEmpty().WithMessage("ActorId is required")
                    .MustAsync(ActorsExistAsync).WithMessage("Actor doesn't exist");
            }

            private async Task<bool> ActorsExistAsync(IEnumerable<int> actorIds, CancellationToken cancellationToken)
            {
                if (actorIds == null || !actorIds.Any())
                    return false;

                var existingActorIds = await _context.Actors
                    .AnyAsync(actor => actorIds.Contains(actor.Id), cancellationToken);

                return existingActorIds && actorIds.Distinct().Count() == actorIds.Count();
            }

        }

    }
}
