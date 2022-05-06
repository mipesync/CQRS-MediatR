using CQRS_MediatR.API.Models;
using MediatR;

namespace CQRS_MediatR.BLL.Commands
{
    public record AuthUserCommand(User User) : IRequest<User?>;
}
