using CQRS_MediatR.BLL.Commands;
using MediatR;
using AppContext = CQRS_MediatR.API.DBContext.AppContext;

namespace CQRS_MediatR.BLL.Handlers.UserHandlers
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly AppContext _context;
        public DeleteUserHandler(AppContext context) => _context = context;

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            _context.Users.Remove(request.User);
            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
