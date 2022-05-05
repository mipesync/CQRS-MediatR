using MediatR;

namespace CQRS_MediatR.BLL.Commands
{
    public record DeleteUserCommand(string Id) : IRequest;
}
