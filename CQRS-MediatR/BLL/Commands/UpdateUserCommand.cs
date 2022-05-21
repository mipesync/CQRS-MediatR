using CQRS_MediatR.API.Models;
using MediatR;
using static CQRS_MediatR.API.Controllers.UsersController;

namespace CQRS_MediatR.BLL.Commands
{
    public record UpdateUserCommand(UserDto UserDto, User User) : IRequest<User>;
}
