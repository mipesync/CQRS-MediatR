using CQRS_MediatR.Models;
using MediatR;

namespace CQRS_MediatR.BLL.Commands
{
    public record UpdateUserCommand(User User) : IRequest<User>;
}
