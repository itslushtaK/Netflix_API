using Application.Common.Abstractions;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Producents
{
    public class Create
    {
        public record Command(string firstName, string lastName, string country, IFormFile? photo) : IRequest
        {
            public Producent ToEntity() => new Producent
            {
                FirstName = firstName,
                LastName = lastName,
                Country = country,
                CreatedAt = DateTime.UtcNow
            };
        }

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
                var producent = request.ToEntity();
                
                if (request.photo is not null)
                {
                    var uploadResult = await _photoService.AddPhoto(request.photo);
                    producent.Photo = uploadResult.SecureUri.AbsoluteUri;
                }
                if (producent != null)
                {
                    await _context.Producents.AddAsync(producent);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(producent => producent.firstName)
                    .NotEmpty()
                    .WithMessage("This field is required!");

                RuleFor(producent => producent.lastName)
                    .NotEmpty()
                    .WithMessage("This field is required!");

                RuleFor(producent => producent.country)
                    .NotEmpty()
                    .WithMessage("This field is required!");
            }
        }
    }
}
