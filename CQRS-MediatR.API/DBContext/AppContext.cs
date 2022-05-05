using CQRS_MediatR.API.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CQRS_MediatR.API.DBContext
{
    public partial class AppContext : DbContext
    {
        public AppContext() => Database.EnsureCreated();

        public AppContext(DbContextOptions<AppContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
