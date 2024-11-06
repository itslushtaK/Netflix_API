using Application.Common.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using SendGrid.Helpers.Errors.Model;

namespace Application.Features.Actors
{
    public class UpdatePhoto
    {

        public record Command(int id ,IFormFile photo) : IRequest;

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;
            private readonly IFileStorageService _fileStorageService;
            public CommandHandler(IApplicationDbContext context , IFileStorageService fileStorageService)
            {
                _context = context;
                _fileStorageService = fileStorageService;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var actor = await _context.Actors.FindAsync(request.id);

                if (actor == null)
                {
                    throw new NotFoundException($"Actor with ID {request.id} not found.");
                }
                if (!string.IsNullOrEmpty(actor.Photo) && request.photo != null)
                {
                    await _fileStorageService.DeleteFileAsync(actor.Photo);
                }

                string photo = null;
                if (request.photo != null)
                {
                    photo = await _fileStorageService.UploadFileAsync(request.photo);
                }

                actor.Photo = photo;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
