using CQRS_MediatR.API.Models;
using MediatR;

namespace CQRS_MediatR.API.BLL.Commands
{
    public record CreateUserCommand(User User): IRequest<User>;
}
