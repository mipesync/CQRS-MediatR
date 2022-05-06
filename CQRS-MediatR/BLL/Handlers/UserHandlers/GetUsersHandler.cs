using CQRS_MediatR.API.Models;
using CQRS_MediatR.BLL.Queries;
using MediatR;
using Microsoft.Data.Sqlite;
using AppContext = CQRS_MediatR.API.DBContext.AppContext;

namespace CQRS_MediatR.BLL.Handlers.UserHandlers
{
    public class GetUsersHandler : IRequestHandler<GetUsersQuery, List<User>>
    {
        private readonly AppContext _context;
        private readonly string _connection = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
            .Build().GetSection("ConnectionStrings:Sqlite").Value;

        public GetUsersHandler(AppContext context) => _context = context;

        public Task<List<User>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            List<User> users = new List<User>();

            var sqlExpression = "SELECT * FROM Users";
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
                            var passhash = reader.GetValue(3).ToString();

                            users.Add(new User { Id = id!, Name = name!, Username = username!, PassHash = passhash! });
                        }
                    }
                }
            }
            return Task.FromResult(users);
        }
    }
}
