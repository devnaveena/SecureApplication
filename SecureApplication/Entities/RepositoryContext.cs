using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Entities
{

    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Book> Book { get; set; }

    }
}