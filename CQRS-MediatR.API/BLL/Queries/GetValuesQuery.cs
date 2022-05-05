using CQRS_MediatR.API.Models;
using MediatR;

namespace CQRS_MediatR.API.BLL.Queries
{
    public record GetUsersQuery() : IRequest<IEnumerable<User>>;
    public record GetUserByIdQuery(string id) : IRequest<User>;
}
