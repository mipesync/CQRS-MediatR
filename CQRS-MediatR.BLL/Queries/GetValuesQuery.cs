using CQRS_MediatR.API.Models;
using MediatR;

namespace CQRS_MediatR.BLL.Queries
{
    public class GetValuesQuery
    {
        public record GetUsersQuery() : IRequest<IEnumerable<User>>;
    }
}
