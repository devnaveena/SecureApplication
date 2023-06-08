using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace SecureApplicationTests.Helpers
{
    public class InMemorydataContext
    {
        public static RepositoryContext Inmemory()
        {
            var options = new DbContextOptionsBuilder<RepositoryContext>().UseInMemoryDatabase(databaseName: "MyDatabase").Options;
            RepositoryContext? context = new RepositoryContext(options);
            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            SeedData(context!);

            return context!;
        }
        private static void SeedData(RepositoryContext context)
        {

            context.Book.Add(new Book
            {
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                Description = "A classic American novel depicting the lavish and decadent lifestyle of the 1920s.",
                PublishDate = new DateTime(1925, 4, 10),
                IsbnNumber = "XRDTA14s5Ptp21GyVgYW0ibRrInsDf3Pb1nuEO7fNJs=",
                Publisher = "Scribner",
                Location = "New York",
                Language = "English",
                Genre = "Fiction",
                Id = new Guid("0B5851A4-02CA-4370-8D1B-130C36D369CA"),
                PageCount = 180,
                Price = 12.99m,
                IsAvailable = true
            });
            context.User.Add(new User
            {
                UserName = "Naveena",
                Password = "69c0fc804c49254d7741c6c684802cd111b6fd34e472a64d8524f2d207e9a074d684b27b7f79674b18dc90b2e9859a24f8be2cff051e17d416f111444caebb03",
                DateOfBirth = new DateTime(2002 - 05 - 30),
                Email = "navee@example.com",
                Phone = 9344958244,
                Role = "Admin",
                Gender = "Female",
                PasswordSalt = "3x9N1qrOo2plfzvXavCPqA==",
                Id = new Guid("0B5851A4-02CA-4370-8D1B-130C36D379CA"),
                IsActive = true,
                DateCreated = DateTime.Now,
                DateUpdated = null
            });
            context.User.Add(new User
            {
                UserName = "Abi",
                Password = "88e506dfc7693e9d297b257e12ccc1a530cf6173092663cd6cac02287274040ab3fc8685fbf53d3dbc1b96e35ef50181bda690198193b1217e06d62d3cca305b",
                DateOfBirth = new DateTime(2002 - 05 - 30),
                Email = "Abi@example.com",
                Phone = 9344958244,
                Role = "Reader",
                Gender = "Female",
                PasswordSalt = "FQG7BPgwHOf3VNaIbAe48Q==",
                Id = new Guid("F31A8980-51C1-49B6-B77B-7D70D32B2157"),
                IsActive = true,
                DateCreated = DateTime.Now,
                DateUpdated = null
            });

            context.SaveChanges();

        }

    }
}