using Application.Common.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using SendGrid.Helpers.Errors.Model;

namespace Application.Features.Movies
{
    public class AddUpdatePhoto
    {
        public record Command(int id , IFormFile? photo) : IRequest;
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
                var movie = await _context.Movies.FindAsync(request.id);

                if(movie == null)
                {
                    throw new NotFoundException($"Actor with ID {request.id} not found.");
                }
                if(!string.IsNullOrEmpty(movie.Photo) && request.photo != null)
                {
                    await _fileStorageService.DeleteFileAsync(movie.Photo);
                }

                string photo = null;

                if (request.photo != null) 
                {
                    photo = await _fileStorageService.UploadFileAsync(request.photo);
                }

                movie.Photo = photo;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
