using CQRS_MediatR.API.Models;
using MediatR;

namespace CQRS_MediatR.API.BLL.Commands
{
    public record UpdateUserCommand(User User) : IRequest<User>;
}
