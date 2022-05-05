using MediatR;

namespace CQRS_MediatR.API.BLL.Commands
{
    public record DeleteUserCommand(string Id) : IRequest;
}
