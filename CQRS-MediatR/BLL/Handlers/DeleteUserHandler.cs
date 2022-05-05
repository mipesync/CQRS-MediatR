using CQRS_MediatR.BLL.Commands;
using MediatR;
using AppContext = CQRS_MediatR.DBContext.AppContext;

namespace CQRS_MediatR.BLL.Handlers
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly AppContext _context;
        public DeleteUserHandler(AppContext context) => _context = context;

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.Id);

            _context.Users.Remove(user!);
            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
