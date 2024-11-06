using Application.Common.Abstractions;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Actors
{
    public class Create
    {
        public record Command(string firstName, string lastName, int? age, string country, IFormFile? profilephoto) : IRequest
        {
            public Actor ToEntity() => new Actor
            {
                FirstName = firstName,
                LastName = lastName,
                Age = age,
                CreatedAt = DateTime.UtcNow,
                Country = country,
            };
        }
        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;
            private readonly IHttpContextAccessor _httpContextAccessor;

            private readonly IPhotoService _photoService;
            public CommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IPhotoService photoService)
            {
                _context = context;
                _httpContextAccessor = httpContextAccessor;
                _photoService = photoService;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var actor = request.ToEntity();
                string photoUrl = null;
                if (request.profilephoto != null)
                {
                    var uploadResult = await _photoService.AddPhoto(request.profilephoto);
                    photoUrl = uploadResult.SecureUri.AbsoluteUri;
                    actor.Photo = photoUrl;
                }

                _context.Actors.Add(actor);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public class CommandValidatior : AbstractValidator<Command>
        {
            public CommandValidatior()
            {
                RuleFor(actor => actor.firstName)
                    .NotEmpty()
                    .WithMessage("FirstName is required");

                RuleFor(actor => actor.lastName)
                    .NotEmpty()
                    .WithMessage("LastName is required");

                RuleFor(actor => actor.country)
                    .NotEmpty()
                    .WithMessage("Country is required");
            }
        }
    }
}
