namespace CQRS_MediatR.Models
{
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string PassHash { get; set; } = null!;
    }
}
