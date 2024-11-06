using Application.Common.Abstractions;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using SendGrid.Helpers.Errors.Model;

namespace Application.Features.Movies
{
    public class Update
    {

        public record Command(int id, string name,
                              string description, int rating,
                              Category category, int producentId,
                              IFormFile? photo, IFormFile? video,
                              ICollection<int>? ActorIds) : IRequest;

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;
            private readonly IPhotoService _photoService;

            public CommandHandler(IApplicationDbContext context, IPhotoService photoService)
            {
                _context = context;
                _photoService = photoService;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var movie = _context.Movies.FirstOrDefault(movie => movie.Id == request.id);

                if (movie is null)
                {
                    throw new NotFoundException("No Movie FOUND!!!");
                }

                movie.Name = request.name;
                movie.Description = request.description;
                movie.Category = request.category;
                movie.ProducentId = request.producentId;
                movie.UpdatedAt = DateTime.UtcNow;
                if (request.photo != null)
                {
                    var uploadResult = await _photoService.AddPhoto(request.photo);
                    movie.Photo = uploadResult.SecureUri.AbsoluteUri;
                }

                if (request.video != null)
                {
                    if(movie.VideoPublicId is not null)
                    {
                        await _photoService.DeletePhoto(movie.VideoPublicId);
                    }
                    var uploadResult = await _photoService.UploadVideoAsync(request.video);
                    movie.Video = uploadResult.SecureUri.AbsoluteUri;
                    movie.VideoPublicId = uploadResult.PublicId;
                }

                if (request.ActorIds != null && request.ActorIds.Any())
                {
                    var existingActorMovies = _context.ActorMovies
                            .Where(am => am.MovieId == request.id);

                    _context.ActorMovies.RemoveRange(existingActorMovies);

                    foreach(var actorId in request.ActorIds)
                    {
                        _context.ActorMovies.Add(new ActorMovie
                        {
                            MovieId = request.id,
                            ActorId = actorId
                        });
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                    }
                }
            }

            public class CommandValidator : AbstractValidator<Command>
            {
                public CommandValidator()
                {
                    RuleFor(movie => movie.name)
                        .NotEmpty()
                        .WithMessage("Name is required!");

                    RuleFor(movie => movie.description)
                        .NotEmpty()
                        .WithMessage("Description is required!");

                    RuleFor(movie => movie.category)
                        .NotEmpty()
                        .WithMessage("Category is required!");

                    RuleFor(movie => movie.producentId)
                        .NotEmpty()
                        .WithMessage("ProducentId is required!");
                }
            }
        }
    }

