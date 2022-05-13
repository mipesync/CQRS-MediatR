using CQRS_MediatR.API.Models;
using CQRS_MediatR.BLL.Queries;
using MediatR;
using Microsoft.Data.Sqlite;
using AppContext = CQRS_MediatR.API.DBContext.AppContext;

namespace CQRS_MediatR.BLL.Handlers.UserHandlers
{
    public class GetUserByUsernameHandler : IRequestHandler<GetUserByUsernameQuery, User?>
    {
        private readonly string _connection = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
            .Build().GetSection("ConnectionStrings:Sqlite").Value;

        public Task<User?> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
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
                            var passHash = reader.GetValue(3).ToString();

                            user = new User { Id = id!, Name = name, Username = username!, PassHash = passHash! };
                        }
                    }
                }
            }
            return Task.FromResult(user);
        }
    }
}
