using Application.Common.Abstractions;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Application.Features.Movies
{
    public class Add
    {
        public record Command(string Name,
                              string Description,
                              Category Category,
                              int ProducentId,
                              IFormFile? Photo,
                              IFormFile? Video,
                              ICollection<int>? ActorIds) : IRequest
        {
            public Movie ToEntity() => new Movie
            {
                Name = Name,
                Description = Description,
                Category = Category,
                CreatedAt = DateTime.UtcNow,
                ProducentId = ProducentId,
            };
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;
            private readonly IPhotoService _photoService;
            private readonly ILogger<CommandHandler> _logger; 

            public CommandHandler(IApplicationDbContext context, IPhotoService photoService, ILogger<CommandHandler> logger)
            {
                _context = context;
                _photoService = photoService;
                _logger = logger;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var movie = request.ToEntity();

                if (request.Photo is not null)
                {
                    var uploadResult = await _photoService.AddPhoto(request.Photo);
                    movie.Photo = uploadResult.SecureUri.AbsoluteUri;
                }

                if (request.Video is not null)
                {
                   
                    var uploadResult = await _photoService.UploadVideoAsync(request.Video);
                   
                    movie.Video = uploadResult.SecureUri.AbsoluteUri;
                    movie.VideoPublicId = uploadResult.PublicId;
                }

                if (request.ActorIds != null && request.ActorIds.Any())
                {
                    var actors = await _context.Actors
                        .Where(a => request.ActorIds.Contains(a.Id))
                        .ToListAsync(cancellationToken);

                    var actorMovies = actors.Select(actor => new ActorMovie
                    {
                        ActorId = actor.Id,
                        MovieId = movie.Id
                    }).ToList();

                    await _context.ActorMovies.AddRangeAsync(actorMovies, cancellationToken);
                }

                await _context.Movies.AddAsync(movie, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Movie saved. Movie Id: {MovieId}", movie.Id);

            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            IApplicationDbContext context;
            public CommandValidator()
            {
                RuleFor(movie => movie.Name)
                    .NotEmpty()
                    .WithMessage("Name is required!");

                RuleFor(movie => movie.Description)
                    .NotEmpty()
                    .WithMessage("Description is required!");

                //RuleFor(movie => movie.ProducentId)
                //    .NotNull()
                //    .MustAsync(IsValidProducent)
                //    .WithMessage("Producent doesn't exist");

                RuleFor(movie => movie.Category)
                    .NotEmpty()
                    .WithMessage("Category is required!")
                    .MustAsync(IsValidCategory)
                    .WithMessage("Invalid Category");
            }

            //public async Task<bool> IsValidProducent(int producentId, CancellationToken cancellationToken)
            //{
            //    return await context.Producents.AnyAsync(producent => producent.Id == producentId, cancellationToken);
            //}
            private async Task<bool> IsValidCategory(Category category, CancellationToken cancellationToken)
            {
                var validCategories = Enum.GetValues(typeof(Category)).Cast<Category>();
                return validCategories.Contains(category);
            }
        }
    }
}
