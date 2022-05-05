using CQRS_MediatR.API.BLL.Queries;
using CQRS_MediatR.API.Models;
using MediatR;
using Microsoft.Data.Sqlite;
using AppContext = CQRS_MediatR.API.DBContext.AppContext;

namespace CQRS_MediatR.API.BLL.Handlers
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, User>
    {
        private readonly AppContext _context;
        private readonly string _connection = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
            .Build().GetSection("ConnectionStrings:Sqlite").Value;

        public GetUserByIdHandler(AppContext context) => _context = context;

        public Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            User user = null!;

            var sqlExpression = $"SELECT * FROM Users WHERE Id='{request.id}'";
            using (var sqliteConnection = new SqliteConnection(_connection))
            {
                sqliteConnection.Open();

                SqliteCommand sqliteCommand = new SqliteCommand(sqlExpression, sqliteConnection);
                using (var reader = sqliteCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var _id = reader.GetValue(0).ToString();
                            var name = reader.GetValue(1).ToString();
                            var username = reader.GetValue(2).ToString();
                            var passhash = reader.GetValue(3).ToString();

                            user = new User { Id = _id!, Name = name!, Username = username!, PassHash = passhash! };
                        }
                    }
                }
            }
            return Task.FromResult(user);
        }
    }
}
