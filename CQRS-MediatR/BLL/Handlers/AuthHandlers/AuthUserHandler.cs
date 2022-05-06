using CQRS_MediatR.API.Models;
using CQRS_MediatR.BLL.Commands;
using MediatR;
using Microsoft.Data.Sqlite;
using AppContext = CQRS_MediatR.API.DBContext.AppContext;

namespace CQRS_MediatR.BLL.Handlers.AuthHandlers
{
    public class AuthUserHandler : IRequestHandler<AuthUserCommand, User?>
    {
        private readonly AppContext _context;
        private readonly string _connection = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
            .Build().GetSection("ConnectionStrings:Sqlite").Value;

        public AuthUserHandler(AppContext context) => _context = context;

        public Task<User?> Handle(AuthUserCommand request, CancellationToken cancellationToken)
        {
            User? user = null;
            var sqlExpression = $"SELECT * FROM Users WHERE Username='{request.User.Username}'";
            using (var sqliteConnection = new SqliteConnection(_connection))
            {
                sqliteConnection.Open();

                var sqliteCommand = new SqliteCommand(sqlExpression, sqliteConnection);
                using (var reader = sqliteCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetValue(0).ToString();
                            var name = reader.GetValue(1).ToString();
                            var username = reader.GetValue(2).ToString();
                            var passHash = reader.GetValue(2).ToString();

                            if (BCrypt.Net.BCrypt.EnhancedVerify(request.User.PassHash, passHash))
                                user = new User { Id = id, Name = name, Username = username!, PassHash = passHash! };
                        }
                    }
                }
            }
            return Task.FromResult(user);
        }
    }
}
