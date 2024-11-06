using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Roles
{
    public class SeedRolesCommand
    {
        public record Command(IEnumerable<SeedRole> Roles) : IRequest<Unit>;

        public class SeedRolesCommandHandler : IRequestHandler<Command, Unit>
        {
            private readonly RoleManager<IdentityRole> _roleManager;

            public SeedRolesCommandHandler(RoleManager<IdentityRole> roleManager)
            {
                _roleManager = roleManager;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                foreach (var roleModel in request.Roles)
                {
                    if (!await _roleManager.RoleExistsAsync(roleModel.Name))
                    {
                        var role = new IdentityRole(roleModel.Name);
                        await _roleManager.CreateAsync(role);
                    }
                }

                return Unit.Value;
            }
        }
    }
}
