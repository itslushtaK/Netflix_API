using Application.Common.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Application.Features.Movies
{
    public class AddLike
    {
        public record Command(int id, bool? like, string userId) : IRequest;

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
                    .Include(m => m.Likes)
                    .FirstOrDefaultAsync(movie => movie.Id == request.id, cancellationToken);

                if (movie == null)
                {
                    throw new NotFoundException();
                }
                var existingLike = movie.Likes.FirstOrDefault(l => l.UserId == request.userId);

                if (request.like.HasValue)
                {
                    if (request.like.Value && existingLike == null)
                    {
                        movie.Likes.Add(new Like { MovieId = request.id, UserId = request.userId });
                    }
                    else if (!request.like.Value && existingLike != null)
                    {
                        movie.Likes.Remove(existingLike);
                    }

                    movie.UpdateLikeCount();
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
}
