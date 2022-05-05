using CQRS_MediatR.Models;
using MediatR;

namespace CQRS_MediatR.BLL.Commands
{
    public record CreateUserCommand(User User): IRequest<User>;
}
