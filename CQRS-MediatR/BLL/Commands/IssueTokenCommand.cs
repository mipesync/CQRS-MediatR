using CQRS_MediatR.API.Models;
using MediatR;

namespace CQRS_MediatR.BLL.Commands
{
    public record IssueTokenCommand(User User, HttpContext Context) : IRequest<string>;
}
