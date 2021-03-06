using CQRS_MediatR.API.Models;
using CQRS_MediatR.BLL.Commands;
using MediatR;
using AppContext = CQRS_MediatR.API.DBContext.AppContext;

namespace CQRS_MediatR.BLL.Handlers.UserHandlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly AppContext _context;

        public CreateUserHandler(AppContext context) => _context = context;

        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            request.User.PassHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.User.PassHash);
            _context.Add(request.User);
            await _context.SaveChangesAsync();
            return request.User;
        }
    }
}
