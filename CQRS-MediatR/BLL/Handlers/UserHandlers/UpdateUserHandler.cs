using CQRS_MediatR.API.Models;
using CQRS_MediatR.BLL.Commands;
using MediatR;
using AppContext = CQRS_MediatR.API.DBContext.AppContext;

namespace CQRS_MediatR.BLL.Handlers.UserHandlers
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, User>
    {
        private readonly AppContext _context;
        public UpdateUserHandler(AppContext context) => _context = context;

        public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            request.User.Name = request.UserDto.Name is null ? request.User.Name : request.UserDto.Name;
            request.User.Username = request.UserDto.Username is null ? request.User.Username : request.UserDto.Username;
            request.User.PassHash = request.UserDto.PassHash is null ? request.User.PassHash : BCrypt.Net.BCrypt.EnhancedHashPassword(request.UserDto.PassHash);

            _context.Update(request.User);
            await _context.SaveChangesAsync();

            return request.User;
        }
    }
}
