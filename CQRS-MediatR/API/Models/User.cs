namespace CQRS_MediatR.API.Models
{
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string? Id { get; set; }
        public string? Name { get; set; }
        public string Username { get; set; } = null!;
        public string PassHash { get; set; } = null!;
    }
}
