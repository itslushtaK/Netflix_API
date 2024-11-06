using Application.Common.Abstractions;
using FluentValidation;
using MediatR;
using SendGrid.Helpers.Errors.Model;

namespace Application.Features.Actors
{
    public class Update
    {

        public record Command(int id ,string firstName , string lastName , int age /*, IFormFile photo*/) : IRequest;

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;
            private readonly IPhotoService _photoService;

            public CommandHandler(IApplicationDbContext context , IPhotoService photoService)
            {
                _context = context;
                _photoService = photoService;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var actor = _context.Actors.FirstOrDefault(actor => actor.Id == request.id);

                if(actor is null)
                {
                    throw new NotFoundException();
                }
                actor.FirstName = request.firstName;
                actor.LastName= request.lastName;
                actor.Age = request.age;
                //var uploadResult = await _photoService.AddPhoto(request.photo);
                //actor.Photo = uploadResult.SecureUri.AbsoluteUri;        
                
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(actor => actor.firstName)
                    .NotEmpty()
                    .WithMessage("FirstName is required!");

                RuleFor(actor => actor.lastName)
                   .NotEmpty()
                   .WithMessage("LastName is required!");

                //RuleFor(actor => actor.photo)
                //  .NotEmpty()
                //  .WithMessage("Photo is required!");
            }
        }
    }
}
